using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.BLL
{
    [TestClass]
    public class ReportBLLTests
    {
        private IReportBLL _reportBLL;

        [TestInitialize]
        public void Setup()
        {
            _reportBLL = new ReportBLL();
        }

        [TestMethod]
        public void GenerateIndividualTimeRecordReportGeneratesReport()
        {
            var timeRecord = CreateValidEntities.TimeRecord(null);

            var result = _reportBLL.GenerateIndividualTimeRecordReport(timeRecord, ReportType.PDF);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ReportResult));
            Assert.AreEqual("contentType", result.ContentType);
            Assert.AreEqual(new byte[1].ToString(), result.ReportContent.ToString());
        }

        [TestMethod]
        public void GenerateCostShareReturnsReportResult()
        {
            var result = _reportBLL.GenerateCostShare(CreateValidEntities.Project(null), 2009, ReportType.Excel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ReportResult));
            Assert.AreEqual("contentType", result.ContentType);
            Assert.AreEqual(new byte[1].ToString(), result.ReportContent.ToString());
        }
    }
}
