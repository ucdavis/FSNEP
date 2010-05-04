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
                                                                  Repository.OfType<CostShare>(), projectId);

            return View(viewModel);
        }
    }

    public class CostShareAuditHistoryViewModel
    {
        public static CostShareAuditHistoryViewModel Create(IRepository<Project> projectRepository, IRepository<CostShare> costShareRepository, int? projectId)
        {
            var chosenProject = projectId.HasValue ? projectRepository.GetNullableByID(projectId.Value) : null;

            var costSharesInProject =
                costShareRepository.Queryable.Where(x => x.User.Projects.Contains(chosenProject)).OrderByDescending(
                    x => x.Year).ThenByDescending(x => x.Month);
           
            return new CostShareAuditHistoryViewModel {CostShares = costSharesInProject, Project = chosenProject};
        }

        public IEnumerable<CostShare> CostShares { get; set; }

        public Project Project { get; set; }
    }
}