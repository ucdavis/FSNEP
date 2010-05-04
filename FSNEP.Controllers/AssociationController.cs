using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAESArch.Core.DataInterfaces;
using FSNEP.Core.Domain;

namespace FSNEP.Controllers
{
    public class AssociationController : SuperController
    {
        public ActionResult Projects(int? id)
        {
            Project project = null;

            if (id.HasValue) project = Repository.OfType<Project>().GetNullableByID(id.Value);

            var viewModel = ProjectsAccountsViewModel.Create(Repository);
            viewModel.Project = project;

            return View(viewModel);
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