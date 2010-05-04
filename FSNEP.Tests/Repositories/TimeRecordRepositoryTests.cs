using System.Linq;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

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

        protected override void LoadData()
        {
            using (var ts = new TransactionScope())
            {
                LoadStatus();

                ts.CommitTransaction();
            }

            base.LoadData();

            NHibernateSessionManager.Instance.GetSession().Flush();
        }

        public void LoadStatus()
        {
            var status1 = new Status { Name = "S1" };
            var status2 = new Status { Name = "S2" };

            var statusRepository = Repository.OfType<Status>();

            statusRepository.EnsurePersistent(status1);
            statusRepository.EnsurePersistent(status2);
        }
    }
}