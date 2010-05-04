using System.Linq;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class TimeRecordEntryRepositoryTests : RepositoryTestBase
    {
        [TestMethod]
        public void CanSaveValidTimeRecordEntry()
        {
            var entry = new TimeRecordEntry
                            {
                                Date = ValidDate,
                                Hours = ValidHours,
                                Comment = ValidComment,
                                Record = Repository.OfType<TimeRecord>().Queryable.First()
                            };

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(entry);

            Assert.AreEqual(false, entry.IsTransient());
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
        private const int ValidDate = 25;
        private const string ValidComment = "Comment";
        private const double ValidHours = 6.5;
    }
}