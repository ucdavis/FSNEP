using System.Web.Mvc;
using FSNEP.BLL.Dev;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.Utils;

namespace FSNEP.Controllers
{
    public class ReportController : SuperController
    {
        private readonly ITimeRecordBLL _timeRecordBLL;
        private readonly IReportBLL _reportBLL;

        public ReportController(ITimeRecordBLL timeRecordBLL, IReportBLL reportBLL)
        {
            _timeRecordBLL = timeRecordBLL;
            _reportBLL = reportBLL;
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

            var report = _reportBLL.GenerateIndividualTimeRecordReport(record, ReportType.PDF);

            return File(report.ReportContent, report.ContentType, "TimeRecord.pdf");
        }
    }
}