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

            viewModel.Projects = new MultiSelectList(UserBLL.GetAllProjectsByUser().ToList(), "ID", "Name",
                                                     viewModel.User.Projects.Select(p => p.ID));

            viewModel.FundTypes = new MultiSelectList(UserBLL.GetAvailableFundTypes().ToList(), "ID", "Name",
                                                     viewModel.User.FundTypes.Select(p => p.ID));

            return View(viewModel);
        }

        [AcceptPost]
        public ActionResult ModifyUser(string id, Guid? supervisorId, int[] projectList, int[] fundTypeList)
        {
            var user = string.IsNullOrEmpty(id) ? new User() : UserBLL.GetUser(id);

            UpdateModel(user, "User"); //Update the user from the data entered in the form
            
            ValidationHelper<User>.Validate(user, ModelState, "User");
            
            //Make sure we get a supervisor, some projects and some fundtypes
            if (!supervisorId.HasValue) ModelState.AddModelError("SupervisorID", "You must select a supervisor");

            if (projectList == null) ModelState.AddModelError("ProjectList", "You must select at least one project");

            if (fundTypeList == null) ModelState.AddModelError("FundTypeList", "You must select at least one fund type");

            Response.Write("Model Is Valid? " + ModelState.IsValid);

            return ModifyUser(id);
        }
    }

    public class ModifyUserViewModel
    {
        public User User { get; set; }
        public SelectList Supervisors { get; set; }
        public MultiSelectList Projects { get; set; }
        public MultiSelectList FundTypes { get; set; }
    }
}
