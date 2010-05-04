using System.Linq;
using System.Web.Mvc;
using CAESArch.BLL;
using FSNEP.BLL.Impl;
using FSNEP.Controllers.Helpers;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;

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
        [Transaction]
        public ActionResult Projects()
        {
            var activeProjects = ProjectBLL.Repository.Queryable.Where(a => a.IsActive);

            return View(activeProjects);
        }

        [AcceptPost]
        public ActionResult InactivateProject(int projectId)
        {
            //get the project
            var project = ProjectBLL.Repository.GetNullableByID(projectId);

            if (project == null)
            {
                Message = "Project Not Found";

                return this.RedirectToAction(a => a.Projects());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                project.IsActive = false;

                ProjectBLL.Repository.EnsurePersistent(project);

                ts.CommitTransaction();
            }

            Message = "Project Removed Successfully";

            return this.RedirectToAction(a => a.Projects());
        }

        [AcceptPost]
        public ActionResult CreateProject(string newProjectName)
        {
            var newProject = new Project {IsActive = true, Name = newProjectName};

            ValidationHelper<Project>.Validate(newProject, ModelState);

            if (!ModelState.IsValid)
            {
                Message = "Project Creation Failed";

                return this.RedirectToAction(a => a.Projects());
            }

            //Add the new project
            using (var ts = new TransactionScope())
            {
                ProjectBLL.Repository.EnsurePersistent(newProject);

                ts.CommitTransaction();
            }

            Message = "Project Created Successfully";

            return this.RedirectToAction(a => a.Projects());
        }
    }
}