using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using MvcContrib;

namespace FSNEP.Controllers
{
    public class SupervisorController : SuperController
    {
        private readonly ICostShareBLL _costShareBLL;
        private readonly ITimeRecordBLL _timeRecordBLL;

        public SupervisorController(ICostShareBLL costShareBLL, ITimeRecordBLL timeRecordBLL)
        {
            _costShareBLL = costShareBLL;
            _timeRecordBLL = timeRecordBLL;
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
                                                                              costShare);

            return View(viewModel);
        }

        public ActionResult TimeRecordList()
        {
            var records = _timeRecordBLL.GetReviewableAndCurrentRecords(CurrentUser);

            return View(records.Cast<Record>().ToList());
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
                                                                              timeRecord);

            return View(viewModel);
        }
    }

    public class ReviewViewModel<T,TEnT> where TEnT : Entry where T : Record
    {
        public static ReviewViewModel<T,TEnT> Create(IRepository<TEnT> entryRepository, T record)
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
    }
}