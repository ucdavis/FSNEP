using System;
using System.Web.Mvc;
using FSNEP.BLL.Interfaces;
using UCDArch.Core.Utils;
using FSNEP.Core.Domain;
namespace FSNEP.Controllers
{
    public class TimeRecordController : SuperController
    {
        private readonly ITimeRecordBLL _timeRecordBLL;

        public TimeRecordController(ITimeRecordBLL timeRecordBLL)
        {
            Check.Require(timeRecordBLL != null);

            _timeRecordBLL = timeRecordBLL;
        }

        [ActionName("Entry")]
        public ActionResult TimeRecordEntry(int id)
        {
            var timeRecord = _timeRecordBLL.GetNullableByID(id);

            Check.Require(timeRecord != null, "Invalid time record indentifier");

            var currentUser = ControllerContext.HttpContext.User.Identity.Name;

            if (!_timeRecordBLL.HasAccess(currentUser, timeRecord))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to this time record", currentUser));
            }

            if (!_timeRecordBLL.IsEditable(timeRecord))
            {
                throw new NotImplementedException("Need to redirect to time record review page");
            }

            var viewModel = TimeRecordEntryViewModel.Create();

            viewModel.TimeRecord = timeRecord;

            return View(viewModel);
        }

        private RedirectToRouteResult RedirectToErrorPage(string message)
        {
            Message = message;

            return RedirectToAction("Error", "Home");
        }
    }

    public class TimeRecordEntryViewModel
    {
        public static TimeRecordEntryViewModel Create()
        {
            var viewModel = new TimeRecordEntryViewModel();

            return viewModel;
        }

        public TimeRecord TimeRecord { get; set; }
    }
}