using System.Web.Mvc;
using UCDArch.Web.ModelBinder;

namespace FSNEP.ModelBinder
{
    public class CustomBinder : UCDArchModelBinder
    {
        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //Do not do auto model validation
        }
    }

}