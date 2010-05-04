using FSNEP.BLL.Impl;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Interfaces
{
    public interface IReportBLL
    {
        ReportResult GenerateIndividualTimeRecordReport(TimeRecord timeRecord, ReportType reportType);
        ReportResult GenerateCostShare(Project project, int year, ReportType reportType);
    }
}