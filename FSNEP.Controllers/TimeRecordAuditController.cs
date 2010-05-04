using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Controllers
{
    /// <summary>
    /// Viewable list of time records for admins which will be used for verifying electronic signatures
    /// </summary>
    [AdminOnly]
    public class TimeRecordAuditController : SuperController
    {
        public ActionResult History(int? projectId)
        {
            var viewModel = TimeRecordAuditHistoryViewModel.Create(Repository.OfType<Project>(),
                                                                  Repository.OfType<TimeRecord>(), Repository.OfType<User>(),
                                                                  projectId);

            return View(viewModel);
        }
    }

    /// <summary>
    /// Model for selecting time records and displaying them to the user on the History view
    /// </summary>
    public class TimeRecordAuditHistoryViewModel
    {
        public static TimeRecordAuditHistoryViewModel Create(IRepository<Project> projectRepository, IRepository<TimeRecord> recordRepository, IRepository<User> userRepository, int? projectId)
        {
            var chosenProject = projectId.HasValue ? projectRepository.GetNullableByID(projectId.Value) : null;

            var projects = projectRepository.Queryable.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();

            var viewModel = new TimeRecordAuditHistoryViewModel { Project = chosenProject, Projects = projects };

            if (chosenProject != null)
            {
                var availableTimeRecordUserIds = userRepository.Queryable.Where(x => x.Projects.Contains(chosenProject)).Select(x => x.Id).ToList();

                viewModel.Records =
                    recordRepository.Queryable.Where(x => availableTimeRecordUserIds.Contains(x.User.Id)).OrderByDescending(
                        x => x.Year).ThenByDescending(x => x.Month).ToList();
            }

            return viewModel;
        }

        public IEnumerable<TimeRecord> Records { get; set; }

        public IEnumerable<Project> Projects { get; set; }

        public Project Project { get; set; }
    }
}