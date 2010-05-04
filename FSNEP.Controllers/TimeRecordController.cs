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
using MvcContrib;
using FSNEP.Controllers.Helpers.Attributes;

namespace FSNEP.Controllers
{
    [TimeRecordOnly]
    public class TimeRecordController : ApplicationController
    {
        private readonly ITimeRecordBLL _timeRecordBLL;
        private readonly IRepository<TimeRecord> _timeRecordRepository;
        private readonly IUserBLL _userBLL;
        private readonly ITimeRecordCalendarGenerator _timeRecordCalendarGenerator;

        public TimeRecordController(ITimeRecordBLL timeRecordBLL, IRepository<TimeRecord> timeRecordRepository, IUserBLL userBLL, ITimeRecordCalendarGenerator timeRecordCalendarGenerator)
        {
            Check.Require(timeRecordBLL != null);
            Check.Require(timeRecordCalendarGenerator != null);

            _timeRecordBLL = timeRecordBLL;
            _timeRecordRepository = timeRecordRepository;
            _userBLL = userBLL;
            _timeRecordCalendarGenerator = timeRecordCalendarGenerator;
        }

        [HandleTransactionsManually]
        public ActionResult History()
        {
            var recordHistory =
                _timeRecordRepository.Queryable.Where(x => x.User.UserName == CurrentUser.Identity.Name).
                    OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);

            return View(recordHistory);
        }

        public ActionResult Current()
        {
            var timeRecord = _timeRecordBLL.GetCurrent(CurrentUser);

            if (timeRecord == null)
            {
                Message = Constants.NoTimeRecord;
                
                return this.RedirectToAction(a => a.History());
            }

            return this.RedirectToAction(a => a.Entry(timeRecord.Id));
        }

        public ActionResult Review(int id)
        {
            var timeRecord = _timeRecordRepository.GetNullableByID(id);

            Check.Require(timeRecord != null, "Invalid cost share identifier");

            if (!_timeRecordBLL.HasAccess(CurrentUser, timeRecord))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to this cost share", CurrentUser.Identity.Name));
            }

            var viewModel = TimeRecordReviewViewModel.Create(timeRecord, Repository);

