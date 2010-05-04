using System;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class AuditRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<Audit> _auditRepository = new Repository<Audit>();

        //Don't load any data
        protected override void LoadData() { }

        [TestMethod]
        public void CanSaveCompleteAndValidAudit()
        {
            var audit = new Audit
                            {
                                AuditDate = DateTime.Now,
                                ObjectName = "User",
                                ObjectId = "154",
                                Username = "newuser"
                            };
            
            audit.SetActionCode(AuditActionType.Update);

            using (var ts = new TransactionScope())
            {
                _auditRepository.EnsurePersistent(audit);

                ts.CommitTransaction();
            }

            Assert.AreEqual(false, audit.IsTransient());
        }

        [TestMethod]
        public void CanSaveAuditWithoutObjectId()
        {
            var audit = new Audit
            {
                AuditDate = DateTime.Now,
                ObjectName = "User",
                Username = "newuser"
            };

            audit.SetActionCode(AuditActionType.Update);

            using (var ts = new TransactionScope())
            {
                _auditRepository.EnsurePersistent(audit);

                ts.CommitTransaction();
            }

            Assert.AreEqual(false, audit.IsTransient());
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveAuditWithoutSettingActionCode()
        {
            var audit = new Audit
            {
                AuditDate = DateTime.Now,
                ObjectName = "User",
                Username = "newuser"
            };

            using (var ts = new TransactionScope())
            {
                _auditRepository.EnsurePersistent(audit);

                ts.CommitTransaction();
            }
        }

        [TestMethod]
        public void BlankAuditShouldFailValidation()
        {
            var audit = new Audit();

            var isValid = audit.IsValid();

            Assert.AreEqual(false, isValid);
        }

    }
}