using System.Web.Mvc;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.Utils;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Controllers
{
    public class FileController : ApplicationController
    {
        private readonly IRepository<CostShare> _costShareRepository;
        private readonly ICostShareBLL _costShareBLL;

        public FileController(IRepository<CostShare> costShareRepository, ICostShareBLL costShareBLL)
        {
            _costShareRepository = costShareRepository;
            _costShareBLL = costShareBLL;
        }

        public ActionResult ViewEntryFile(int entryId)
        {
            var entry = Repository.OfType<CostShareEntry>().GetNullableByID(entryId);

            Check.Require(entry != null, "Invalid entry identifier");

            var costShare = _costShareRepository.GetNullableByID(entry.Record.Id);

            Check.Require(costShare != null, "Invalid cost share identifier");

            if (!_costShareBLL.HasReviewAccess(CurrentUser, costShare))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to review this cost share", CurrentUser.Identity.Name));
            }

            return File(entry.EntryFile.Content, entry.EntryFile.ContentType, entry.EntryFile.Name);
        }
    }
}