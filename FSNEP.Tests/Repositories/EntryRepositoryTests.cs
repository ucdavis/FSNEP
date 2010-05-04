using System;
using System.Linq;
using FSNEP.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class EntryRepositoryTests : RepositoryTestBase
    {
        private const int ValidYear = 2009;
        private const int ValidMonth = 6;

        [TestMethod]
        public void CanSaveValidEntry()
        {
            var entry = CreateValidEntry();

            Repository.OfType<Entry>().EnsurePersistent(entry);

            Assert.AreEqual(false, entry.IsTransient());
        }

        private Entry CreateValidEntry()
        {
            var entry = new Entry {Comment = "Valid", Record = Repository.OfType<Record>().Queryable.First()};

            return entry;
        }

        protected override void LoadData()
        {
            base.LoadData();

            using (var ts = new TransactionScope())
            {
                LoadStatus();
                LoadRecords();

                ts.CommitTransaction();
            }

            NHibernateSessionManager.Instance.GetSession().Flush();
        }

        private void LoadRecords()
        {
            var record = CreateValidRecord();

            Repository.OfType<Record>().EnsurePersistent(record);
        }

        private Record CreateValidRecord()
        {
            var record = new Record
            {
                Month = ValidMonth,
                Year = ValidYear,
                Status = Repository.OfType<Status>().Queryable.First(),
                User = Repository.OfType<User>().Queryable.First()
            };

            return record;
        }

        public void LoadStatus()
        {
            var status1 = new Status {Name = "S1"};
            var status2 = new Status { Name = "S2" };

            var statusRepository = Repository.OfType<Status>();

            statusRepository.EnsurePersistent(status1);
            statusRepository.EnsurePersistent(status2);
        }

    }
}