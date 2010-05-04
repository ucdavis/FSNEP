using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Impl;

namespace FSNEP.Controllers
{
    public class LookupController : SuperController
    {
        public IProjectBLL ProjectBLL;

        public LookupController(IProjectBLL projectBLL)
        {
            ProjectBLL = projectBLL;
        }
        
        /// <summary>
        /// Return a list of all projects
        /// </summary>
        public ActionResult ProjectsList()
        {
            var activeProjects = ProjectBLL.Repository.Queryable.Where(a => a.IsActive);

            return View(activeProjects);
        }
    }
}