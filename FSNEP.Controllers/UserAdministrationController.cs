using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.Controllers.Helpers.Extensions;
using FSNEP.Core.Domain;
using System.Linq;
using MvcContrib.Attributes;
using MvcContrib;
using System;
using System.Web.Security;
using FSNEP.Core.Abstractions;
using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;
using UCDArch.Core.Utils;
using UCDArch.Web.Attributes;
using UCDArch.Web.Helpers;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Validator;

namespace FSNEP.Controllers
{
    [Authorize]
    public class UserAdministrationController : SuperController
    {
        private static readonly string DefaultPassword = Constants.STR_Default_Pass;

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
        [Transaction]
        public ActionResult List()
        {
            var users = UserBLL.GetAllUsers();

            return View(users.ToList());
        }

        /// <summary>
        /// TODO: Remove this testing method
        /// </summary>
        [Transaction]
        public ActionResult DeleteUser(string id)
        {
            //This only works with tester accounts
            if (id.StartsWith("tester"))
            {
                var user = UserBLL.GetUser(id);

                UserBLL.Remove(user);

                UserBLL.UserAuth.MembershipService.DeleteUser(id);
            }
            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Create the User View Model
        /// </summary>
        /// <returns>CreateUserViewModel</returns>
        public ActionResult Create()
        {
            var viewModel = CreateUserViewModel.Create(UserBLL, Repository);
            viewModel.User = new User { FTE = 1, IsActive = true }; //Default these two values

            return View(viewModel);
        }

        /// <summary>
        /// Create the user from the "Create User View Model" and try to persist it
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roleList">List of roles for this user. May be added to by code.</param>
        /// <returns>Either the user view model on failure or the list of users on success</returns>
        [AcceptPost]
        public ActionResult Create(CreateUserViewModel model, List<string> roleList)
        {
            model.User.UserName = model.UserName; //transfer the username to the user class

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState,
                                                              MvcValidationAdapter.GetValidationResultsFor(model));

            model.User.TransferValidationMessagesTo("User", ModelState);
            
            //CheckUserAssociations(model.User); //Make sure the associations are set//Done by users.cs now

            if (roleList == null || roleList.Count == 0)
                ModelState.AddModelError("RoleList", "User must have at least one role");

            if (!ModelState.IsValid)
            {
                //If we aren't valid, return to the create page
                var viewModel = CreateUserViewModel.Create(UserBLL, Repository);
                viewModel.TransferValuesFrom(model);

                viewModel.UserRoles = roleList;

                return View(viewModel);
            }
            else
            {
                var user = model.User;

                //Set the current user as the new user's creator
                user.CreatedBy = UserBLL.GetUser();

                //User is valid, let's go through the create process
                EnsureProperRoles(roleList, user);

                MembershipCreateStatus createStatus;

                //Create the user
                MembershipUser membershipUser = UserBLL.UserAuth.MembershipService.CreateUser(model.UserName,
                                                                                              DefaultPassword,
                                                                                              model.Email,
                                                                                              model.Question,
                                                                                              model.Answer, true, null,
                                                                                              out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    UserBLL.AddUserToRoles(model.UserName, roleList);
                }
                else
                {
                    //This approach maintains all entered info for the user if there is an error
                    switch (createStatus)
                    {
                        case MembershipCreateStatus.DuplicateEmail:
                            //This is currently disabled in the Web.config
                            ModelState.AddModelError("_FORM", "Create Failed Duplicate Email");
                            break; // return Create();
                        case MembershipCreateStatus.DuplicateProviderUserKey:
                            ModelState.AddModelError("_FORM", "Create Failed Duplicate Provider User Key");
                            break; // return Create();
                        case MembershipCreateStatus.DuplicateUserName:
                            //This one should be working 
                            ModelState.AddModelError("UserName", "Username already exists");
                            break; // return Create();
                        case MembershipCreateStatus.InvalidAnswer:
                            ModelState.AddModelError("_FORM", "Create Failed Invalid Answer");
                            break; // return Create();
                        case MembershipCreateStatus.InvalidEmail:
                            ModelState.AddModelError("_FORM", "Create Failed Invalid Email");
                            break; // return Create();
                        case MembershipCreateStatus.InvalidQuestion:
                            ModelState.AddModelError("_FORM", "Create Failed Invalid Question");
                            break; // return Create();
                        case MembershipCreateStatus.InvalidUserName:
                            ModelState.AddModelError("_FORM", "Create Failed Invalid User Name");
                            break; // return Create();
                        case MembershipCreateStatus.ProviderError:
                            ModelState.AddModelError("_FORM", "Create Failed Provider Error");
                            break; // return Create();
                        //Added these two back, if we don't want them here we can disable the two unit tests.
                        case MembershipCreateStatus.InvalidProviderUserKey:
                            ModelState.AddModelError("_FORM", "Create Failed Invalid Provider User Key");
                            break; // return Create();
                        case MembershipCreateStatus.InvalidPassword:
                            ModelState.AddModelError("_FORM", "Create Failed Invalid Password");
                            break; // return Create();
                        case MembershipCreateStatus.Success:
                            throw new ApplicationException("Unexpected value.");
                        case MembershipCreateStatus.UserRejected:
                            ModelState.AddModelError("_FORM", "Create Failed User Rejected");
                            break; // return Create();
                        default:
                            ModelState.AddModelError("_FORM", "Create Failed");
                            break; // return Create();

                    }
                    //If we aren't valid, return to the create page
                    var viewModel = CreateUserViewModel.Create(UserBLL, Repository);
                    viewModel.TransferValuesFrom(model);

                    viewModel.UserRoles = roleList;

                    return View(viewModel);
                }



                user.SetUserID((Guid) membershipUser.ProviderUserKey);

                user.Token = Guid.NewGuid(); //setup the new user token

                UserBLL.DbContext.BeginTransaction();
                
                try
                {
                    //save the user
                    UserBLL.EnsurePersistent(user);

                    var supervisorEmail = UserBLL.UserAuth.MembershipService.GetUser(user.Supervisor.Id).Email;
                    //MessageGateway.SendMessageToNewUser(user, model.UserName, model.Email, supervisorEmail,
                    //                                    Url.AbsoluteAction("Index", "Home", new {token = user.Token}));
                    MessageGateway.SendMessageToNewUser(user, model.UserName, model.Email, supervisorEmail,
                                                        Url.AbsoluteAction("NewUser", "Account", new {id = user.Token}));
                    

                    UserBLL.DbContext.CommitTransaction();
                }
                catch (Exception)
                {
                    UserBLL.DbContext.RollbackTransaction();

                    //delete the user then throw the exception
                    UserBLL.UserAuth.MembershipService.DeleteUser(model.UserName);

                    throw;
                }

                Message = string.Format("{0} Created Successfully", model.UserName);

                return this.RedirectToAction(a => a.List());
            }
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

            var viewModel = UserViewModel.Create(UserBLL, Repository);
            viewModel.User = user;

            //Make sure that any projects that the user already has show up in the view of projects even if the current user doesn't have those projects.
            if(user.Projects != null)
            {
                foreach (Project project in user.Projects)
                {
                    if(!viewModel.Projects.Contains(project))
                    {
                        viewModel.Projects.Add(project);
                    }
                }
            }

            //Now the user roles are the roles for the given id
            viewModel.UserRoles = UserBLL.GetUserRoles(id);

            //If the UserRoles contains values not in the Available roles, add them. Otherwise
            //they would be removed when the user is saved.
            if(viewModel.UserRoles != null)
            {
                var joinRoles = new List<string>();
                foreach (string userRole in viewModel.UserRoles)
                {
                    if(!viewModel.AvailableRoles.Contains(userRole))
                    {
                        joinRoles.Add(userRole);
                    }
                }
                if(joinRoles.Count > 0)
                {
                    foreach (string availableRole in viewModel.AvailableRoles)
                    {
                        if(!joinRoles.Contains(availableRole))
                        {
                            joinRoles.Add(availableRole);
                        }
                    }
                }
                if(joinRoles.Count > 0)
                {
                    viewModel.AvailableRoles = joinRoles.AsEnumerable(); 
                }
            }

            return View(viewModel);
        }

