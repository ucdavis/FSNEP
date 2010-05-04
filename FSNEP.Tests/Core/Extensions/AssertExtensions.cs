﻿using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Core.Extensions
{
    static class AssertExtensions
    {
        public static void AssertContains(this ICollection<string> list, string str)
        {
            Assert.IsTrue(list.Contains(str), "Expect value \"" + str + "\" not found.");
        }

        public static void AssertContains(this ICollection<string> list, string str, string message)
        {
            Assert.IsTrue(list.Contains(str), message);
        }

        /// <summary>
        /// Asserts the errors are exactly as specified.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="errors">The errors.</param>
        public static void AssertErrorsAre(this ModelStateDictionary modelState, params string[] errors)
        {
            var resultsList = new List<string>();

            foreach (var result in modelState.Values)
            {
                foreach (var errs in result.Errors)
                {
                    resultsList.Add(errs.ErrorMessage);
                }
            }

            Assert.AreEqual(resultsList.Count, errors.Length, "Number of error messages do not match");
            //foreach (var error in errors)
            //{
            //    Assert.IsTrue(resultsList.Contains(error), "Expected error \"" + error + "\" not found");
            //}
            //newUserModel.ViewData.ModelState.AssertErrorsAre("FirstName: The value cannot be null.",
            //                                                 "FirstName: The value cannot be null.");
            //Changed it so the above cut and paste error with a unit test could not happen.
            var errorsList = new List<string>();
            foreach (var error  in errors)
            {
                errorsList.Add(error);
            }

            //Look through the errors in the modelState and check they are as expected.
            foreach (var error in resultsList)
            {
                Assert.IsTrue(errorsList.Contains(error), "Expected error \"" + error + "\" not found");
            }
        }
    }
}
