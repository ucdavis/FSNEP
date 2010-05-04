using System.Collections.Generic;
using System.Web.Mvc;
using CAESArch.BLL;
using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using CAESArch.Core.DataInterfaces;
using FSNEP.Controllers;

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

        protected static INonStaticGenericBLLBase<T, IdT> GetStubRepository<T, IdT>()
        {
            return MockRepository.GenerateStub<INonStaticGenericBLLBase<T, IdT>>();
        }

        protected IRepository<T> FakeRepository<T>()
        {
            return MockRepository.GenerateStub<IRepository<T>>();
        }

        /// <summary>
        /// Count All of the error messages to assert that only the messages that we want are happening.
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns>Count All of the error messages</returns>
        protected int CountErrorMessages(ModelStateDictionary modelState)
        {
            var returnValue = 0;
            foreach (var val in modelState.Values)
            {
                returnValue += val.Errors.Count;                
            }
            return returnValue;
        }

        /// <summary>
        /// Get all the error messages
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        protected List<string> GetErrorMessages(ModelStateDictionary modelState)
        {
            var resultsList = new List<string>();

            foreach (var result in modelState.Values)
            {
                foreach (var errs in result.Errors)
                {
                    resultsList.Add(errs.ErrorMessage);
                }
            }

            return resultsList;
        }

    }
}
