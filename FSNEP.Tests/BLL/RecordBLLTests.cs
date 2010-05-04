using FSNEP.BLL.Dev;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using Rhino.Mocks;

namespace FSNEP.Tests.BLL
{
    [TestClass]
    public class RecordBLLTests
    {
        private IRecordBLL<Record> _recordBLL;
        private IRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = MockRepository.GenerateStub<IRepository>();
            _recordBLL = new RecordBLL<Record>(_repository);
        }

        [TestMethod]
        public void IsEditableReturnsTrueIfStatusIsCurrent()
        {
            var status = new Status {Name = Status.Option.Current.ToString()};

            var record = new Record {Status = status};

            var editable = _recordBLL.IsEditable(record);

            Assert.AreEqual(true, editable);
        }

        [TestMethod]
        public void IsEditableReturnsTrueIfStatusIsDisapproved()
        {
            var status = new Status { Name = Status.Option.Disapproved.ToString() };

            var record = new Record { Status = status };

            var editable = _recordBLL.IsEditable(record);

            Assert.AreEqual(true, editable);
        }

        [TestMethod]
        public void IsEditableReturnsFalseIfStatusIsApproved()
        {
            var status = new Status { Name = Status.Option.Approved.ToString() };

            var record = new Record { Status = status };

            var editable = _recordBLL.IsEditable(record);

            Assert.AreEqual(false, editable);
        }

        [TestMethod]
        public void IsEditableReturnsPendingReviewIfStatusIsPendingReview()
        {
            var status = new Status { Name = Status.Option.PendingReview.ToString() };

            var record = new Record { Status = status };

            var editable = _recordBLL.IsEditable(record);

            Assert.AreEqual(false, editable);
        }
    }
}