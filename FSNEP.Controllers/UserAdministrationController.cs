using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CAESArch.BLL;
using CAESArch.Core.DataInterfaces;
using FSNEP.BLL.Impl;
using FSNEP.Controllers.Helpers;
using FSNEP.Controllers.Helpers.Extensions;
using FSNEP.Core.Domain;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using MvcContrib.Attributes;
using MvcContrib;
using System;
using System.Web.Security;
using FSNEP.Core.Abstractions;
using CAESArch.Core.Utils;
using CAESArch.Core.Validators;

namespace FSNEP.Controllers
{
    [Authorize]
    public class UserAdministrationController : SuperController
    {
        private const string DefaultPassword = "jaskidjflkajsdlf$#12";

        public IUserBLL UserBLL;
        public IMessageGateway MessageGateway;

        public UserAdministrationController(IUserBLL userBLL, IMessageGateway messageGateway)
        {
            UserBLL = userBLL;
            MessageGateway = messageGateway;
        }

        /// <summary>
        /// Provides a list of all active users in the system
        /// </summary>
        public ActionResult List()
        {
            var users = UserBLL.GetAllUsers();

            return View(users.ToList());
        }

        /// <summary>
        /// TODO: Remove this testing method
        /// </summary>
        public ActionResult DeleteUser(string id)
        {
            //This only works with tester accounts
            if (id.StartsWith("tester"))
            {
                var user = UserBLL.GetUser(id);

                using (var ts = new TransactionScope())
                {
                    UserBLL.Remove(user);

                    ts.CommitTransaction();
                }

                UserBLL.UserAuth.MembershipService.DeleteUser(id);
            }
            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        public ActionResult Create()
        {
            //Create the viewmodel with a blank user
            var viewModel = new CreateUserViewModel { User = new User { FTE = 1, IsActive = true } };

            PopulateDefaultUserViewModel(viewModel);

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult Create(CreateUserViewModel model, Guid? supervisorId, IEnumerable<int> projectList,
                                       IEnumerable<int> fundTypeList, List<string> roleList)
        {

            var user = model.User;
            user.Supervisor = new User();

            ValidationHelper<CreateUserViewModel>.Validate(model, ModelState); //Validate the create user properties

            CheckUserProperties(supervisorId, projectList, fundTypeList); //Make sure the associations are set

            ValidationHelper<User>.Validate(user, ModelState, "User"); //validate the user properties

            if (roleList == null) ModelState.AddModelError("RoleList", "User must have at least one role");

            if (!ModelState.IsValid)
            {
                return Create();
            }

            PopulateUserProperties(user, supervisorId, projectList, fundTypeList);

            EnsureProperRoles(roleList, user);

            MembershipCreateStatus createStatus;

            //Create the user
            MembershipUser membershipUser = UserBLL.UserAuth.MembershipService.CreateUser(model.UserName,
                                                                                          DefaultPassword,
                                                                                          model.Email, model.Question,
                                                                                          model.Answer, true, null,
                                                                                          out createStatus);            
            if (createStatus == MembershipCreateStatus.Success)
            {
                UserBLL.AddUserToRoles(model.UserName, roleList);
            }
            else
            {
                //TODO: provide more meaningful return values as they are added and link them to specific fields (Rememeber Unit Tests)
                switch (createStatus)
                {
                    case MembershipCreateStatus.DuplicateEmail:
                        //This is currently disabled in the Web.config
                        ModelState.AddModelError("_FORM", "Create Failed Duplicate Email");
                        return Create();
                    case MembershipCreateStatus.DuplicateProviderUserKey:
                        ModelState.AddModelError("_FORM", "Create Failed Duplicate Provider User Key");
                        return Create();
                    case MembershipCreateStatus.DuplicateUserName:
                        //This one should be working 
                        ModelState.AddModelError("UserName", "Username already exists");
                        return Create();                        
                    case MembershipCreateStatus.InvalidAnswer:
                        ModelState.AddModelError("_FORM", "Create Failed Invalid Answer");
                        return Create();
                    case MembershipCreateStatus.InvalidEmail:
                        ModelState.AddModelError("_FORM", "Create Failed Invalid Email");
                        return Create();
                    case MembershipCreateStatus.InvalidPassword:
                        ModelState.AddModelError("_FORM", "Create Failed Invalid Password");
                        return Create();
                    case MembershipCreateStatus.InvalidProviderUserKey:
                        ModelState.AddModelError("_FORM", "Create Failed Invalid Provider User Key");
                        return Create();
                    case MembershipCreateStatus.InvalidQuestion:
                        ModelState.AddModelError("_FORM", "Create Failed Invalid Question");
                        return Create();
                    case MembershipCreateStatus.InvalidUserName:
                        ModelState.AddModelError("_FORM", "Create Failed Invalid User Name");
                        return Create();
                    case MembershipCreateStatus.ProviderError:
                        ModelState.AddModelError("_FORM", "Create Failed Provider Error");
                        return Create();
                    case MembershipCreateStatus.Success:
                        break;
                    case MembershipCreateStatus.UserRejected:
                        ModelState.AddModelError("_FORM", "Create Failed User Rejected");
                        return Create();
                    default:
                        ModelState.AddModelError("_FORM", "Create Failed");
                        return Create();

                }
                
            }

            

            user.SetUserID((Guid)membershipUser.ProviderUserKey);

            user.Token = Guid.NewGuid(); //setup the new user token

            var ts = new TransactionScope();

            try
            {
                //save the user
                UserBLL.EnsurePersistent(user);

                //Send the user a message
                //var newUserTokenPath = Url.AbsoluteAction("Index", "Home", new { token = user.Token });
                //var supervisorEmail = UserBLL.UserAuth.MembershipService.GetUser(user.Supervisor.ID).Email;
                //MessageGateway.SendMessageToNewUser(user, model.UserName, model.Email, supervisorEmail, newUserTokenPath);

                var supervisorEmail = UserBLL.UserAuth.MembershipService.GetUser(user.Supervisor.ID).Email;
                MessageGateway.SendMessageToNewUser(user, model.UserName, model.Email, supervisorEmail, Url.AbsoluteAction("Index", "Home", new { token = user.Token }));

                ts.CommitTransaction();
            }
            catch (Exception)
            {
                ts.RollBackTransaction();

                UserBLL.UserAuth.MembershipService.DeleteUser(model.UserName);
                //delete the user then throw the exception

                throw;
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Maps the guid user ident to a username ident
        /// </summary>
        public ActionResult ModifyById(Guid? id)
        {
            if (id== null) return this.RedirectToAction(a => a.Create());

            var user = UserBLL.UserAuth.MembershipService.GetUser(id);

            return user == null ? this.RedirectToAction(a => a.Create()) : this.RedirectToAction(a => a.Modify(user.UserName));
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.RedirectToAction(a => a.Create());
            }

            var user = UserBLL.GetUser(id);

            //If the user could not be found, redirect to creating a user
            if (user == null) return this.RedirectToAction(a => a.Create());

            var viewModel = UserViewModel.Create(UserBLL);
            viewModel.User = user;

            //Now the user roles are the roles for the given id
            viewModel.UserRoles = UserBLL.GetUserRoles(id);

            return View(viewModel);
        }

        [AcceptPost]
        public string Edit(UserViewModel userViewModel)
        {
            var result = new StringBuilder();

            result.AppendFormat("Supervisor is {0}", userViewModel.User.Supervisor != null ? userViewModel.User.Supervisor.FullName : "Null" );
            
            result.AppendLine("<br/>");
            result.AppendFormat("Supervisor is {0}", userViewModel.User.Supervisor);
            
            result.AppendLine("<br/>");
            result.AppendFormat("Supervisor is {0}", userViewModel.User.Supervisor);
            
            result.AppendLine("<br/>");
            result.AppendFormat("Supervisor is {0}", userViewModel.User.Supervisor);

            return result.ToString();
        }

        /// <summary>
        /// Returns the user object indentified by the given userid.  If there is no user, return just the other information needed for creating a new user.
        /// </summary>
        /// <param name="id">the userid/username</param>
        public ActionResult Modify(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.RedirectToAction(a => a.Create());
            }

            var user = UserBLL.GetUser(id);

            //If the user could not be found, redirect to creating a user
            if (user == null) return this.RedirectToAction(a => a.Create());

            var viewModel = UserViewModel.Create(UserBLL);
            viewModel.User = user;

            //Now the user roles are the roles for the given id
            viewModel.UserRoles = UserBLL.GetUserRoles(id);

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult Modify(User user, List<string> roleList)
        {
            //ValidationHelper<User>.Validate(user, ModelState);

            var viewModel = UserViewModel.Create(UserBLL);
            viewModel.User = user;

            return View(viewModel);

            #region OldCode
            /*
            var user = UserBLL.GetUser(id);

            TryUpdateModel(user, "User"); //Update the user from the data entered in the form

            CheckUserProperties(supervisorId, projectList, fundTypeList);

            ValidationHelper<User>.Validate(user, ModelState, "User");

            if (roleList == null) ModelState.AddModelError("RoleList", "User must have at least one role");

            if (!ModelState.IsValid)
            {
                return Modify(id);
            }

            PopulateUserProperties(user, supervisorId, projectList, fundTypeList);

            if (roleList == null) roleList = new List<string>();

            EnsureProperRoles(roleList, user);

            // If the user has subordinates, make sure they have supervisor role
            if (UserBLL.GetSubordinates(user).Count() > 0 && !roleList.Contains(RoleNames.RoleSupervisor))
            {
                roleList.Add(RoleNames.RoleSupervisor);
            }
 
            //Now reconcile the user's roles
            UserBLL.SetRoles(id, roleList);
            
            //We have a valid viewstate, so save the changes
            using (var ts = new TransactionScope())
            {
                UserBLL.EnsurePersistent(user);

                ts.CommitTransaction();
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
             */

            #endregion
        }

        /// <summary>
        /// Check the associated user properties for validity
        /// </summary>
        private void CheckUserProperties(Guid? supervisorId, IEnumerable<int> projectList, IEnumerable<int> fundTypeList)
        {
            //TODO: Review. With Unit tests, it is possible to supply these values and have them cleared out when the method "PopulateUserProperties" is encountered. Probably can't happen through the UI, but maybe we want to test later as well?
            //Make sure we get a supervisor, some projects and some fundtypes
            if (!supervisorId.HasValue) ModelState.AddModelError("SupervisorID", "You must select a supervisor");

            if (projectList == null) ModelState.AddModelError("ProjectList", "You must select at least one project");

            if (fundTypeList == null)
                ModelState.AddModelError("FundTypeList", "You must select at least one fund type");
        }

        /// <summary>
        /// Populate the given user with the proper associated properties
        /// </summary>
        private void PopulateUserProperties(User user, Guid? supervisorId, IEnumerable<int> projectList,
                                            IEnumerable<int> fundTypeList)
        {
            user.Supervisor = UserBLL.GetByID(supervisorId.Value);

            var projects = from proj in Repository.OfType<Project>().Queryable
                           where projectList.Contains(proj.ID)
                           select proj;

            var fundtypes = from ft in Repository.OfType<FundType>().Queryable
                            where fundTypeList.Contains(ft.ID)
                            select ft;

            user.Projects = projects.ToList();
            user.FundTypes = fundtypes.ToList();
        }

        private void PopulateDefaultUserViewModel(UserViewModel viewModel)
        {
            throw new NotImplementedException();
            /*
            viewModel.Supervisors = new SelectList(UserBLL.GetSupervisors(), "ID", "FullName",
                                                   viewModel.User.Supervisor != null
                                                       ? viewModel.User.Supervisor.ID
                                                       : Guid.Empty);

            viewModel.Projects = new MultiSelectList(UserBLL.GetAllProjectsByUser().ToList(), "ID", "Name",
                                                     viewModel.User.Projects.Select(p => p.ID));

            viewModel.FundTypes = new MultiSelectList(UserBLL.GetAvailableFundTypes().ToList(), "ID", "Name",
                                                      viewModel.User.FundTypes.Select(p => p.ID));

            viewModel.AvailableRoles = UserBLL.GetAllRoles();
             */
        }

        private static void EnsureProperRoles(ICollection<string> roles, User user)
        {
            //Business role checks
            Check.Require(roles != null);

            //If user selects a 'state' fund type, ensure the timehseet role
            bool hasStateType = false;

            foreach (var ft in user.FundTypes)
            {
                if (ft.Name.StartsWith("State", StringComparison.OrdinalIgnoreCase))
                {
                    hasStateType = true;
                }
            }

            if (hasStateType && !roles.Contains(RoleNames.RoleTimeSheet))
            {
                roles.Add(RoleNames.RoleTimeSheet);
            }
        }
    }

    public class CreateUserViewModel : UserViewModel
    {
        [RequiredValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        public string UserName { get; set; }

        [RequiredValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        [RegexValidator(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase,
            MessageTemplate = "Must be a valid email address")]
        public string Email { get; set; }

        [RequiredValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        public string Question { get; set; }

        [RequiredValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        public string Answer { get; set; }
    }

    public class UserViewModel
    {
        /// <summary>
        /// Creates the user view model, including populating the lookups
        /// </summary>
        public static UserViewModel Create(IUserBLL userBLL)
        {
            var viewModel = new UserViewModel
                                {
                                    Supervisors = userBLL.GetSupervisors().OrderBy(a => a.LastName).ToList(),
                                    Projects = userBLL.GetAllProjectsByUser().OrderBy(a => a.Name).ToList(),
                                    FundTypes = userBLL.GetAvailableFundTypes().OrderBy(a => a.Name).ToList(),
                                    AvailableRoles = userBLL.GetAllRoles()
                                };

            return viewModel;
        }

        public User User { get; set; }
        public List<User> Supervisors { get; set; }
        public List<Project> Projects { get; set; }
        public List<FundType> FundTypes { get; set; }

        public IEnumerable<string> AvailableRoles { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}