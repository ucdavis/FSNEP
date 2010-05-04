using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;

namespace FSNEP.Tests.Core.Extensions
{
    public static class RouteTestingExtensions
    {
        public static void ShouldMapToIgnoringParams(this string strRoute, string expectedController, string expectedAction)
        {
            var route = strRoute.Route();

            var actualController = route.Values["controller"] as string;
            var actualAction = route.Values["action"] as string;


            if (actualController != expectedController)
            {
                throw new ArgumentException(string.Format("Controller was {0}, expected {1}", actualController,
                                                          expectedController));
            }

            if (actualAction != expectedAction)
            {
                throw new ArgumentException(string.Format("Action was {0}, expected {1}", actualAction,
                                                          expectedAction));
            }            
        }

        /// <summary>
        /// Validates that the route should map to a particular controller and action.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="routeData">The route data.</param>
        /// <param name="action">The action.</param>
        /// <param name="ignoreParameters">if set to <c>true</c> [ignore parameters].</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, ActionResult>> action, bool ignoreParameters) where TController : Controller
        {
            Assert.IsNotNull(routeData, "The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression) action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.Name;
            actualAction.AssertSameStringAs(expectedAction);

            //check parameters
            if (!ignoreParameters)
            {
                for (int i = 0; i < methodCall.Arguments.Count; i++)
                {
                    string name = methodCall.Method.GetParameters()[i].Name;

                    object value = ((ConstantExpression) methodCall.Arguments[i]).Value;

                    Assert.AreEqual(routeData.Values.GetValue(name), value.ToString());
                    //Assert.That(routeData.Values.GetValue(name), Is.EqualTo(value.ToString()));
                }
            }
            return routeData;
        }

        /// <summary>
        /// Validates that the route should map to a particular controller and action.
        /// This one uses actions without a return value.
        /// </summary>
        /// <typeparam name="TController">The type of the controller.</typeparam>
        /// <param name="routeData">The route data.</param>
        /// <param name="action">The action.</param>
        /// <param name="ignoreParameters">if set to <c>true</c> [ignore parameters].</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Action<TController>> action, bool ignoreParameters) where TController : Controller
        {
            Assert.IsNotNull(routeData, "The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();
            string expectedAction = methodCall.Method.Name;
            actualAction.AssertSameStringAs(expectedAction);

            //check parameters
            if (!ignoreParameters)
            {
                for (int i = 0; i < methodCall.Arguments.Count; i++)
                {
                    string name = methodCall.Method.GetParameters()[i].Name;

                    object value = ((ConstantExpression)methodCall.Arguments[i]).Value;

                    Assert.AreEqual(routeData.Values.GetValue(name), value.ToString());
                    //Assert.That(routeData.Values.GetValue(name), Is.EqualTo(value.ToString()));
                }
            }
            return routeData;
        }
    }
}