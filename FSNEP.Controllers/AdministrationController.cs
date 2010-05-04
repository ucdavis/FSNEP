using System.Collections.Generic;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.Controllers.Helpers;
using FSNEP.Core.Domain;
using System.Linq;
using MvcContrib.Attributes;
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

        /// <summary>
        /// Returns the user object indentified by the given userid.  If there is no user, return just the other information needed for creating a new user.
        /// </summary>
        /// <param name="id">the userid/username</param>
        public ActionResult ModifyUser(string id)
        {
            var viewModel = new ModifyUserViewModel
                                {
                                    User = string.IsNullOrEmpty(id) ? new User() : UserBLL.GetUser(id)
                                };

            viewModel.Supervisors = new SelectList(UserBLL.GetSupervisors(), "ID", "FullName",
                                                   viewModel.User.Supervisor != null ? viewModel.User.Supervisor.ID : Guid.Empty);

            viewModel.Projects = new MultiSelectList(UserBLL.GetAllProjectsByUser(), "ID", "Name",
                                                     viewModel.User.Projects.Select(p => p.ID));

            viewModel.FundTypes = new MultiSelectList(UserBLL.GetAllProjectsByUser(), "ID", "Name",
                                                     viewModel.User.FundTypes.Select(p => p.ID));

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult ModifyUser(string id, User user, Guid? supervisorId, List<Project> projects)
        {
            user = new User();

            ValidationHelper<User>.Validate(user, ModelState, "User");

            
            //Look for errors
            if (!supervisorId.HasValue)
                ModelState.AddModelError("SupervisorID", "You must select a supervisor");

            return ModifyUser(id);
        }

    }

    public class ModifyUserViewModel
    {
        public User User { get; set; }
        public SelectList Supervisors { get; set; }
        public MultiSelectList Projects { get; set; }
        public MultiSelectList FundTypes { get; set; }

        public ModifyUserViewModel()
        {
            
        }
    }
}
