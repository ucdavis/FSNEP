using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Dev;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers.Helpers.Extensions;
using FSNEP.Core.Domain;
using UCDArch.Core.Utils;
using MvcContrib.Attributes;
using FSNEP.Core.Abstractions;

namespace FSNEP.Controllers
{
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
        public ActionResult PrintTimeRecord(int id)
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
        /// Display the cost share report form
        /// </summary>
        /// <returns></returns>
        public ActionResult CostShare()
        {
            var projects = _userBLL.GetAllProjectsByUser(Repository.OfType<Project>()).ToList();

            return View(projects);
        }

        [AcceptPost]
        public ActionResult CostShare(Project project, int year)
        {
            var user = _userBLL.GetUser();

            //Make sure the project is valid for this user
            if(user.Projects.Contains(project) == false)
            {
                return new HttpUnauthorizedResult();
            }

            return _reportBLL.GenerateCostShare(project, year, ReportType.Excel).ToFileResult("CostShare.xls");
        }
    }
}