        /// <summary>
        /// Attempt to persist the user changes
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleList">List of roles for the user. May be added to by code.</param>
        /// <param name="id">UserName/ID</param>
        /// <returns>Either the user view model on failure or the list of users on success</returns>
        [AcceptPost]
        [Transaction]
        public ActionResult Modify(User user, List<string> roleList, string id)
        {
            var userToUpdate = UserBLL.GetUser(id);

            TransferValuesTo(userToUpdate, user);

            userToUpdate.TransferValidationMessagesTo(ModelState);
            
            //CheckUserAssociations(userToUpdate); //Done by User.cs now

            if (roleList == null || roleList.Count == 0)
                ModelState.AddModelError("RoleList", "User must have at least one role");

            if (!ModelState.IsValid)
            {
                var viewModel = UserViewModel.Create(UserBLL, Repository);
                viewModel.User = user;
                viewModel.UserRoles = roleList; //Prevent the roles from being cleared out with an error

                return View(viewModel);
            }
            else
            {
                //Do the save
                EnsureProperRoles(roleList, userToUpdate);

                //Now reconcile the user's roles
                UserBLL.SetRoles(id, roleList);

                //We have a valid viewstate, so save the changes
                UserBLL.EnsurePersistent(userToUpdate);

                Message = string.Format("{0} Modified Successfully", id);

                return this.RedirectToAction(a => a.List());
            }
        }

