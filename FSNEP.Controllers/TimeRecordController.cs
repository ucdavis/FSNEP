using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Calendar;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using FSNEP.Core.Domain;
namespace FSNEP.Controllers
{
    public class TimeRecordController : SuperController
    {
        private readonly ITimeRecordBLL _timeRecordBLL;
        private readonly IUserBLL _userBLL;
        private readonly ITimeRecordCalendarGenerator _timeRecordCalendarGenerator;

        public TimeRecordController(ITimeRecordBLL timeRecordBLL, IUserBLL userBLL, ITimeRecordCalendarGenerator timeRecordCalendarGenerator)
        {
            Check.Require(timeRecordBLL != null);
            Check.Require(timeRecordCalendarGenerator != null);

            _timeRecordBLL = timeRecordBLL;
            _userBLL = userBLL;
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

            var viewModel = TimeRecordEntryViewModel.Create(Repository, timeRecord, _userBLL, _timeRecordCalendarGenerator);

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
        public static TimeRecordEntryViewModel Create(IRepository repository, TimeRecord timeRecord, IUserBLL userBLL, ITimeRecordCalendarGenerator calendarGenerator)
        {
            var viewModel = new TimeRecordEntryViewModel
                                {
                                    TimeRecord = timeRecord,
                                    CalendarDays = calendarGenerator.GenerateCalendar(timeRecord),
                                    Projects = userBLL.GetAllProjectsByUser(repository.OfType<Project>()).ToList(),
                                    FundTypes = userBLL.GetUser().FundTypes,
                                    ActivityCategories =
                                        repository.OfType<ActivityCategory>().Queryable.Where(c => c.IsActive).OrderBy(
                                        c => c.Name).ToList()
                                };

            return viewModel;
        }

        public TimeRecord TimeRecord { get; set; }
        public IList<TimeRecordCalendarDay> CalendarDays { get; set; }
        public IList<Project> Projects { get; set; }
        public IList<FundType> FundTypes { get; set; }
        public IList<ActivityCategory> ActivityCategories { get; set; }
    }
}