using System.Collections.Generic;
using System.Web.Mvc;
using CAESArch.BLL;
using FSNEP.BLL.Impl;
using FSNEP.Controllers.Helpers;
using FSNEP.Core.Domain;
using System.Linq;
using MvcContrib.Attributes;
using MvcContrib;
using System;

namespace FSNEP.Controllers
{
    [Authorize]
    public class AdministrationController : SuperController
    {
        public IUserBLL UserBLL;
        
        public AdministrationController(IUserBLL userBLL)
        {
            UserBLL = userBLL;
        }

        public ActionResult CreateUser()
        {
            //Create the viewmodel with a blank user
            var viewModel = new CreateUserViewModel { User = new User() };

            PopulateDefaultUserViewModel(viewModel);

            return View(viewModel);
        }

        /// <summary>
        /// Returns the user object indentified by the given userid.  If there is no user, return just the other information needed for creating a new user.
        /// </summary>
        /// <param name="id">the userid/username</param>
        public ActionResult ModifyUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.RedirectToAction(a => a.CreateUser());
            }

            var viewModel = new UserViewModel { User = UserBLL.GetUser(id) };

            //If the user could not be found, redirect to creating a user
            if (viewModel.User == null) return this.RedirectToAction(a => a.CreateUser());

            PopulateDefaultUserViewModel(viewModel);

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult ModifyUser(string id, Guid? supervisorId, IEnumerable<int> projectList, IEnumerable<int> fundTypeList)
        {
            var user = UserBLL.GetUser(id);
            UpdateModel(user, "User"); //Update the user from the data entered in the form
            
            ValidationHelper<User>.Validate(user, ModelState, "User");
            
            CheckUserProperties(supervisorId, projectList, fundTypeList);

            if (!ModelState.IsValid)
            {
                return ModifyUser(id);
            }

            PopulateUserProperties(user, supervisorId, projectList, fundTypeList);

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

            if (fundTypeList == null) ModelState.AddModelError("FundTypeList", "You must select at least one fund type");
        }

        /// <summary>
        /// Populate the given user with the proper associated properties
        /// </summary>
        private void PopulateUserProperties(User user, Guid? supervisorId, IEnumerable<int> projectList, IEnumerable<int> fundTypeList)
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
                                                  viewModel.User.Supervisor != null ? viewModel.User.Supervisor.ID : Guid.Empty);

            viewModel.Projects = new MultiSelectList(UserBLL.GetAllProjectsByUser().ToList(), "ID", "Name",
                                                     viewModel.User.Projects.Select(p => p.ID));

            viewModel.FundTypes = new MultiSelectList(UserBLL.GetAvailableFundTypes().ToList(), "ID", "Name",
                                                     viewModel.User.FundTypes.Select(p => p.ID));

        }
    }

    public class CreateUserViewModel : UserViewModel
    {

    }

    public class UserViewModel
    {
        public User User { get; set; }
        public SelectList Supervisors { get; set; }
        public MultiSelectList Projects { get; set; }
        public MultiSelectList FundTypes { get; set; }
    }
}
