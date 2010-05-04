using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAESArch.Core.DataInterfaces;
using FSNEP.Core.Domain;
using CAESArch.Core.Utils;
using FSNEP.Controllers.Helpers.Attributes;

namespace FSNEP.Controllers
{
    public class AssociationController : SuperController
    {
        [Transaction]
        public ActionResult Projects(int? id)
        {
            Project project = null;

            if (id.HasValue) project = Repository.OfType<Project>().GetNullableByID(id.Value);

            var viewModel = ProjectsAccountsViewModel.Create(Repository);
            viewModel.Project = project;

            Message = "Message";

            return View(viewModel);
        }

        /// <summary>
        /// Associate the project identified by id with the accounts given 
        /// </summary>
        /// <param name="id">ProjectId</param>
        /// <param name="accountIds">AccountIds to associate with the given project</param>
        /// <returns>Redirection back to projects for additional associations</returns>
        [Transaction]
        public ActionResult Associate(int id, int[] accountIds)
        {
            var project = Repository.OfType<Project>().GetByID(id);

            Check.Require(project != null, "Valid ProjectId not passed into Associate action");
            
            project.Accounts.Clear();

            foreach (var accountID in accountIds)
            {
                project.Accounts.Add(Repository.OfType<Account>().GetByID(accountID));
            }

            Repository.OfType<Project>().EnsurePersistent(project);

            Message = "Accounts successfully associated";

            return RedirectToAction("Projects");
        }
    }

    public class ProjectsAccountsViewModel
    {
        public static ProjectsAccountsViewModel Create(IRepository repository)
        {
            var viewModel = new ProjectsAccountsViewModel
                                {
                                    Projects =
                                        repository.OfType<Project>().Queryable.Where(p => p.IsActive).OrderBy(
                                        p => p.Name).ToList(),
                                    Accounts =
                                        repository.OfType<Account>().Queryable.Where(a => a.IsActive).OrderBy(
                                        a => a.Name).ToList()
                                };

            return viewModel;
        }

        public List<Project> Projects { get; set; }
        public List<Account> Accounts { get; set; }
        public Project Project { get; set; }
    }
}