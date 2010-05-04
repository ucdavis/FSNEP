using System.Web.Mvc;
using FSNEP.Core.Abstractions;

namespace FSNEP.Controllers.Helpers.Extensions
{
    public static class ActionResultExtensions
    {
        /// <summary>
        /// Return the result as file content with the given filename (will present as file open/save dialog)
        /// </summary>
        public static FileResult ToFileResult(this ReportResult reportResult, string fileName)
        {
            var result = reportResult.ToFileResult();

            result.FileDownloadName = fileName;

            return result;
        }

        /// <summary>
        /// Return the result as file content
        /// </summary>
        /// <returns></returns>
        public static FileResult ToFileResult(this ReportResult reportResult)
        {
            var result = new FileContentResult(reportResult.ReportContent, reportResult.ContentType);

            return result;
        }
    }
}