            return View(viewModel);
        }

        public ActionResult Entry(int id)
        {
            var timeRecord = _timeRecordRepository.GetNullableByID(id);

            Check.Require(timeRecord != null, "Invalid time record identifier");
            
            if (!_timeRecordBLL.HasAccess(CurrentUser, timeRecord))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to this time record", CurrentUser.Identity.Name));
            }

            if (!timeRecord.IsEditable)
            {
                return RedirectToAction("Review", new { id });
            }

            var viewModel = TimeRecordEntryViewModel.Create(Repository, _userBLL, _timeRecordBLL, timeRecord, _timeRecordCalendarGenerator);

            return View(viewModel);
        }

        [AcceptPost]
        [Transaction]
        public JsonNetResult AddEntry(int recordId, TimeRecordEntry entry)
        {
            var timeRecord = _timeRecordRepository.GetNullableByID(recordId);

            Check.Require(timeRecord != null, "Invalid time record identifier");

            Check.Require(_timeRecordBLL.HasAccess(CurrentUser, timeRecord),
                          "Current user does not have access to this record");

            timeRecord.AddEntry(entry);//Add the entry to the time record

            Check.Require(entry.IsValid(), "Entry is not valid");

            _timeRecordRepository.EnsurePersistent(timeRecord);

            _timeRecordRepository.DbContext.CommitTransaction();

            var modificationResult = new TimeRecordEntryModificationDto(entry.Id, entry.Hours);

            return new JsonNetResult(modificationResult);
        }

        [AcceptPost]
        [Transaction]
        public JsonNetResult RemoveEntry(int entryId)
        {
            var entryRepository = Repository.OfType<TimeRecordEntry>();

            var entry = entryRepository.GetByID(entryId);

            entryRepository.Remove(entry);

            var modificationResult = new TimeRecordEntryModificationDto(entry.Id, -entry.Hours);

            return new JsonNetResult(modificationResult);
        }

        [AcceptPost]
        [Transaction]
        public JsonNetResult EditEntry(int entryId, TimeRecordEntry entry)
        {
            var entryRepository = Repository.OfType<TimeRecordEntry>();

            var entryToUpdate = entryRepository.GetNullableByID(entryId);

            Check.Require(entryToUpdate != null, "Entry not found");

            var originalHours = entryToUpdate.Hours;

            TransferValuesTo(entryToUpdate, entry);

            Check.Require(entryToUpdate.IsValid(), "Entry is not valid");

            entryRepository.EnsurePersistent(entryToUpdate);

            var modificationResult = new TimeRecordEntryModificationDto(entryToUpdate.Id, entryToUpdate.Hours - originalHours);

            return new JsonNetResult(modificationResult);
        }

        [Transaction]
        public JsonNetResult GetEntry(int entryId)
        {
            var entry = Repository.OfType<TimeRecordEntry>().GetNullableByID(entryId);

            Check.Require(entry != null, "Entry not found");

            var result = new
                             {
                                 entry.Hours, 
                                 entry.Id, 
                                 entry.Comment,
                                 ActivityType = entry.ActivityType.Name,
                                 Project = entry.Project.Name,
                                 Account = entry.Account.Name,
                                 FundType = entry.FundType.Name
                             };

            return new JsonNetResult(result);
        }

        [AcceptPost]
        [Transaction]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(int id)
        {
            var timeRecord = _timeRecordRepository.GetNullableByID(id);

            Check.Require(timeRecord != null, "Invalid time record identifier");

            if (!_timeRecordBLL.HasAccess(CurrentUser, timeRecord))
            {
                return RedirectToErrorPage(string.Format("{0} does not have access to submit this time record", CurrentUser.Identity.Name));
            }

            _timeRecordBLL.Submit(timeRecord, CurrentUser);

            Message = string.Format("Time Record for {0:MMMM yyyy} Submitted Successfully", timeRecord.Date);

            return RedirectToAction("History");
        }

        public ViewResult ListOfActivitiesAndDescriptions()
        {
            return View();
        }

        /// <summary>
        /// Transfer the new values from the given entry to the entry to update.
        /// Currently you can only update the comment, hours, and activity type
        /// </summary>
        private static void TransferValuesTo(TimeRecordEntry entryToUpdate, TimeRecordEntry entry)
        {
            entryToUpdate.Comment = entry.Comment;
            entryToUpdate.Hours = entry.Hours;
            entryToUpdate.ActivityType = entry.ActivityType;
        }
    }

    public class TimeRecordEntryModificationDto
    {
        public TimeRecordEntryModificationDto(int entryId, double hoursDelta)
        {
            Id = entryId;
            HoursDelta = hoursDelta;
        }

        public int Id { get; set; }
        public double HoursDelta { get; set; }
    }

    public class TimeRecordReviewViewModel
    {
        public static TimeRecordReviewViewModel Create(TimeRecord timeRecord, IRepository repository)
        {
            var timeRecordEntries = repository.OfType<TimeRecordEntry>().Queryable.Where(x => x.Record.Id == timeRecord.Id).OrderBy(x=>x.Date);

            var viewModel = new TimeRecordReviewViewModel
            {
                TimeRecord = timeRecord,
                TimeRecordEntries = timeRecordEntries.ToList()
            };

            return viewModel;
        }

        public TimeRecord TimeRecord { get; set; }
        public IEnumerable<TimeRecordEntry> TimeRecordEntries { get; set; }
    }


    public class TimeRecordEntryViewModel
    {
        public static TimeRecordEntryViewModel Create(IRepository repository, IUserBLL userBLL, ITimeRecordBLL timeRecordBLL, TimeRecord timeRecord, ITimeRecordCalendarGenerator calendarGenerator)
        {
            var viewModel = new TimeRecordEntryViewModel
                                {
                                    TimeRecord = timeRecord,
                                    TotalHours =
                                        repository.OfType<TimeRecordEntry>()
                                            .Queryable
                                            .Where(x => x.Record.Id == timeRecord.Id)
                                            .Sum(x => x.Hours),
                                    CalendarDays = calendarGenerator.GenerateCalendar(timeRecord),
                                    Projects = userBLL.GetAllProjectsByUser(repository.OfType<Project>()).ToList(),
                                    FundTypes = userBLL.GetUser().FundTypes,
                                    ActivityCategories =
                                        repository.OfType<ActivityCategory>()
                                        .Queryable
                                        .Where(c => c.IsActive)
                                        .OrderBy(c => c.Name)
                                        .ToList(),
                                    AdjustmentEntries =
                                        repository.OfType<TimeRecordEntry>()
                                        .Queryable
                                        .Where(x => x.Record.Id == timeRecord.Id && x.AdjustmentDate != null)
                                        .OrderBy(x => x.AdjustmentDate)
                                        .ToList()
                                };

            return viewModel;
        }

        public TimeRecord TimeRecord { get; set; }
        public double TotalHours { get; set; }
        public IList<TimeRecordCalendarDay> CalendarDays { get; set; }
        public IList<Project> Projects { get; set; }
        public IList<FundType> FundTypes { get; set; }
        public IList<ActivityCategory> ActivityCategories { get; set; }
        public IList<TimeRecordEntry> AdjustmentEntries { get; set; }
    }
}