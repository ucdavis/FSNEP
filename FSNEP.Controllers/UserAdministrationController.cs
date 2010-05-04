﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CAESArch.BLL;
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
                    UserBLL.Repository.Remove(user);

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
                ModelState.AddModelError("UserName", "Username already exists");
                return Create();
            }

            user.SetUserID((Guid)membershipUser.ProviderUserKey);

            user.Token = Guid.NewGuid(); //setup the new user token

            var ts = new TransactionScope();

            try
            {
                //save the user
                UserBLL.Repository.EnsurePersistent(user);

                //Send the user a message
                var newUserTokenPath = Url.AbsoluteAction("Index", "Home", new { token = user.Token });
                var supervisorEmail = UserBLL.UserAuth.MembershipService.GetUser(user.Supervisor.ID).Email;

                MessageGateway.SendMessageToNewUser(user, model.UserName, model.Email, supervisorEmail, newUserTokenPath);

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

            var viewModel = new UserViewModel { User = UserBLL.GetUser(id) };

            //If the user could not be found, redirect to creating a user
            if (viewModel.User == null) return this.RedirectToAction(a => a.Create());

            PopulateDefaultUserViewModel(viewModel);

            //Now the user roles are the roles for the given id
            viewModel.UserRoles = UserBLL.GetUserRoles(id);

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult Modify(string id, Guid? supervisorId, IEnumerable<int> projectList,
                                       IEnumerable<int> fundTypeList, List<string> roleList)
        {
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
                UserBLL.Repository.EnsurePersistent(user);

                ts.CommitTransaction();
            }

            return this.RedirectToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Check the associated user properties for validity
        /// </summary>
        private void CheckUserProperties(Guid? supervisorId, IEnumerable<int> projectList, IEnumerable<int> fundTypeList)
        {
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
            user.Supervisor = UserBLL.Repository.GetByID(supervisorId.Value);

            var projects = from proj in UserBLL.Repository.EntitySet<Project>()
                           where projectList.Contains(proj.ID)
                           select proj;

            var fundtypes = from ft in UserBLL.Repository.EntitySet<FundType>()
                            where fundTypeList.Contains(ft.ID)
                            select ft;

            user.Projects = projects.ToList();
            user.FundTypes = fundtypes.ToList();
        }

        private void PopulateDefaultUserViewModel(UserViewModel viewModel)
        {
            viewModel.Supervisors = new SelectList(UserBLL.GetSupervisors(), "ID", "FullName",
                                                   viewModel.User.Supervisor != null
                                                       ? viewModel.User.Supervisor.ID
                                                       : Guid.Empty);

            viewModel.Projects = new MultiSelectList(UserBLL.GetAllProjectsByUser().ToList(), "ID", "Name",
                                                     viewModel.User.Projects.Select(p => p.ID));

            viewModel.FundTypes = new MultiSelectList(UserBLL.GetAvailableFundTypes().ToList(), "ID", "Name",
                                                      viewModel.User.FundTypes.Select(p => p.ID));

            viewModel.AvailableRoles = UserBLL.GetAllRoles();
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
        [NotNullValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        public string UserName { get; set; }

        [NotNullValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        [RegexValidator(@"\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b", RegexOptions.IgnoreCase,
            MessageTemplate = "Must be a valid email address")]
        public string Email { get; set; }

        [NotNullValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        public string Question { get; set; }

        [NotNullValidator]
        [StringLengthValidator(1, 50, MessageTemplate = "Must be between {3} and {5} characters long")]
        public string Answer { get; set; }
    }

    public class UserViewModel
    {
        public User User { get; set; }
        public SelectList Supervisors { get; set; }
        public MultiSelectList Projects { get; set; }
        public MultiSelectList FundTypes { get; set; }

        public IEnumerable<string> AvailableRoles { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}