using System.Linq;
using CAESArch.Core.DataInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Auditing
{
    [TestClass]
    public class AuditInterceptorTests
    {
        public AuditInterceptor AuditInterceptor { get; set; }
        public IRepository<Audit> AuditRepository { get; set; }

        public AuditInterceptorTests()
        {
            var userAuth = MockRepository.GenerateStub<IUserAuth>();
            AuditRepository = MockRepository.GenerateStub<IRepository<Audit>>();

            AuditInterceptor = new AuditInterceptor(userAuth, AuditRepository);
        }

        [TestMethod]
        public void AuditObjectModificationSouldNotSaveAuditEntity()
        {
            AuditInterceptor.UserAuth.Expect(a => a.CurrentUserName).Return("currentUser");

            AuditInterceptor.AuditObjectModification(new Audit(), null, AuditActionType.Update);

            AuditRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<Audit>.Is.Anything));
        }

        [TestMethod]
        public void AuditObjectModificationSavesTheCurrentUser()
        {
            Audit audit = null;

            AuditInterceptor.UserAuth.Expect(a => a.CurrentUserName).Return("currentUser");

            AuditRepository
                .Expect(a => a.EnsurePersistent(Arg<Audit>.Is.Anything))
                .WhenCalled(a => audit = (Audit)a.Arguments.First());

            AuditInterceptor.AuditObjectModification(new object(), null, AuditActionType.Update);

            Assert.AreEqual("currentUser", audit.Username);
        }

        [TestMethod]
        public void AuditObjectModificationLeavesObjectIdNull()
        {
            Audit audit = null;

            AuditInterceptor.UserAuth.Expect(a => a.CurrentUserName).Return("currentUser");

            AuditRepository
                .Expect(a => a.EnsurePersistent(Arg<Audit>.Is.Anything))
                .WhenCalled(a => audit = (Audit)a.Arguments.First());

            AuditInterceptor.AuditObjectModification(new object(), null, AuditActionType.Update);

            Assert.IsNull(audit.ObjectId);
        }

        [TestMethod]
        public void AuditObjectModificationSetsObjectName()
        {
            var sampleObject = new RouteConfigurator();

            Audit audit = null;

            AuditInterceptor.UserAuth.Expect(a => a.CurrentUserName).Return("currentUser");

            AuditRepository
                .Expect(a => a.EnsurePersistent(Arg<Audit>.Is.Anything))
                .WhenCalled(a => audit = (Audit)a.Arguments.First());

            AuditInterceptor.AuditObjectModification(sampleObject, null, AuditActionType.Update);

            Assert.AreEqual("RouteConfigurator", audit.ObjectName);
        }

        [TestMethod]
        public void AuditObjectModificationCallsEnsurePersistant()
        {
            AuditInterceptor.UserAuth.Expect(a => a.CurrentUserName).Return("currentUser");

            AuditRepository.Expect(a => a.EnsurePersistent(Arg<Audit>.Is.Anything));

            AuditInterceptor.AuditObjectModification(new object(), null, AuditActionType.Update);

            AuditRepository.AssertWasCalled(a=>a.EnsurePersistent(Arg<Audit>.Is.Anything));
        }
    }
}