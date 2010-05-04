using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Helpers;

namespace FSNEP.Controllers
{
    /// <summary>
    /// Processes admin-level audit actions for cost share and time records
    /// </summary>
    [AdminOnly]
    public class AuditController : SuperController
    {
        public ActionResult TimeRecordHistory(int? projectId)
        {
            var viewModel = AuditHistoryViewModel<TimeRecord>.Create(Repository.OfType<Project>(),
                                                                  Repository.OfType<TimeRecord>(), Repository.OfType<User>(),
                                                                  projectId);

            return View(viewModel);
        }

        public ActionResult CostShareHistory(int? projectId)
        {
            var viewModel = AuditHistoryViewModel<CostShare>.Create(Repository.OfType<Project>(),
                                                                  Repository.OfType<CostShare>(), Repository.OfType<User>(),
                                                                  projectId);

            return View(viewModel);
        }

        public ActionResult CostShareReview(int id)
        {
            var costShare = Repository.OfType<CostShare>().GetNullableByID(id);

            Check.Require(costShare != null);

            var viewModel = CostShareAuditReviewViewModel.Create(Repository.OfType<CostShareEntry>(), costShare);

            return View(viewModel);
        }

        [AcceptPost]
        public JsonResult CostShareExclude(int id, string excludeReason)
        {
            var costShareEntryRepository = Repository.OfType<CostShareEntry>();

            var costShareEntry = costShareEntryRepository.GetNullableByID(id);

            Check.Require(costShareEntry != null, "Invalid Entry Identifier");

            //Set the entry to be excluded and include the reason
            costShareEntry.Exclude = true;
            costShareEntry.ExcludeReason = excludeReason;

            if (costShareEntry.ExcludeReason == null || costShareEntry.ExcludeReason.Trim() == string.Empty)
                ModelState.AddModelError("ExcludeReason", "Exclude Reason Is Required");

            costShareEntry.TransferValidationMessagesTo(ModelState);

            Check.Require(ModelState.IsValid, "Exclude Reason Is Not Valid");

            costShareEntryRepository.EnsurePersistent(costShareEntry);

            return Json(new { Success = true, EntryId = costShareEntry.Id });
        }
    }

    // <summary>
    /// Model for selecting time records and displaying them to the user on the History view
    /// </summary>
    public class AuditHistoryViewModel<T> where T: Record
    {
        public static AuditHistoryViewModel<T> Create(IRepository<Project> projectRepository, IRepository<T> recordRepository, IRepository<User> userRepository, int? projectId)
        {
            var chosenProject = projectId.HasValue ? projectRepository.GetNullableByID(projectId.Value) : null;

            var projects = projectRepository.Queryable.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();

            var viewModel = new AuditHistoryViewModel<T> { Project = chosenProject, Projects = projects };

            if (chosenProject != null)
            {
                var availableCostShareUserIds = userRepository.Queryable.Where(x => x.Projects.Contains(chosenProject)).Select(x => x.Id).ToList();

                viewModel.Records =
                    recordRepository.Queryable.Where(x => availableCostShareUserIds.Contains(x.User.Id)).OrderByDescending(
                        x => x.Year).ThenByDescending(x => x.Month).ToList();
            }

            return viewModel;
        }

        public IEnumerable<T> Records { get; set; }

        public IEnumerable<Project> Projects { get; set; }

        public Project Project { get; set; }
    }

    public class CostShareAuditReviewViewModel
    {
        public static CostShareAuditReviewViewModel Create(IRepository<CostShareEntry> costShareEntryRepository, CostShare costShare)
        {
            var viewModel = new CostShareAuditReviewViewModel { CostShare = costShare };

            var costShareEntries = costShareEntryRepository.Queryable.Where(x => x.Record.Id == costShare.Id);

            viewModel.Entries = costShareEntries.ToList();
            viewModel.IsAccepted = viewModel.CostShare.Status.NameOption == Status.Option.Approved;

            return viewModel;
        }

        public IEnumerable<CostShareEntry> Entries { get; set; }

        public CostShare CostShare { get; set; }

        public bool IsAccepted { get; set; }
    }
}