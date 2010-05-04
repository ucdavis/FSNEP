using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Calendar;
using MvcContrib.Attributes;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using FSNEP.Core.Domain;
using UCDArch.Web.ActionResults;
using UCDArch.Web.Attributes;

namespace FSNEP.Controllers
{
    [Authorize] //TODO: Authorize for only time record users
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

            var viewModel = TimeRecordEntryViewModel.Create(Repository, _userBLL, _timeRecordBLL, timeRecord, _timeRecordCalendarGenerator);

            return View(viewModel);
        }

        [AcceptPost]
        [Transaction]
        public JsonNetResult AddEntry(int recordId, TimeRecordEntry entry)
        {
            var timeRecord = _timeRecordBLL.GetNullableByID(recordId);

            Check.Require(timeRecord != null, "Invalid time record indentifier");

            var currentUser = ControllerContext.HttpContext.User.Identity.Name;

            Check.Require(_timeRecordBLL.HasAccess(currentUser, timeRecord),
                          "Current user does not have access to this record");

            timeRecord.AddEntry(entry);//Add the entry to the time record

            Check.Require(entry.IsValid(), "Entry is not valid");

            _timeRecordBLL.EnsurePersistent(timeRecord);
            
            _timeRecordBLL.DbContext.CommitTransaction();

            //return the new Id
            return new JsonNetResult(new {id = entry.Id});
        }

        [AcceptPost]
        [Transaction]
        public void RemoveEntry(int entryId)
        {
            var entryRepository = Repository.OfType<TimeRecordEntry>();

            var entry = entryRepository.GetById(entryId);

            entryRepository.Remove(entry);
        }

        [AcceptPost]
        [Transaction]
        public void EditEntry(int entryId, TimeRecordEntry entry)
        {
            var entryRepository = Repository.OfType<TimeRecordEntry>();

            var entryToUpdate = entryRepository.GetNullableByID(entryId);

            Check.Require(entryToUpdate != null, "Entry not found");

            TransferValuesTo(entryToUpdate, entry);

            Check.Require(entryToUpdate.IsValid(), "Entry is not valid");

            entryRepository.EnsurePersistent(entryToUpdate);
        }

        [Transaction]
        public JsonNetResult GetEntry(int entryId)
        {
            var entry = Repository.OfType<TimeRecordEntry>().GetNullableByID(entryId);

            Check.Require(entry != null, "Entry not found");

            return new JsonNetResult(entry);
        }

        /// <summary>
        /// Transfer the new values from the given entry to the entry to update.
        /// Currently you can only update the comment and hours
        /// </summary>
        private static void TransferValuesTo(TimeRecordEntry entryToUpdate, TimeRecordEntry entry)
        {
            entryToUpdate.Comment = entry.Comment;
            entryToUpdate.Hours = entry.Hours;
        }

        private RedirectToRouteResult RedirectToErrorPage(string message)
        {
            Message = message;

            return RedirectToAction("Error", "Home");
        }
    }

    public class TimeRecordEntryViewModel
    {
        public static TimeRecordEntryViewModel Create(IRepository repository, IUserBLL userBLL, ITimeRecordBLL timeRecordBLL, TimeRecord timeRecord, ITimeRecordCalendarGenerator calendarGenerator)
        {
            var viewModel = new TimeRecordEntryViewModel
                                {
                                    TimeRecord = timeRecord,
                                    IsSubmittable = timeRecordBLL.IsSubmittable(timeRecord),
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
        public bool IsSubmittable { get; set; }
    }
}