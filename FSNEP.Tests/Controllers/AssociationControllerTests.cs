using FSNEP.Controllers;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class AssociationControllerTests : ControllerTestBase<AssociationController>
    {
        [TestMethod]
        public void RoutingProjectIdMapsToProjectWithIdParam()
        {
            int? id = 5;

            "~/Administration/Association/Projects/5"
                .ShouldMapTo<AssociationController>(a => a.Projects(id));
        }
    }
}