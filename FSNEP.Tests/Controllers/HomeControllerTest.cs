using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Controllers;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);

            // Assert
            if (result != null)
            {
                var viewData = result.ViewData;
                Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
            }
        }

        /// <summary>
        /// Homes controller error.
        /// </summary>
        [TestMethod]
        public void HomeControllerError()
        {
            var controller = new HomeController();
            var result = controller.Error("This is a test error!") as ViewResult;
            Assert.IsNotNull(result);
            if (result != null)
            {
                Assert.AreEqual("This is a test error!", result.MasterName);
            }
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
