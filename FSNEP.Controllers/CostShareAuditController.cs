using System.Linq;
using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;

namespace FSNEP.Controllers
{
    /// <summary>
    /// Allows the state office to audit cost share records
    /// </summary>
    [AdminOnly]
    public class CostShareAuditController : SuperController
    {
        public ActionResult ChooseProject()
        {
            var projects = Repository.OfType<Project>().Queryable.OrderBy(x => x.Name);

            return View(projects.ToList());
        }

        public ActionResult History(int? projectId)
        {
            if (projectId.HasValue == false)
            {
                return RedirectToAction("ChooseProject");
            }

            var chosenProject = Repository.OfType<Project>().GetById(projectId.Value);

            var costSharesInProject =
                Repository.OfType<CostShare>().Queryable.Where(x => x.User.Projects.Contains(chosenProject));

            return View(costSharesInProject.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month));
        }
    }
}