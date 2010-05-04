using System;
using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers.Helpers.Extensions;
using FSNEP.Core.Domain;
using UCDArch.Core.Utils;
using MvcContrib.Attributes;
using UCDArch.Web.ActionResults;

namespace FSNEP.Controllers
{
    [Authorize]
    public class ReportController : SuperController
    {
        private readonly ITimeRecordBLL _timeRecordBLL;
        private readonly IReportBLL _reportBLL;
        private readonly IUserBLL _userBLL;

        public ReportController(IReportBLL reportBLL, IUserBLL userBLL, ITimeRecordBLL timeRecordBLL)
        {
            _timeRecordBLL = timeRecordBLL;
            _reportBLL = reportBLL;
            _userBLL = userBLL;
        }

        /// <summary>
        /// Print out the time record for the given time record id
        /// </summary>
        /// <remarks>
        /// </remarks>
        public ActionResult PrintOwnTimeRecord(int id)
        {
            var record = Repository.OfType<TimeRecord>().GetNullableByID(id);

            Check.Require(record != null, "Record not found");

            if (_timeRecordBLL.HasAccess(CurrentUser, record) == false)
            {
                return new HttpUnauthorizedResult(); //User is unauthorized unless they have access to print the current record
            }

            return _reportBLL.GenerateIndividualTimeRecordReport(record, ReportType.PDF).ToFileResult("TimeRecord.pdf");
        }

        /// <summary>
        /// Print out the time record for the given time record id
        /// </summary>
        public ActionResult DisplayViewableTimeRecord(int id)
        {
            var record = Repository.OfType<TimeRecord>().GetNullableByID(id);

            Check.Require(record != null, "Record not found");

            var viewableUsers = _userBLL.GetAllViewableUsers();

            var canView = viewableUsers.Contains(record.User);

            if (!canView)
            {
                return new HttpUnauthorizedResult();
            }

            return _reportBLL.GenerateIndividualTimeRecordReport(record, ReportType.Web).ToFileResult();
        }

        /// <summary>
        /// Chose a time record to print
        /// </summary>
        public ActionResult TimeRecord()
        {
            var users = _userBLL.GetAllViewableUsers().ToList();

            return View(users);
        }

        [AcceptPost]
        public ActionResult TimeRecord(int recordId)
        {
            return DisplayViewableTimeRecord(recordId);
        }

        public ActionResult GetRecordForUser(Guid? val)
        {
            if (val.HasValue == false) return new JsonNetResult(null);

            var userId = val.Value;

            var records = Repository.OfType<TimeRecord>().Queryable.Where(x => x.User.Id == userId).ToList();

            var keyValuePair = records.Select(x => new {value = x.Id, text = x.Date.ToString("MMMM yyyy")});

            return new JsonNetResult(keyValuePair);
        } 

        /// <summary>
        /// Display the cost share report form
        /// </summary>
        /// <returns></returns>
        public ActionResult CostShare()
        {
            var projects = _userBLL.GetAllProjectsByUser(Repository.OfType<Project>()).OrderBy(x=>x.Name).ToList();

            return View(projects);
        }

        [AcceptPost]
        public ActionResult CostShare(int projectId, int year)
        {
            var project = Repository.OfType<Project>().GetNullableByID(projectId);

            Check.Require(project != null);

            var projects = _userBLL.GetAllProjectsByUser(Repository.OfType<Project>());

            //Make sure given project in the list of all projects for this user
            if (projects.Where(x => x.Id == project.Id).Count() == 0)
            {
                return new HttpUnauthorizedResult();
            }
            
            return _reportBLL.GenerateCostShare(project, year, ReportType.Excel).ToFileResult(string.Format("{0}CostShareReport.xls", year));
        }
    }
}