using System.Linq;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class TimeRecordRepositoryTests : RepositoryTestBase
    {
        private const int ValidYear = 2009;
        private const int ValidMonth = 6;

        [TestMethod]
        public void CanSaveValidTimeRecord()
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

            Assert.AreEqual(false, record.IsTransient());
        }
    }
}