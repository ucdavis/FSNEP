using FSNEP.Controllers;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class AdministrationControllerTests : ControllerTestBase<AdministrationController>
    {
        [TestMethod]
        public void RoutingModifyUserWithNoParamMapsToModifyUserMethodWithEmptyUsername()
        {
            string username = string.Empty;

            "~/Administration/ModifyUser"
                .ShouldMapTo<AdministrationController>(a => a.ModifyUser(username));
        }

        [TestMethod]
        public void RoutingModifyUserWithUsernameMapsToModifyUserMethodWithThatUsername()
        {
            "~/Administration/ModifyUser/testuser"
                .ShouldMapTo<AdministrationController>(a => a.ModifyUser("testuser"));
        }
    }
}