        private static void TransferValuesTo(User userToUpdate, User user)
        {
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Salary = user.Salary;
            userToUpdate.FTE = user.FTE;
            userToUpdate.BenefitRate = user.BenefitRate;
            userToUpdate.IsActive = user.IsActive;
            userToUpdate.Supervisor = user.Supervisor;
            userToUpdate.FundTypes = user.FundTypes;
            userToUpdate.Projects = user.Projects;
        }

        /// <summary>
        /// Checks on associated user info accoding to business rules
        /// </summary>
        /// <param name="user">The user instance to check</param>
        private void CheckUserAssociations(User user)
        {
            throw new NotImplementedException("This method is no longer needed because the validation is done with the users.cs");
            //if (user.Projects.Count == 0) 
            //    ModelState.AddModelError("User.Projects", "You must select at least one project");

            //if (user.FundTypes.Count == 0)
            //    ModelState.AddModelError("User.FundTypes", "You must select at least one fund type");
        }

        /// <summary>
        /// Bus. rules:  
        /// If the fundtype starts with State, the user much have the timesheet role.
        /// If the user has subordinates, they must be a supervisor
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="user"></param>
        private void EnsureProperRoles(ICollection<string> roles, User user)
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

            // If the user has subordinates, make sure they have supervisor role
            if (UserBLL.GetSubordinates(user).Count() > 0 && !roles.Contains(RoleNames.RoleSupervisor))
            {
                roles.Add(RoleNames.RoleSupervisor);
            }
        }
    }

    public class CreateUserViewModel : UserViewModel
    {
        /// <summary>
        /// Creates the user view model, including populating the lookups
        /// </summary>
        public new static CreateUserViewModel Create(IUserBLL userBLL, IRepository repository)
        {
            var baseViewModel = UserViewModel.Create(userBLL, repository);

            var viewModel = new CreateUserViewModel
                                {
                                    Supervisors = baseViewModel.Supervisors,
                                    Projects = baseViewModel.Projects,
                                    FundTypes = baseViewModel.FundTypes,
                                    AvailableRoles = baseViewModel.AvailableRoles
                                };

            return viewModel;
        }

        public void TransferValuesFrom(CreateUserViewModel model)
        {
            UserName = model.UserName;
            Email = model.Email;
            Question = model.Question;
            Answer = model.Answer;

            User = model.User;
        }

        [Required]
        [Length(1, 50, Message = "Must be between 1 and 50 characters long")]
        public string UserName { get; set; }

        [Required]
        [Length(1, 50, Message = "Must be between 1 and 50 characters long")]
        [Pattern(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase,
            Message = "Must be a valid email address")]
        public string Email { get; set; }

        [Required]
        [Length(1, 50, Message = "Must be between 1 and 50 characters long")]
        public string Question { get; set; }

        [Required]
        [Length(1, 50, Message = "Must be between 1 and 50 characters long")]
        public string Answer { get; set; }
    }

    public class UserViewModel
    {
        /// <summary>
        /// Creates the user view model, including populating the lookups
        /// </summary>
        public static UserViewModel Create(IUserBLL userBLL, IRepository repository)
        {
            var viewModel = new UserViewModel
                                {
                                    Supervisors = userBLL.GetSupervisors().OrderBy(a => a.LastName).ToList(),
                                    Projects = userBLL.GetAllProjectsByUser(repository.OfType<Project>()).OrderBy(a => a.Name).ToList(),
                                    FundTypes = userBLL.GetAvailableFundTypes(repository.OfType<FundType>()).OrderBy(a => a.Name).ToList(),
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
