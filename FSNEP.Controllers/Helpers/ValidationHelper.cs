using System.Web.Mvc;

namespace FSNEP.Controllers.Helpers
{
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
    }
}