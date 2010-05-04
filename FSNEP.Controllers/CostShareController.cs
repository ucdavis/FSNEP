using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using MvcContrib.Attributes;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using UCDArch.Web.Attributes;
using MvcContrib;
using UCDArch.Web.Helpers;
using System.Web;

namespace FSNEP.Controllers
{
    [Authorize] //TODO: Authorize only for Cost Share users
    public class CostShareController : SuperController
    {
        private readonly IRepository<CostShare> _costShareRepository;
        private readonly ICostShareBLL _costShareBLL;
        private readonly IUserBLL _userBLL;

        public CostShareController(IRepository<CostShare> costShareRepository, ICostShareBLL costShareBLL, IUserBLL userBLL)
        {
            _costShareRepository = costShareRepository;
            _costShareBLL = costShareBLL;
            _userBLL = userBLL;
        }

        [HandleTransactionManually]
        public ActionResult History()
        {
            var recordHistory =
                _costShareRepository.Queryable.Where(x => x.User.UserName == CurrentUser.Identity.Name).
                    OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            return View(recordHistory);
        }

        public ActionResult Current()
        {
            var costShare = _costShareBLL.GetCurrent(CurrentUser);

            if (costShare == null)
            {
                Message = Constants.NoCostShare;

                return this.RedirectToAction(a => a.History());
            }

            return this.RedirectToAction(a => a.Entry(costShare.Id));
        }

        public ActionResult Review(int id)
        {
            var costShare = _costShareRepository.GetNullableByID(id);

            Check.Require(costShare != null, "Invalid cost share indentifier");

            if (!_costShareBLL.HasAccess(CurrentUser, costShare))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to review this cost share", CurrentUser.Identity.Name));
            }

            var viewModel = CostShareReviewViewModel.Create(costShare, Repository);
            
            return View(viewModel);
        }

        public ActionResult Entry(int id)
        {
            var costShare = _costShareRepository.GetNullableByID(id);

            Check.Require(costShare != null, "Invalid cost share indentifier");

            if (!_costShareBLL.HasAccess(CurrentUser, costShare))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to this cost share", CurrentUser.Identity.Name));
            }

            if (!costShare.IsEditable)
            {
                //return RedirectToAction("Review", new { id });
                return this.RedirectToAction(a => a.Review(id));
            }

            var viewModel = CostShareEntryViewModel.Create(Repository, _userBLL, _costShareBLL, costShare);
            viewModel.Entry = new CostShareEntry();

            return View(viewModel);
        }

        [AcceptPost]
        [Transaction]
        [ValidateAntiForgeryToken]
        public ActionResult Entry(int id, CostShareEntry entry, HttpPostedFileBase postedFile)
        {
            var costShare = _costShareRepository.GetNullableByID(id);
            
            Check.Require(costShare != null, "Invalid cost share indentifier");

            Check.Require(_costShareBLL.HasAccess(CurrentUser, costShare),
                          "Current user does not have access to this record");

            entry.Record = costShare;

            if (postedFile != null && postedFile.ContentLength != 0)
            {
                var entryFile = new EntryFile { Name = postedFile.FileName, Content = new byte[postedFile.ContentLength], ContentType = postedFile.ContentType };

                postedFile.InputStream.Read(entryFile.Content, 0, postedFile.ContentLength);

                if (entryFile.IsValid())
                {
                    entry.EntryFile = entryFile;
                }
                else
                {
                    entryFile.TransferValidationMessagesTo(ModelState);
                }
            }
            
            entry.TransferValidationMessagesTo(ModelState);

            if (!ModelState.IsValid)
            {
                var viewModel = CostShareEntryViewModel.Create(Repository, _userBLL, _costShareBLL, costShare);
                viewModel.Entry = entry;

                return View(viewModel);
            }
            
            costShare.AddEntry(entry);//Add the entry to the time record
            
            _costShareRepository.EnsurePersistent(costShare);

            _costShareRepository.DbContext.CommitTransaction();

            Message = "Cost Share Entry Added";

            return this.RedirectToAction(x => x.Entry(id));
        }

        [AcceptPost]
        [Transaction]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveEntry(int entryId)
        {
            var entryRepository = Repository.OfType<CostShareEntry>();

            var entry = entryRepository.GetById(entryId);

            var parentRecord = entry.Record;

            parentRecord.Entries.Remove(entry);

            Repository.OfType<Record>().EnsurePersistent(parentRecord);
            
            Message = "Cost Share Entry Removed";

            return this.RedirectToAction(x => x.Entry(parentRecord.Id));
        }

        [AcceptPost]
        [Transaction]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(int id)
        {
            var costShare = _costShareRepository.GetNullableByID(id);

            Check.Require(costShare != null, "Invalid cost share indentifier");

            if (!_costShareBLL.HasAccess(CurrentUser, costShare))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to submit this cost share", CurrentUser.Identity.Name));
            }

            _costShareBLL.Submit(costShare, CurrentUser);

            Message = string.Format("Cost Share for {0:MMMM yyyy} Submitted Successfully", costShare.Date);

            return this.RedirectToAction(a => a.History());
        }
    }

    public class CostShareReviewViewModel
    {
        public static CostShareReviewViewModel Create(CostShare costShare, IRepository repository)
        {
            var costShareEntries = repository.OfType<CostShareEntry>().Queryable.Where(x => x.Record.Id == costShare.Id);

            var viewModel = new CostShareReviewViewModel
                                {
                                    CostShare = costShare,
                                    CostShareEntries = costShareEntries.ToList()
                                };

            return viewModel;
        }

        public CostShare CostShare { get; set; }
        public IEnumerable<CostShareEntry> CostShareEntries { get; set; }
    }

    public class CostShareEntryViewModel
    {
        public static CostShareEntryViewModel Create(IRepository repository, IUserBLL userBLL, ICostShareBLL costShareBLL, CostShare costShare)
        {
            var viewModel = new CostShareEntryViewModel
                                {
                                    CostShare = costShare,
                                    Projects = userBLL.GetAllProjectsByUser(repository.OfType<Project>()).ToList(),
                                    FundTypes = userBLL.GetUser().FundTypes.ToList(),
                                    ExpenseTypes =
                                        repository.OfType<ExpenseType>().Queryable.Where(x => x.IsActive).OrderBy(
                                        x => x.Name).ToList(),
                                    CostShareEntries = repository.OfType<CostShareEntry>().Queryable.Where(x=>x.Record.Id == costShare.Id).ToList()
                                };

            return viewModel;
        }

        public CostShare CostShare { get; set; }
        public CostShareEntry Entry { get; set; }
        public IEnumerable<CostShareEntry> CostShareEntries { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<FundType> FundTypes { get; set; }
        public IEnumerable<ExpenseType> ExpenseTypes { get; set; }
    }
}