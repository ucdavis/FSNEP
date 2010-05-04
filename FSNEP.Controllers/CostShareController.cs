using System.Linq;
using System.Web.Mvc;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Web.Attributes;

namespace FSNEP.Controllers
{
    [Authorize]
    public class CostShareController : SuperController
    {
        private readonly IRepository<CostShare> _costShareRepository;

        public CostShareController(IRepository<CostShare> costShareRepository)
        {
            _costShareRepository = costShareRepository;
        }

        [HandleTransactionManually]
        public ActionResult History()
        {
            var recordHistory =
                _costShareRepository.Queryable.Where(x => x.User.UserName == CurrentUser.Identity.Name).
                    OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            return View(recordHistory);
        }
    }
}