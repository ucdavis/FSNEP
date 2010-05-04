using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.Core.Domain;
using System.Linq;

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
            //First get the lookups, like projects, fundtypes, and supervisors  
            var viewModel = new ModifyUserViewModel();

            if (string.IsNullOrEmpty(id))
            {
                return View(viewModel);
            }

            viewModel.User = UserBLL.GetUser(id);

            //Populate the supervisor with the correct supervisor chosen
            viewModel.Supervisors = new SelectList(UserBLL.GetSupervisors(), "ID", "FullName",
                                                   viewModel.User.Supervisor.ID);

            viewModel.Projects = new MultiSelectList(UserBLL.GetAllProjectsByUser(), "ID", "Name",
                                                     viewModel.User.Projects.Select(p => p.ID));

            viewModel.FundTypes = new MultiSelectList(UserBLL.GetAllProjectsByUser(), "ID", "Name",
                                                     viewModel.User.FundTypes.Select(p => p.ID));

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

        public MultiSelectList Projects { get; set; }

        public MultiSelectList FundTypes { get; set; }
    }
}
