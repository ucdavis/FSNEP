using System;
using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.Utils;

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

        public ActionResult TimeRecordList()
        {
            var records = _timeRecordBLL.GetReviewableAndCurrentRecords(CurrentUser);

            return View(records.Cast<Record>().ToList());
        }

        public ActionResult CostShareReview(int id)
        {
            var costShare = Repository.OfType<CostShare>().GetNullableByID(id);

            Check.Require(costShare != null);

            _costShareBLL.HasReviewAccess(CurrentUser, costShare);

            throw new NotImplementedException();
        }
    }
}