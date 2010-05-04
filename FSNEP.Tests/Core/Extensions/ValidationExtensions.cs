using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace FSNEP.Tests.Core.Extensions
{
    public static class ValidationExtensions
    {
        public static List<ValidationResult> AsList(this IEnumerable<ValidationResult> validationResults)
        {
            var resultsList = new List<ValidationResult>();

            foreach (var result in validationResults)
            {
                resultsList.Add(result);
            }

            return resultsList;
        }

        public static List<string> AsMessageList(this IEnumerable<ValidationResult> validationResults)
        {
            var resultsList = new List<string>();

            foreach (var result in validationResults)
            {
                resultsList.Add(string.Format("{0}: {1}", result.Key, result.Message));
            }

            return resultsList;
        }

    }

}
