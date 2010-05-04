using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Calendar;
using UCDArch.Core.Utils;
using FSNEP.Core.Domain;
namespace FSNEP.Controllers
{
    public class TimeRecordController : SuperController
    {
        private readonly ITimeRecordBLL _timeRecordBLL;
        private readonly ITimeRecordCalendarGenerator _timeRecordCalendarGenerator;

        public TimeRecordController(ITimeRecordBLL timeRecordBLL, ITimeRecordCalendarGenerator timeRecordCalendarGenerator)
        {
            Check.Require(timeRecordBLL != null);
            Check.Require(timeRecordCalendarGenerator != null);

            _timeRecordBLL = timeRecordBLL;
            _timeRecordCalendarGenerator = timeRecordCalendarGenerator;
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

            var viewModel = TimeRecordEntryViewModel.Create(timeRecord, _timeRecordCalendarGenerator);

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
        public static TimeRecordEntryViewModel Create(TimeRecord timeRecord, ITimeRecordCalendarGenerator calendarGenerator)
        {
            var viewModel = new TimeRecordEntryViewModel
                                {
                                    TimeRecord = timeRecord,
                                    CalendarDays = calendarGenerator.GenerateCalendar(timeRecord)
                                };

            return viewModel;
        }

        public TimeRecord TimeRecord { get; set; }
        public IList<TimeRecordCalendarDay> CalendarDays { get; set; }
    }
}