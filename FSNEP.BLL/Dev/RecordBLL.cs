using System;
using System.Linq;
using System.Security.Principal;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;
using FSNEP.Core.Abstractions;

namespace FSNEP.BLL.Dev
{
    public class RecordBLL<T> : IRecordBLL<T> where T : Record, new()
    {
        private readonly IRepository _repository;

        public RecordBLL(IRepository repository)
        {
            _repository = repository;
        }

        public bool HasAccess(IPrincipal user, T record)
        {
            Check.Require(record != null);
            return record.User.UserName == user.Identity.Name;
        }

        public bool HasReviewAccess(IPrincipal user, T record)
        {
            Check.Require(record != null);

            if (user.IsInRole(RoleNames.RoleAdmin))
            { //Admins can review any sheet
                return true;
            }

            if (record.User.UserName == user.Identity.Name || record.User.Supervisor.UserName == user.Identity.Name)
            {
                return true; //Current user can always access her own sheet, as can her supervisor
            }

            return false; //Default deny
        }

        public bool IsEditable(T record)
        {
            Check.Require(record != null);

            //if (record.Status.Name == Status.Option.Current.ToString() || record.Status.Name == Status.Option.Disapproved.ToString())
            if (record.Status.NameOption == Status.Option.Current || record.Status.NameOption == Status.Option.Disapproved)
            {
                return true; //editable only if the status is current or disapproved
            }

            return false;
        }

        public T GetCurrent(IPrincipal user)
        {
            //Get the current record for the user.
            //var currentRecord = _repository.OfType<T>()
            //    .Queryable
            //    .Where(x => x.User.UserName == user.Identity.Name)
            //    .Where(x => x.Status.Name == Status.GetName(Status.Option.Current))
            //    .OrderByDescending(x => x.Year)
            //    .ThenByDescending(x => x.Month)
            //    .FirstOrDefault();
            var currentRecord = GetCurrentRecord(user);

            //Create a new timesheet if there is no current,
            // or if it is within the same year and we have passed into a new month (even if there was an old current still active)
            // or if we have passed into a new year, (even if there as an old current still active)
            if (currentRecord == null ||
                ((currentRecord.Year == GetCurrentSheetDate().Year) && (currentRecord.Month < GetCurrentSheetDate().Month)) ||
                (currentRecord.Year < GetCurrentSheetDate().Year))
            {
                return CreateRecord(user);
            }
            else
            {
                return currentRecord;
            }
        }

        /// <summary>
        /// Gets the current record. Split out to make testing easier
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public T GetCurrentRecord(IPrincipal user)
        {
            return _repository.OfType<T>()
                .Queryable
                .Where(x => x.User.UserName == user.Identity.Name)
                .Where(x => x.Status.Name == Status.GetName(Status.Option.Current))
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .FirstOrDefault();
        }

        private T CreateRecord(IPrincipal user)
        {
            DateTime currentDate = GetCurrentSheetDate();

            if (SheetExists(user, currentDate))
            {
                //Does a sheet exist for the next month? (current calendar month)
                if (SheetExists(user, SystemTime.Now()))
                {
                    return default(T); //Don't create the sheet if one already exists
                }

                //Do create a sheet, just using the next month's value
                currentDate = currentDate.AddMonths(1);
            }

            var newRecord = new T
                                {
                                    Year = currentDate.Year,
                                    Month = currentDate.Month,
                                    Status = _repository.OfType<Status>()
                                                .Queryable.Where(x => x.Name == Status.GetName(Status.Option.Current)).Single(),
                                    User = _repository.OfType<User>()
                                                .Queryable.Where(x => x.UserName == user.Identity.Name).Single()
                                };

            if (typeof(T) == typeof(TimeRecord)) (newRecord as TimeRecord).Salary = newRecord.User.Salary;


            //Create the record and track the creation
            PersistRecordWithTracking(newRecord, user, _repository);

            return newRecord;
        }

        private static void PersistRecordWithTracking(T record, IPrincipal user, IRepository repository)
        {
            var tracking = new RecordTracking
            {
                ActionDate = SystemTime.Now(),
                Record = record,
                Status = record.Status,
                UserName = user.Identity.Name
            };

            repository.OfType<T>().EnsurePersistent(record); //persist the record

            repository.OfType<RecordTracking>().EnsurePersistent(tracking); //persist the tracking info along with it
        }

        /// <summary>
        /// Returns true if a record exists for this user and date (year/month)
        /// </summary>
        private bool SheetExists(IPrincipal user, DateTime date)
        {
            return _repository.OfType<T>()
                .Queryable
                .Where(x => x.User.UserName == user.Identity.Name)
                .Where(x => x.Year == date.Year && x.Month == date.Month)
                .Any();
        }

        /// <summary>
        /// Gets the date that should exist for the current sheet.  This contains rules for when a sheet should be created within a given month.
        /// </summary>
        /// <remarks>Only the month and year returns from this function should be used.</remarks>
        public DateTime GetCurrentSheetDate()
        {
            DateTime currentDate = SystemTime.Now();

            if (currentDate.Day <= 30) //if we are in the first 30 days of the month, we want to use the previous month
            {
                currentDate = currentDate.AddMonths(-1);
            }

            return currentDate;
        }
    }
}