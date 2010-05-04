using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.Core.Domain;

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
            var viewModel = new ModifyUserViewModel();

            //First get the lookups, like projects, fundtypes, and supervisors  
            viewModel.Projects = new SelectList(UserBLL.GetAllProjectsByUser(), "ID", "Name");
            viewModel.FundTypes = new SelectList(UserBLL.GetAvailableFundTypes(), "Name", "ID");


            return View(viewModel);
        }

    }

    public class ModifyUserViewModel
    {
        public ModifyUserViewModel() : this(new User()) {}

        public ModifyUserViewModel(User user)
        {
            User = user;
        }

        public bool NewUser
        {
            get { return User.IsTransient(); }
        }

        public User User { get; set; }

        public SelectList Supervisors { get; set; }

        public SelectList Projects { get; set; }

        public SelectList FundTypes { get; set; }
    }
}
