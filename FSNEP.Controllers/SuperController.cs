using System.Web.Mvc;
using FSNEP.Controllers.Helpers.Attributes;
using CAESArch.Core.DataInterfaces;

namespace FSNEP.Controllers
{
    [HandleErrorWithELMAH]
    public class SuperController : Controller
    {
        public IRepository Repository { get; set; } //General repository set through DI

        private const string TempDataMessageKey = "Message";

        public string Message
        {
            get { return TempData[TempDataMessageKey] as string; }
            set { TempData[TempDataMessageKey] = value; }
        }
    }
}