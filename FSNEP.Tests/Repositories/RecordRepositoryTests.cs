using System.Linq;
using FSNEP.Tests.Core;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class RecordRepositoryTests : RepositoryTestBase
    {
        private const int ValidYear = 2009;
        private const int ValidMonth = 2009;

        private readonly IRepository _repository = new Repository();

        [TestMethod]
        public void CanSaveValidRecord()
        {
            var record = CreateValidRecord();

            Repository.OfType<Record>().EnsurePersistent(record);

            Assert.AreEqual(false, record.IsTransient());
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

        private void LoadStatus()
        {
            var status1 = new Status {Name = "S1"};
            var status2 = new Status { Name = "S2" };

            var statusRepository = _repository.OfType<Status>();

            statusRepository.EnsurePersistent(status1);
            statusRepository.EnsurePersistent(status2);
        }

    }
}