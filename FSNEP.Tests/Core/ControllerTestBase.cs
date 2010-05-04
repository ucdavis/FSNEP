using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using FSNEP.Controllers;
using UCDArch.Core.DomainModel;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Tests.Core
{
    [TestClass]
    public abstract class ControllerTestBase<CT> where CT : SuperController
    {
        protected CT Controller { get; private set; }
        private TestControllerBuilder Builder { get; set; }

        protected ControllerTestBase()
        {
            new RouteConfigurator().RegisterRoutes();

            Builder = new TestControllerBuilder();

            SetupController();

            Controller.Repository = MockRepository.GenerateStub<IRepository>();
        }

        /// <summary>
        /// Override this method to setup a controller that doesn't have an empty ctor
        /// </summary>
        protected virtual void SetupController()
        {
            Controller = Builder.CreateController<CT>();
        }

        protected void CreateController(params object[] constructorArgs)
        {
            Controller = Builder.CreateController<CT>(constructorArgs);
        }

        protected IRepository<T> FakeRepository<T>() where T : ValidatableObject
        {
            return MockRepository.GenerateStub<IRepository<T>>();
        }
    }
}
