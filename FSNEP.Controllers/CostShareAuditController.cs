using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Controllers
{
    /// <summary>
    /// Allows the state office to audit cost share records
    /// </summary>
    [AdminOnly]
    public class CostShareAuditController : SuperController
    {
        public ActionResult History(int? projectId)
        {
            var viewModel = CostShareAuditHistoryViewModel.Create(Repository.OfType<Project>(),
                                                                  Repository.OfType<CostShare>(), Repository.OfType<User>(),
                                                                  projectId);

            return View(viewModel);
        }
    }

    public class CostShareAuditHistoryViewModel
    {
        public static CostShareAuditHistoryViewModel Create(IRepository<Project> projectRepository, IRepository<CostShare> recordRepository, IRepository<User> userRepository, int? projectId)
        {
            var chosenProject = projectId.HasValue ? projectRepository.GetNullableByID(projectId.Value) : null;

            var projects = projectRepository.Queryable.Where(x=>x.IsActive).OrderBy(x => x.Name).ToList();
           
            var viewModel = new CostShareAuditHistoryViewModel {Project = chosenProject, Projects = projects};

            if (chosenProject != null)
            {
                var availableCostShareUserIds = userRepository.Queryable.Where(x => x.Projects.Contains(chosenProject)).Select(x=>x.Id).ToList();

                viewModel.Records =
                    recordRepository.Queryable.Where(x => availableCostShareUserIds.Contains(x.User.Id)).OrderByDescending(
                        x => x.Year).ThenByDescending(x => x.Month).ToList();
            }

            return viewModel;
        }

        public IEnumerable<CostShare> Records { get; set; }

        public IEnumerable<Project> Projects { get; set; }

        public Project Project { get; set; }
    }
}