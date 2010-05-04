using FSNEP.BLL.Impl;
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
            var projectsBLL = MockRepository.GenerateStub<IProjectBLL>();

            CreateController(projectsBLL);
        }

        [TestMethod]
        public void RoutingProjectListGetsAllProjects()
        {
            "~/Administration/Lookups/Projects"
                .ShouldMapTo<LookupController>(a => a.Projects());
        }
    }
}