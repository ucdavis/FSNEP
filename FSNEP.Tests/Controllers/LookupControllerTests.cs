using CAESArch.Core.DataInterfaces;
using FSNEP.Controllers;
using FSNEP.Tests.Core;
using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class LookupControllerTests : ControllerTestBase<LookupController>
    {
        protected override void SetupController()
        {
            var repository = MockRepository.GenerateStub<IRepository>();

            CreateController(repository);
        }

        [TestMethod]
        public void RoutingProjectListGetsAllProjects()
        {
            "~/Administration/Lookups/Projects"
                .ShouldMapTo<LookupController>(a => a.Projects());
        }
    }
}