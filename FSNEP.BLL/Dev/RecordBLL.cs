using System;
using System.Collections.Generic;
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
        private readonly IMessageGateway _messageGateway;
        private readonly ISignatureFactory _signatureFactory;

        public RecordBLL(IRepository repository, IMessageGateway messageGateway, ISignatureFactory signatureFactory)
        {
            _repository = repository;
            _messageGateway = messageGateway;
            _signatureFactory = signatureFactory;
        }

        public virtual bool HasAccess(IPrincipal user, T record)
        {
            Check.Require(record != null);
            return record.User.UserName == user.Identity.Name;
        }

        /// <summary>
        /// True if the user can review this sheet.
        /// This is if they are the user's supervisor or 
        /// A delegate of the supervisor can also access it
        /// </summary>
        public virtual bool HasReviewAccess(IPrincipal user, T record)
        {
            Check.Require(record != null);
            
            if (record.User.Supervisor.UserName == user.Identity.Name)
            {
                return true; //Supervisor can review the record
            }

            var supervisorDelegate = record.User.Supervisor.Delegate;

            if (supervisorDelegate != null && supervisorDelegate.UserName == user.Identity.Name)
            {
                return true; //Supervisor's delegate can review the record as well
            }

            return false; //Default deny
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

        private void PersistRecordWithTracking(T record, IPrincipal user, IRepository repository)
        {
            var signature = _signatureFactory.CreateSignature(record, repository);

            var tracking = new RecordTracking
            {
                ActionDate = SystemTime.Now(),
                Record = record,
                Status = record.Status,
                DigitalSignature = signature,
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

        /// <summary>
        /// Returns all of the reviewable & current records for the given supervisor's reviewees.
        /// </summary>
        /// <remarks>
        /// Criteria for a record being visible
        /// 1) Current user is the supervisor of the record's owner
        /// 2) Current user is a delegate for a supervisor who supervises the record's owner
        /// </remarks>
        /// <param name="user">The user who will review the record</param>
        public virtual IEnumerable<T> GetReviewableAndCurrentRecords(IPrincipal user)
        {
            var reviewableAndCurrentStatuses = new[]
            {
                Status.GetName(Status.Option.PendingReview), 
                Status.GetName(Status.Option.Current)
            };

            var records = _repository.OfType<T>().Queryable
                .Where(x => x.User.Supervisor.UserName == user.Identity.Name
                        || (x.User.Supervisor.Delegate != null && x.User.Supervisor.Delegate.UserName == user.Identity.Name))
                .Where(x => reviewableAndCurrentStatuses.Contains(x.Status.Name))
                .OrderBy(x => x.User.LastName)
                .ThenBy(x => x.Year)
                .ThenBy(x => x.Month);

            return records;
        }

        /// <summary>
        /// Submits the given record
        /// </summary>
        public void Submit(T record, IPrincipal user)
        {
            Check.Require(record.IsEditable, "Record must be have either the current or disapproved status in order to be submitted");

            Status pendingReviewStatus = _repository.OfType<Status>()
                .Queryable.Where(x => x.Name == Status.GetName(Status.Option.PendingReview)).Single();

            record.Status = pendingReviewStatus;//Set the status to "Pending Review" (submitted for review)

            PersistRecordWithTracking(record, user, _repository);

            _messageGateway.SendSupervisorNotificationMessage(record);
        }

        public void ApproveOrDeny(T record, IPrincipal user, bool approve)
        {
            var recordStatusOption = record.Status.NameOption;
            var newStatusName = approve ? Status.GetName(Status.Option.Approved) : Status.GetName(Status.Option.Disapproved);
            
            Check.Require(recordStatusOption == Status.Option.PendingReview,
                          "Record must be have the pending review status in order to be approved or denied");

            Status approveOrDenyStatus = _repository.OfType<Status>()
                .Queryable.Where(x => x.Name == newStatusName).Single();

            record.Status = approveOrDenyStatus;//Set the status to approved or denied

            PersistRecordWithTracking(record, user, _repository);

            //Now send the confirmation message
            _messageGateway.SendReviewMessage(record, approve);
        }
    }
}