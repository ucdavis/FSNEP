using System;
using System.Collections.Specialized;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Dev
{
    public interface IReportBLL
    {
        ReportResult GenerateIndividualTimeRecordReport(TimeRecord timeRecord, ReportType reportType);
        ReportResult GenerateCostShare(Project project, int year, ReportType reportType);
    }

    public class ReportBLL : IReportBLL
    {
        public ReportResult GenerateIndividualTimeRecordReport(TimeRecord timeRecord, ReportType reportType)
        {
            const string reportPath = "/FSNEP.Report/IndividualActivityRecord";

            var parameters = new ListDictionary();
            parameters["TimeRecordID"] = timeRecord.Id;

            return GetReport(reportPath, parameters, reportType);
        }

        public ReportResult GenerateCostShare(Project project, int year, ReportType reportType)
        {
            const string reportPath = "/FSNEP.Report/CostShare";

            var parameters = new ListDictionary();

            parameters["projectID"] = project.Id;
            parameters["year"] = year;

            return GetReport(reportPath, parameters, reportType);
        }

        public ReportResult GetReport(string reportPath, ListDictionary parameters, ReportType reportType)
        {
            //TODO: Do the render and get back some byes and a content type

            var result = new ReportResult(new byte[1], "contentType");

            return result;

            /*
            Microsoft.Reporting.WebForms.ReportViewer rview = new Microsoft.Reporting.WebForms.ReportViewer();

            rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

            rview.ServerReport.ReportPath = reportPath; //"/FSNEP.Report/StateShareExpense";

            System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter> paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();

            foreach (string key in parameters.Keys)
            {
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter(key, parameters[key] == null ? null : parameters[key].ToString()));
            }

            //paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter("TimeSheetID", parameters["TimeSheetID"]));

            rview.ServerReport.SetParameters(paramList);

            string mimeType, encoding, extension, deviceInfo;
            string[] streamids;
            Microsoft.Reporting.WebForms.Warning[] warnings;

            string format = "pdf"; //default to PDF

            if (reportType == ReportType.Excel) format = "excel";

            deviceInfo =
                "<DeviceInfo>" +
                "<SimplePageHeaders>True</SimplePageHeaders>" +
                "</DeviceInfo>";

            byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

            HttpContext.Current.Response.Clear();

            HttpContext.Current.Response.ContentType = mimeType;
            HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}.{1}", outputFileName, extension));

            HttpContext.Current.Response.OutputStream.Write(bytes, 0, bytes.Length);
            HttpContext.Current.Response.OutputStream.Flush();
            HttpContext.Current.Response.OutputStream.Close();
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.Close();
            */
        }
    }

    public enum ReportType
    {
        PDF,
        Excel
    }
}