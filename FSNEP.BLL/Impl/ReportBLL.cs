using System;
using System.Collections.Specialized;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public class ReportBLL : IReportBLL
    {
        public ReportResult GenerateIndividualTimeRecordReport(TimeRecord timeRecord, ReportType reportType)
        {
            const string reportPath = "/FSNEP2/IndividualActivityRecord";

            var parameters = new ListDictionary();
            parameters["TimeRecordID"] = timeRecord.Id;

            return GetReport(reportPath, parameters, reportType);
        }

        public ReportResult GenerateCostShare(Project project, int year, ReportType reportType)
        {
            const string reportPath = "/FSNEP2/CostShare";

            var parameters = new ListDictionary();

            parameters["ProjectID"] = project.Id;
            parameters["year"] = year;

            return GetReport(reportPath, parameters, reportType);
        }

        public ReportResult GetReport(string reportPath, ListDictionary parameters, ReportType reportType)
        {
            var rview = new Microsoft.Reporting.WebForms.ReportViewer();

            rview.ServerReport.ReportServerUrl = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["ReportServer"]);

            rview.ServerReport.ReportPath = reportPath;

            var paramList = new System.Collections.Generic.List<Microsoft.Reporting.WebForms.ReportParameter>();

            foreach (string key in parameters.Keys)
            {
                paramList.Add(new Microsoft.Reporting.WebForms.ReportParameter(key, parameters[key] == null ? null : parameters[key].ToString()));
            }
            
            rview.ServerReport.SetParameters(paramList);

            string mimeType, encoding, extension;
            string[] streamids;
            Microsoft.Reporting.WebForms.Warning[] warnings;

            string format = "pdf"; //default to PDF

            if (reportType == ReportType.Excel) format = "excel";
            if (reportType == ReportType.Web) format = "HTML4.0";

            const string deviceInfo = "<DeviceInfo>" +
                                      "<SimplePageHeaders>True</SimplePageHeaders>" +
                                      "</DeviceInfo>";

            byte[] bytes = rview.ServerReport.Render(format, deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);

            var result = new ReportResult(bytes, mimeType);

            return result;
        }
    }

    public enum ReportType
    {
        PDF,
        Excel,
        Web
    }
}