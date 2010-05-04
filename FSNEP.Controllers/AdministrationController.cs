using System.Collections.Generic;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.Controllers.Helpers;
using FSNEP.Core.Domain;
using System.Linq;
using MvcContrib.Attributes;
using System;
using MvcContrib.UI.Html;

namespace FSNEP.Controllers
{
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
            User user = string.IsNullOrEmpty(id) ? new User() : UserBLL.GetUser(id);
            

            //Populate the supervisor with the correct supervisor chosen
            ViewData["Supervisors"] = new SelectList(UserBLL.GetSupervisors(), "ID", "FullName",
                                                   user.Supervisor.ID);

            ViewData["Projects"] = new MultiSelectList(UserBLL.GetAllProjectsByUser(), "ID", "Name",
                                                     user.Projects.Select(p => p.ID));

            ViewData["FundTypes"] = new MultiSelectList(UserBLL.GetAllProjectsByUser(), "ID", "Name",
                                                     user.FundTypes.Select(p => p.ID));

            return View(user);
        }

        [AcceptPost]
        public ActionResult ModifyUser(string id, User user, Guid? supervisorId, List<Project> projects)
        {
            user = new User();

            ValidationHelper<User>.Validate(user, ModelState);

            
            //Look for errors
            if (!supervisorId.HasValue)
                ModelState.AddModelError("SupervisorID", "You must select a supervisor");

            return ModifyUser(id);
        }

    }
}
