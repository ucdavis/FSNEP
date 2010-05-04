using System.Web.Mvc;
using FSNEP.Core.Abstractions;

namespace FSNEP.Controllers.Helpers.Extensions
{
    public static class ActionResultExtensions
    {
        public static FileResult ToFileResult(this ReportResult reportResult, string fileName)
        {
            var result = new FileContentResult(reportResult.ReportContent, reportResult.ContentType) { FileDownloadName = fileName };

            return result;
        }
    }
}