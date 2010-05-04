﻿using System.Collections.Generic;
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
            var viewModel = CreateUserViewModel.Create(UserBLL);
            viewModel.User = new User();

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult Create(CreateUserViewModel model, Guid? supervisorId, IEnumerable<int> projectList,
                                       IEnumerable<int> fundTypeList, List<string> roleList)
        {
            throw new NotImplementedException();

            /*
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
             */
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
        public ActionResult Modify(User user, List<string> roleList, string id)
        {
            var userToUpdate = UserBLL.GetUser(id);

            TransferValuesTo(userToUpdate, user);

            ValidationHelper<User>.Validate(userToUpdate, ModelState);

            CheckUserAssociations(userToUpdate);

            if (roleList == null || roleList.Count == 0)
                ModelState.AddModelError("RoleList", "User must have at least one role");

            if (ModelState.IsValid)
            {
                //Do the save
                EnsureProperRoles(roleList, userToUpdate);

                //Now reconcile the user's roles
                UserBLL.SetRoles(id, roleList);

                //We have a valid viewstate, so save the changes
                using (var ts = new TransactionScope())
                {
                    UserBLL.EnsurePersistent(userToUpdate);

                    ts.CommitTransaction();
                }

                Message = string.Format("{0} modified successfully", id);

                return this.RedirectToAction(a => a.List());
             
            }
            else //Not valid -- repopulate the viewmodel and send the user back to make the corrections
            {
                var viewModel = UserViewModel.Create(UserBLL);
                viewModel.User = userToUpdate;

                return View(viewModel);
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
            if (user.Projects.Count == 0) 
                ModelState.AddModelError("User.Projects", "You must select at least one project");

            if (user.FundTypes.Count == 0)
                ModelState.AddModelError("User.FundTypes", "You must select at least one fund type");
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
        public new static CreateUserViewModel Create(IUserBLL userBLL)
        {
            var baseViewModel = UserViewModel.Create(userBLL);

            var viewModel = new CreateUserViewModel
                                {
                                    Supervisors = baseViewModel.Supervisors,
                                    Projects = baseViewModel.Projects,
                                    FundTypes = baseViewModel.FundTypes,
                                    AvailableRoles = baseViewModel.AvailableRoles
                                };

            return viewModel;
        }

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