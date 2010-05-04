using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;
using MvcContrib.Attributes;
using ApplicationException=Elmah.ApplicationException;

namespace FSNEP.Controllers
{
    public class SupervisorController : SuperController
    {
        private readonly ICostShareBLL _costShareBLL;
        private readonly ITimeRecordBLL _timeRecordBLL;
        private readonly IUserBLL _userBLL;
        private readonly IDelegateBLL _delegateBLL;

        public SupervisorController(ICostShareBLL costShareBLL, ITimeRecordBLL timeRecordBLL, IUserBLL userBLL, IDelegateBLL delegateBLL)
        {
            _costShareBLL = costShareBLL;
            _timeRecordBLL = timeRecordBLL;
            _userBLL = userBLL;
            _delegateBLL = delegateBLL;
        }

        public ActionResult CostShareList()
        {
            var records = _costShareBLL.GetReviewableAndCurrentRecords(CurrentUser);
            
            return View(records.Cast<Record>().ToList());
        }

        public ActionResult CostShareReview(int id)
        {
            var costShare = Repository.OfType<CostShare>().GetNullableByID(id);

            Check.Require(costShare != null);

            if (!_costShareBLL.HasReviewAccess(CurrentUser, costShare))
            {
                Message = "You do not have access to this record. Please choose another from this list.";

                return this.RedirectToAction(x => x.CostShareList());
            }

            var viewModel = ReviewViewModel<CostShare, CostShareEntry>.Create(Repository.OfType<CostShareEntry>(),
                                                                              _costShareBLL,
                                                                              costShare);

            return View(viewModel);
        }

        public ActionResult TimeRecordList()
        {
            var records = _timeRecordBLL.GetReviewableAndCurrentRecords(CurrentUser);

            return View(records.Cast<Record>().ToList()
                .OrderByDescending(a => a.Year)
                .ThenByDescending(a => a.Month)
                .ThenBy(a => a.User.FullNameLastFirst)
                );
        }

        public ActionResult TimeRecordReview(int id)
        {
            var timeRecord = Repository.OfType<TimeRecord>().GetNullableByID(id);

            Check.Require(timeRecord != null);

            if (!_timeRecordBLL.HasReviewAccess(CurrentUser, timeRecord))
            {
                Message = "You do not have access to this record. Please choose another from this list.";

                return this.RedirectToAction(x => x.TimeRecordList());
            }

            var viewModel = ReviewViewModel<TimeRecord, TimeRecordEntry>.Create(Repository.OfType<TimeRecordEntry>(),
                                                                                _timeRecordBLL,
                                                                                timeRecord);


            return View(viewModel);
        }

        public ActionResult ApproveOrDenyRecord(int id, string reviewComment, bool approved)
        {
            var record = Repository.OfType<Record>().GetNullableByID(id);
            
            Check.Require(record != null, "Invalid record identifier");

            record.ReviewComment = reviewComment;

            if (record is TimeRecord)
            {
                var timeRecord = (TimeRecord) record;

                if (!_timeRecordBLL.HasReviewAccess(CurrentUser, timeRecord))
                {
                    return RedirectToErrorPage(string.Format("{0} does not have access to approve or deny this time record", CurrentUser.Identity.Name));
                }

                _timeRecordBLL.ApproveOrDeny(timeRecord, CurrentUser, approved);

                Message = string.Format("Time Record {0} Successfully", approved ? "Approved" : "Disapproved");

                return this.RedirectToAction(x => x.TimeRecordList());
            }
            
            if (record is CostShare)
            {
                var costShare = (CostShare)record;

                if (!_costShareBLL.HasReviewAccess(CurrentUser, costShare))
                {
                    return RedirectToErrorPage(string.Format("{0} does not have access to approve or deny this cost share", CurrentUser.Identity.Name));
                }

                _costShareBLL.ApproveOrDeny(costShare, CurrentUser, approved);

                Message = string.Format("Cost Record {0} Successfully", approved ? "Approved" : "Disapproved");

                return this.RedirectToAction(x => x.CostShareList());
            }

            //If we haven't matched time record or cost share
            throw new ApplicationException("You can only approve or deny 'Time Records' and 'Cost Shares'.");
        }

        #region Delegates

        [Authorize(Roles="Supervisor")]
        public RedirectToRouteResult Delegate()
        {
            return CurrentUserHasDelegate() ? RedirectToAction("RemoveDelegate") : RedirectToAction("AssignDelegate");
        }

        /// <summary>
        /// Return a list of all active users (except the current one), any one of which can be assigned as a delegate for this supervisor
        /// </summary>
        [Authorize(Roles = "Supervisor")]
        public ActionResult AssignDelegate()
        {
            if (CurrentUserHasDelegate()) return this.RedirectToAction(x => x.RemoveDelegate());

            var users = Repository.OfType<User>().Queryable.Where(x => x.IsActive && x.UserName != CurrentUser.Identity.Name).OrderBy(x=>x.LastName).ToList();

            return View(users);
        }

        [AcceptPost]
        public ActionResult AssignDelegate(Guid userId)
        {
            var userToAssign = _userBLL.GetNullableByID(userId);

            Check.Require(userToAssign != null);

            _delegateBLL.AssignDelegate(userToAssign);

            Message = string.Format("{0} Assigned as a Delegate", userToAssign.FullName);

            return this.RedirectToAction<HomeController>(x => x.Index());
        }

        [Authorize(Roles="Supervisor")]
        public ActionResult RemoveDelegate()
        {
            if (!CurrentUserHasDelegate()) return this.RedirectToAction(x => x.AssignDelegate());
            
            Guid delegateUserId =
                _userBLL.Queryable.Where(x => x.UserName == CurrentUser.Identity.Name && x.Delegate != null).Select(
                x => x.Delegate.Id).Single();
            
            var delegateUser = _userBLL.GetNullableByID(delegateUserId);
            
            return View(delegateUser);
        }

        [AcceptPost]
        public ActionResult RemoveDelegate(Guid userId)
        {
            var userToRemove = _userBLL.GetNullableByID(userId);

            Check.Require(userToRemove != null);

            _delegateBLL.RemoveDelegate(userToRemove);

            Message = string.Format("{0} Removed as a Delegate", userToRemove.FullName);

            return this.RedirectToAction<HomeController>(x => x.Index());
        }

        public bool CurrentUserHasDelegate()
        {
            var currentUserHasDelegate = Repository.OfType<User>()
                                            .Queryable
                                            .Where(x => x.UserName == CurrentUser.Identity.Name && x.Delegate != null)
                                            .Any();

            return currentUserHasDelegate;
        }

        #endregion
    }

    public class ReviewViewModel<T,TEnT> where TEnT : Entry where T : Record
    {
        public static ReviewViewModel<T,TEnT> Create(IRepository<TEnT> entryRepository, IRecordBLL<T> recordBLL, T record)
        {
            var viewModel = new ReviewViewModel<T,TEnT>
                                {
                                    Record = record,
                                    Entries = entryRepository
                                        .Queryable
                                        .Where(x => x.Record.Id == record.Id)
                                        .ToList()
                                };

            return viewModel;
        }

        public T Record { get; set; }
        public IEnumerable<TEnT> Entries { get; set; }
        public bool CanBeApprovedOrDenied { get; set; }
    }
}