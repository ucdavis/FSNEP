using System.Collections.Generic;
using UCDArch.Core;
using UCDArch.Core.CommonValidator;

namespace FSNEP.Controllers.Helpers
{
    public class Validation
    {
        public static ICollection<IValidationResult> GetValidationResultsFor(object objToValidate)
        {
            return GetValidator().ValidationResultsFor(objToValidate);
        }

        private static IValidator GetValidator()
        {
            return SmartServiceLocator<IValidator>.GetService();
        }
    }
    /*
    public static class ValidationHelper<T>
    {
        public static bool Validate(T objToValidate, ModelStateDictionary modelState)
        {
            var validationResults = CAESArch.Core.Utils.ValidateBusinessObject<T>.GetValidationResults(objToValidate);

            foreach (var validationResult in validationResults)
            {
                modelState.AddModelError(validationResult.Key, string.Format("{0}: {1}", validationResult.Key, validationResult.Message));
            }

            return validationResults.IsValid;
        }

        public static bool Validate(T objToValidate, ModelStateDictionary modelState, string prefix)
        {
            var validationResults = CAESArch.Core.Utils.ValidateBusinessObject<T>.GetValidationResults(objToValidate);

            foreach (var validationResult in validationResults)
            {
                modelState.AddModelError(string.Format("{0}.{1}", prefix, validationResult.Key), string.Format("{0}: {1}", validationResult.Key, validationResult.Message));
            }

            return validationResults.IsValid;
        }

        /// <summary>
        /// Builds a string of all the Error Messages in all the Values of the ModelState.
        /// </summary>
        /// <param name="objToValidate"></param>
        /// <param name="modelState"></param>
        /// <returns>String of Error Messages</returns>
        public static string GetErrorMessages(T objToValidate, ModelStateDictionary modelState)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var val in modelState.Values)
            {
                foreach (var err in val.Errors)
                {
                    sb.Append(err.ErrorMessage);
                }
            }
            return sb.ToString();
        }
    }
     */
}