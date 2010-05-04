using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using FSNEP.BLL.Interfaces;

namespace FSNEP.Tests.Auditing
{
    [TestClass]
    public class AuditInterceptorTests
    {
        public AuditInterceptor AuditInterceptor { get; set; }

        public AuditInterceptorTests()
        {
            var userAuth = MockRepository.GenerateStub<IUserAuth>();

            AuditInterceptor = new AuditInterceptor(userAuth);
        }

        [TestMethod]
        public void CreateObjectSavesTheCurrentUser()
        {
            Assert.Fail("Need to write the audit class");
        }
    }
}