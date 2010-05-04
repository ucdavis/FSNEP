using System;
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
using UCDArch.Web.Validator;

namespace FSNEP.Controllers
{
    [Authorize]
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

        public ActionResult Entry(int id)
        {
            var costShare = _costShareRepository.GetNullableByID(id);

            Check.Require(costShare != null, "Invalid cost share indentifier");

            if (!_costShareBLL.HasAccess(CurrentUser, costShare))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to this cost share", CurrentUser.Identity.Name));
            }

            if (!_costShareBLL.IsEditable(costShare))
            {
                throw new NotImplementedException("Need to redirect to the cost share review page");
            }

            var viewModel = CostShareEntryViewModel.Create(Repository, _userBLL, _costShareBLL, costShare);
            viewModel.Entry = new CostShareEntry();

            return View(viewModel);
        }

        [AcceptPost]
        [Transaction]
        public ActionResult Entry(int id, CostShareEntry entry)
        {
            var costShare = _costShareRepository.GetNullableByID(id);

            Check.Require(costShare != null, "Invalid time record indentifier");

            Check.Require(_costShareBLL.HasAccess(CurrentUser, costShare),
                          "Current user does not have access to this record");


            entry.TransferValidationMessagesTo(ModelState);

            if (!entry.IsValid())
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
        public ActionResult RemoveEntry(int id, int entryId)
        {
            var entryRepository = Repository.OfType<CostShareEntry>();

            var entry = entryRepository.GetById(entryId);

            entryRepository.Remove(entry);

            Message = "Cost Share Entry Removed";

            return this.RedirectToAction(x => x.Entry(id));
        }
    }


    public class CostShareEntryViewModel
    {
        public static CostShareEntryViewModel Create(IRepository repository, IUserBLL userBLL, ICostShareBLL costShareBLL, CostShare costShare)
        {
            var viewModel = new CostShareEntryViewModel
                                {
                                    CostShare = costShare,
                                    Projects = userBLL.GetAllProjectsByUser(repository.OfType<Project>()).ToList(),
                                    FundTypes = userBLL.GetUser().FundTypes,
                                    ExpenseTypes =
                                        repository.OfType<ExpenseType>().Queryable.Where(x => x.IsActive).OrderBy(
                                        x => x.Name).ToList()
                                };

            return viewModel;
        }

        public CostShare CostShare { get; set; }
        public CostShareEntry Entry { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<FundType> FundTypes { get; set; }
        public IEnumerable<ExpenseType> ExpenseTypes { get; set; }
    }
}