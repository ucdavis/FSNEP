using System;
using System.Linq;
using FSNEP.Tests.Core;
using FSNEP.Tests.Core.Helpers;
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

        [TestMethod]
        public void CanSaveValidEntryThenRetrieveInParentRecord()
        {
            var recordRepository = Repository.OfType<Record>();

            const int recordId = 1;
            const string entryComment = "CanSaveValidEntryThenRetrieveInParentRecord test";

            var record = recordRepository.GetById(recordId);
            
            var entry = CreateValidEntry();
            entry.Comment = entryComment;

            record.AddEntry(entry);

            recordRepository.EnsurePersistent(record); //save the record and cascade save the entry

            NHibernateSessionManager.Instance.GetSession().Flush();

            //We saved an entry to the record id=recordId
            var newRecord = recordRepository.GetById(recordId);

            Assert.AreEqual(1, newRecord.Entries.Count);
            Assert.AreEqual(entryComment, newRecord.Entries[0].Comment);
        }

        private Entry CreateValidEntry()
        {
            var entry = CreateValidEntities.Entry(null);
            entry.Record = Repository.OfType<Record>().Queryable.First();
            entry.FundType = Repository.OfType<FundType>().Queryable.First();
            entry.Project = Repository.OfType<Project>().Queryable.First();
            entry.Account = Repository.OfType<Account>().Queryable.First();
            //var entry = new Entry
            //                {
            //                    Comment = "Valid",
            //                    Record = Repository.OfType<Record>().Queryable.First(),
            //                    FundType = Repository.OfType<FundType>().Queryable.First(),
            //                    Project = Repository.OfType<Project>().Queryable.First(),
            //                    Account = Repository.OfType<Account>().Queryable.First()
            //                };

            return entry;
        }

        #region Init

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

        #endregion Init
        private Record CreateValidRecord()
        {
            //var record = new Record
            //{
            //    Month = ValidMonth,
            //    Year = ValidYear,
            //    Status = Repository.OfType<Status>().Queryable.First(),
            //    User = Repository.OfType<User>().Queryable.First()
            //};
            var record = CreateValidEntities.Record(null);
            record.Status = Repository.OfType<Status>().Queryable.First();
            record.User = Repository.OfType<User>().Queryable.First();

            return record;
        }

        public void LoadStatus()
        {
            var status1 = new Status {NameOption = Status.Option.Approved};
            var status2 = new Status { NameOption = Status.Option.Current };

            var statusRepository = Repository.OfType<Status>();

            statusRepository.EnsurePersistent(status1);
            statusRepository.EnsurePersistent(status2);
        }

    }
}