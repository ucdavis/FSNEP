using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;

namespace FSNEP.Controllers
{
    [HandleErrorWithELMAH]
    public class SuperController : Controller
    {
        private const string TempDataMessageKey = "Message";

        public string Message
        {
            get { return TempData[TempDataMessageKey] as string; }
            set { TempData[TempDataMessageKey] = value; }
        }

    }
}