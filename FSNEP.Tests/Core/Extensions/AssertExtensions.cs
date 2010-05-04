using System.Collections.Generic;
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

    }
}
