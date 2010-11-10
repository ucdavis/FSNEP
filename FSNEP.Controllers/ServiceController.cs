using System.Linq;
using System.Web.Mvc;
using FSNEP.Core.Domain;
using UCDArch.Core.Utils;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace FSNEP.Controllers
{
    public class ServiceController : ApplicationController
    {
        [Transaction]
        public ActionResult GetAccountsForProject(int id)
        {
            var project = Repository.OfType<Project>().GetNullableById(id);

            Check.Require(project != null);

            return new JsonNetResult(project.Accounts.ToList());
        }
    }
}