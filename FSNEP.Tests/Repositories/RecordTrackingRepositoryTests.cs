using System;
using System.Linq;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class RecordTrackingRepositoryTests : RepositoryTestBase
    {
        [TestMethod]
        public void CanSaveValidRecordTrackingEntry()
        {
            var tracking = new RecordTracking
                               {
                                   ActionDate = DateTime.Now,
                                   UserName = ValidUserName,
                                   Record = Repository.OfType<TimeRecord>().Queryable.First(),
                                   Status = Repository.OfType<Status>().Queryable.First()
                               };

            Repository.OfType<RecordTracking>().EnsurePersistent(tracking);

            Assert.AreEqual(false, tracking.IsTransient());
        }

        protected override void LoadData()
        {
            base.LoadData();

            LoadTimeRecords();
        }

        private void LoadTimeRecords()
        {
            var record = new TimeRecord
            {
                Month = ValidMonth,
                Year = ValidYear,
                Salary = 200,
                Status = Repository.OfType<Status>().Queryable.First(),
                User = Repository.OfType<User>().Queryable.First()
            };

            Repository.OfType<TimeRecord>().EnsurePersistent(record);
        }

        private const int ValidYear = 2009;
        private const int ValidMonth = 6;
        private const string ValidUserName = "ValidUser";
    }
}