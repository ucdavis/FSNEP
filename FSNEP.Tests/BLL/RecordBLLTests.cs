using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using FSNEP.BLL.Dev;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Tests.BLL
{
    [TestClass]
    public class RecordBLLTests
    {
        private IRecordBLL<Record> _recordBLL;
        private IRecordBLL<TimeRecord> _timeRecordBLL;
        private IRecordBLL<CostShare> _costShareBLL;
        private IRepository _repository;
        private IMessageGateway _messageGateway;
        private readonly IPrincipal _principal = MockRepository.GenerateStub<MockPrincipal>();
        private List<Record> Records { get; set; }
        private List<CostShare> CostShareRecords { get; set; }

        private User CurrentUser { get; set; }

        [TestInitialize]
        public void Setup()
        {
            _repository = MockRepository.GenerateStub<IRepository>();
            _messageGateway = MockRepository.GenerateStub<IMessageGateway>();
            _recordBLL = new RecordBLL<Record>(_repository, _messageGateway);
            _timeRecordBLL = new RecordBLL<TimeRecord>(_repository, _messageGateway);
            _costShareBLL = new RecordBLL<CostShare>(_repository, _messageGateway);
            
            CurrentUser = CreateValidUser();
            CurrentUser.UserName = "CurrentUser";
        }

        #region TimeRecord Tests
                   
        #region IsEditable Tests

        [TestMethod]
        public void IsEditableReturnsTrueIfStatusIsCurrent()
        {
            var status = new Status {NameOption = Status.Option.Current};

            //var record = new Record {Status = status};
            var record = CreateValidEntities.Record(null);
            record.Status = status;

            var editable = record.IsEditable;

            Assert.AreEqual(true, editable);
        }

        [TestMethod]
        public void IsEditableReturnsTrueIfStatusIsDisapproved()
        {
            var status = new Status { NameOption = Status.Option.Disapproved };

            //var record = new Record { Status = status };
            var record = CreateValidEntities.Record(null);
            record.Status = status;

            var editable = record.IsEditable;

            Assert.AreEqual(true, editable);
        }

        /// <summary>
        /// Determines whether [is editable returns true if status name is default].
        /// NameOption defaults to Current when the Name isn't a valid enum value.
        /// </summary>
        [TestMethod]
        public void IsEditableReturnsTrueIfStatusNameIsDefault()
        {
            var status = new Status { Name = "Junk data" };

            //var record = new Record { Status = status };
            var record = CreateValidEntities.Record(null);
            record.Status = status;

            var editable = record.IsEditable;

            Assert.AreEqual(true, editable);
        }

        [TestMethod]
        public void IsEditableReturnsFalseIfStatusIsApproved()
        {
            var status = new Status { NameOption = Status.Option.Approved };

            //var record = new Record { Status = status };
            var record = CreateValidEntities.Record(null);
            record.Status = status;

            var editable = record.IsEditable;

            Assert.AreEqual(false, editable);
        }

        [TestMethod]
        public void IsEditableReturnsPendingReviewIfStatusIsPendingReview()
        {
            var status = new Status { NameOption = Status.Option.PendingReview };

            //var record = new Record { Status = status };
            var record = CreateValidEntities.Record(null);
            record.Status = status;

            var editable = record.IsEditable;

            Assert.AreEqual(false, editable);
        }
        #endregion IsEditable Tests

        #region HasAccess Tests

        /// <summary>
        /// Determines whether [has access returns true if record has same name as current user].
        /// </summary>
        [TestMethod]
        public void HasAccessReturnsTrueIfRecordHasSameNameAsCurrentUser()
        {
            //var record = new Record {User = CurrentUser};
            var record = CreateValidEntities.Record(null);
            record.User = CurrentUser;
            Assert.IsTrue(_recordBLL.HasAccess(_principal, record));
        }

        /// <summary>
        /// Determines whether [has access returns false if record has different name as current user].
        /// </summary>
        [TestMethod]
        public void HasAccessReturnsFalseIfRecordHasDifferentNameAsCurrentUser()
        {
            FakeRecordsToCheck();
            Assert.IsFalse(_recordBLL.HasAccess(_principal, Records[1]));
        }

        /// <summary>
        /// Determines whether [has access throws exception if record is null].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void HasAccessThrowsExceptionIfRecordIsNull()
        {
            try
            {
                Assert.IsFalse(_recordBLL.HasAccess(_principal, null));
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Precondition failed.", message.Message);
                throw;
            }
            
        }

        #endregion HasAccess Tests

        #region HasReviewAccess Tests

        /// <summary>
        /// Determines whether [has review access returns true if current user is in record].
        /// </summary>
        [TestMethod]
        public void HasReviewAccessReturnsTrueIfCurrentUserIsInRecord()
        {
            //var record = new Record { User = CurrentUser };
            var record = CreateValidEntities.Record(null);
            record.User = CurrentUser;
            _principal.Expect(a => a.IsInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            Assert.IsTrue(_recordBLL.HasReviewAccess(_principal, record));
        }

        /// <summary>
        /// Determines whether [has review access returns true if current user is users supervisor in record].
        /// </summary>
        [TestMethod]
        public void HasReviewAccessReturnsTrueIfCurrentUserIsUsersSupervisorInRecord()
        {
            var newUser = CreateValidUser();
            newUser.UserName = "NewUser";
            newUser.Supervisor = CurrentUser;

            //var record = new Record { User = newUser }; //We are not passing the current user, but this use has the current user as a supervisor.
            var record = CreateValidEntities.Record(null);
            record.User = newUser;
            _principal.Expect(a => a.IsInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            Assert.IsTrue(_recordBLL.HasReviewAccess(_principal, record));
        }

        /// <summary>
        /// Determines whether [has review access returns fasle if current user is not users supervisor in record].
        /// </summary>
        [TestMethod]
        public void HasReviewAccessReturnsFasleIfCurrentUserIsNotUsersSupervisorInRecord()
        {
            var newUser = CreateValidUser();
            newUser.UserName = "NewUser";
            newUser.Supervisor = CreateValidUser();

            //var record = new Record { User = newUser }; //We are not passing the current user, but this use has the current user as a supervisor.
            var record = CreateValidEntities.Record(null);
            record.User = newUser;
            _principal.Expect(a => a.IsInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            Assert.IsFalse(_recordBLL.HasReviewAccess(_principal, record));
        }

        

        #endregion HasReviewAccess Tests

        #region GetCurrentRecord Tests

        /// <summary>
        /// Get current record returns null when the current user has no records.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsNullWhenTheCurrentUserHasNoRecords()
        {
            FakeRecordsToCheck();

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns the current users record.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsTheCurrentUsersRecord()
        {
            FakeRecordsToCheck();
            var record = CreateValidEntities.Record(null);
            record.Status = new Status {NameOption = Status.Option.Current};
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);
            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status{NameOption = Status.Option.Current},
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test1.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest1()
        {
            FakeRecordsToCheck();
            var record = CreateValidEntities.Record(null);
            record.Status = new Status { NameOption = Status.Option.Approved };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Approved },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test2.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest2()
        {
            FakeRecordsToCheck();
            var record = CreateValidEntities.Record(null);
            record.Status = new Status { NameOption = Status.Option.Disapproved };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Disapproved },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test3.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest3()
        {
            FakeRecordsToCheck();
            var record = CreateValidEntities.Record(null);
            record.Status = new Status { NameOption = Status.Option.PendingReview };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);
            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.PendingReview },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test4.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest4()
        {
            FakeRecordsToCheck();
            var record = CreateValidEntities.Record(null);
            record.Status = new Status { Name = "Junk" };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);
            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { Name= "Junk" },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Gets the current record returns correct record first by date order test1.
        /// If this fails, it may be ok.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsCorrectRecordFirstByDateOrderTest1()
        {
            FakeRecordsToCheck();
            //We expect This one
            var record = CreateValidEntities.Record(null);
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            ////We expect This one
            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Current },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            record = CreateValidEntities.Record(null);
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "OrReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Current },
            //    ReviewComment = "OrReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment); //We expect This one
        }

        /// <summary>
        /// Gets the current record returns correct record first by date order test2.
        /// </summary>
        [TestMethod]
        public void GetCurrentRecordReturnsCorrectRecordFirstByDateOrderTest2()
        {
            FakeRecordsToCheck();

            var record = CreateValidEntities.Record(null);
            record.Month = 12;
            record.Year = 2008;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "NotThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 12,
            //    Year = 2008,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Current },
            //    ReviewComment = "NotThisRecord",
            //    Entries = new List<Entry>()
            //});

            //We expect This one
            record = CreateValidEntities.Record(null);
            record.Month = 10;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);
            //Records.Add(new Record
            //{
            //    Month = 10,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Current },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            record = CreateValidEntities.Record(null);
            record.Month = 09;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "NotThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);
            //Records.Add(new Record
            //{
            //    Month = 09,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Current },
            //    ReviewComment = "NotThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrentRecord(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment); //We expect This one
        }

        #endregion GetCurrentRecord Tests

        #region GetCurrentSheetDate Tests

        /// <summary>
        /// Get current sheet date returns expected date for January 2009.
        /// </summary>
        [TestMethod]
        public void GetCurrentSheetDateReturnsExpectedDateForJanuary2009()
        {
            var january = new DateTime(2008,12,31); //Prime to December 

            for (int i = 0; i < 30; i++)
            {
                january = january.AddDays(1);
                DateTime time = january;
                SystemTime.Now = () => time;

                DateTime result = _recordBLL.GetCurrentSheetDate();
                Assert.IsNotNull(result, "Was null. Position " + i);
                Assert.AreEqual(2008, result.Year, "Year was not 2008. Position " + i);
                Assert.AreEqual(12, result.Month, "Month was not 12. Position " + i);
                Assert.AreEqual(january.Day, result.Day, "Day was not Expected. Position " + i);
            }          
            Assert.AreEqual(1, january.Month);
            Assert.AreEqual(30, january.Day);

            january = january.AddDays(1); //January 31
            SystemTime.Now = () => january;
            DateTime nextResult = _recordBLL.GetCurrentSheetDate();
            Assert.IsNotNull(nextResult);
            Assert.AreEqual(2009, nextResult.Year);
            Assert.AreEqual(01, nextResult.Month);
            Assert.AreEqual(january.Day, nextResult.Day);
        }

        /// <summary>
        /// Get current sheet date returns expected date for February 2009.
        /// All of Feb returns Jan, March 1 returns Feb
        /// </summary>
        [TestMethod]
        public void GetCurrentSheetDateReturnsExpectedDateForFebruary2009()
        {
            var february = new DateTime(2009, 01, 31); //Prime to january 

            for (int i = 0; i < 28; i++) //28 days in Feb 2009
            {
                february = february.AddDays(1);
                DateTime time = february;
                SystemTime.Now = () => time;

                DateTime result = _recordBLL.GetCurrentSheetDate();
                Assert.IsNotNull(result, "Was null. Position " + i);
                Assert.AreEqual(2009, result.Year, "Year was not 2009. Position " + i);
                Assert.AreEqual(1, result.Month, "Month was not 1. Position " + i);
                Assert.AreEqual(february.Day, result.Day, "Day was not Expected. Position " + i);
            }
            Assert.AreEqual(2, february.Month);
            Assert.AreEqual(28, february.Day);

            february = february.AddDays(1); //March 1
            SystemTime.Now = () => february;
            DateTime nextResult = _recordBLL.GetCurrentSheetDate();
            Assert.IsNotNull(nextResult);
            Assert.AreEqual(2009, nextResult.Year);
            Assert.AreEqual(02, nextResult.Month);
            Assert.AreEqual(1, nextResult.Day); //March 1st
        }

        /// <summary>
        /// Get current sheet date returns expected date for march 2009.
        /// </summary>
        [TestMethod]
        public void GetCurrentSheetDateReturnsExpectedDateForMarch2009()
        {
            var march = new DateTime(2009, 02, 28); //Prime to End of Feb 

            for (int i = 0; i < 30; i++) 
            {
                march = march.AddDays(1);
                DateTime time = march;
                SystemTime.Now = () => time;

                DateTime result = _recordBLL.GetCurrentSheetDate();
                Assert.IsNotNull(result, "Was null. Position " + i);
                Assert.AreEqual(2009, result.Year, "Year was not 2009. Position " + i);
                Assert.AreEqual(2, result.Month, "Month was not 2. Position " + i);
                if (march.Day > 28)
                {
                    Assert.AreEqual(28, result.Day, "Day was not Expected. Position " + i);
                }
                else
                {
                    Assert.AreEqual(march.Day, result.Day, "Day was not Expected. Position " + i);
                }
                
            }
            Assert.AreEqual(03, march.Month);
            Assert.AreEqual(30, march.Day);

            march = march.AddDays(1); //March 31
            SystemTime.Now = () => march;
            DateTime nextResult = _recordBLL.GetCurrentSheetDate();
            Assert.IsNotNull(nextResult);
            Assert.AreEqual(2009, nextResult.Year);
            Assert.AreEqual(03, nextResult.Month);
            Assert.AreEqual(march.Day, nextResult.Day); //March 31st
        }

        #endregion GetCurrentSheetDate Tests

        #region GetCurrent Tests

        /// <summary>
        /// Get current returns the existing record.
        /// This is because the sheet date will be 2009/11 
        /// </summary>
        [TestMethod]
        public void GetCurrentReturnsTheExistingRecord1()
        {
            //This should make the month comparison the same for the 2009/11 date in the record below.
            var fakeDate = new DateTime(2009, 12, 01); 
            SystemTime.Now = () => fakeDate;

            FakeRecordsToCheck();
            var record = CreateValidEntities.Record(null);
            record.Month = 11;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 11,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Current },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment);
        }

        /// <summary>
        /// Get current returns the existing record.
        /// This is because the sheet date will be 2009/10 
        /// (The last day of the month 31, will allow a record to be created for the next month )
        /// </summary>
        [TestMethod]
        public void GetCurrentReturnsTheExistingRecord2()
        {
            //This should make the month comparison less than the 2009/11 date in the record below.
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeRecordsToCheck();
            var record = CreateValidEntities.Record(null);
            record.Month = 11;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 11,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Current },
            //    ReviewComment = "ReturnThisRecord",
            //    Entries = new List<Entry>()
            //});

            var currentRecord = _recordBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment);
        }

        

        /// <summary>
        /// Get current creates and returns A new record.
        /// Date has current month because "current date" is the 31st
        /// </summary>
        [TestMethod]
        public void GetCurrentCreatesAndReturnsANewRecord1()
        {
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeRecordsToCheck(); //No records for the current user.

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _recordBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<Record>().AssertWasCalled(a => a.EnsurePersistent(Arg<Record>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(10, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
        }

        /// <summary>
        /// Gets the current creates and returns A new time record.
        /// New Time Record has salary of current user.
        /// </summary>
        [TestMethod]
        public void GetCurrentCreatesAndReturnsANewTimeRecord()
        {
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeTimeRecordsToCheck(); //No records for the current user.

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _timeRecordBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<TimeRecord>().AssertWasCalled(a => a.EnsurePersistent(Arg<TimeRecord>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(10, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
            Assert.AreEqual(CurrentUser.Salary, currentRecord.Salary);
        }


        /// <summary>
        /// Get current creates and returns A new record.
        /// Date has previous month because "Current date" is less than the 30th
        /// </summary>
        [TestMethod]
        public void GetCurrentCreatesAndReturnsANewRecord2()
        {
            var fakeDate = new DateTime(2009, 10, 25);
            SystemTime.Now = () => fakeDate;

            FakeRecordsToCheck(); //No records for the current user.

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _recordBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<Record>().AssertWasCalled(a => a.EnsurePersistent(Arg<Record>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(09, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
            Assert.IsNull(currentRecord.ReviewComment); //Because it is new
        }

        /// <summary>
        /// Get current creates and returns null because a sheet for that already exists (Pending review).
        /// </summary>
        [TestMethod]
        public void GetCurrentCreatesAndReturnsANewRecord3()
        {
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeRecordsToCheck(); //No records for the current user.
            var record = CreateValidEntities.Record(null);
            record.Month = 09;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Approved };
            record.ReviewComment = "ReturnThisRecord1";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 09,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Approved },
            //    ReviewComment = "ReturnThisRecord1",
            //    Entries = new List<Entry>()
            //});

            record = CreateValidEntities.Record(null);
            record.Month = 10;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.PendingReview };
            record.ReviewComment = "ReturnThisRecord2";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);
            //Records.Add(new Record
            //{
            //    Month = 10,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.PendingReview },
            //    ReviewComment = "ReturnThisRecord2",
            //    Entries = new List<Entry>()
            //});
         

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _recordBLL.GetCurrent(_principal);
            Assert.IsNull(currentRecord);
            recordTrackingRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<Record>().AssertWasNotCalled(a => a.EnsurePersistent(Arg<Record>.Is.Anything));
        }

        /// <summary>
        /// Get current creates and returns a new record because a sheet for that 
        /// month does not yet exist, but one for the previous month does exist, 
        /// but it is pending review an not editable.
        /// </summary>
        [TestMethod]
        public void GetCurrentCreatesAndReturnsANewRecord4()
        {
            var fakeDate = new DateTime(2009, 11, 01);
            SystemTime.Now = () => fakeDate;

            FakeRecordsToCheck(); //No records for the current user.
            var record = CreateValidEntities.Record(null);
            record.Month = 09;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Approved };
            record.ReviewComment = "ReturnThisRecord1";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 09,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.Approved },
            //    ReviewComment = "ReturnThisRecord1",
            //    Entries = new List<Entry>()
            //});
            record = CreateValidEntities.Record(null);
            record.Month = 10;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.PendingReview };
            record.ReviewComment = "ReturnThisRecord2";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            Records.Add(record);

            //Records.Add(new Record
            //{
            //    Month = 10,
            //    Year = 2009,
            //    User = CurrentUser,
            //    Status = new Status { NameOption = Status.Option.PendingReview },
            //    ReviewComment = "ReturnThisRecord2",
            //    Entries = new List<Entry>()
            //});


            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _recordBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<Record>().AssertWasCalled(a => a.EnsurePersistent(Arg<Record>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(11, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
            Assert.IsNull(currentRecord.ReviewComment); //Because it is new
        }
 

        #endregion GetCurrent Tests

        #region Submit Tests

        /// <summary>
        /// Submit throws exception when status is pending review.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void SubmitThrowsExceptionWhenStatusIsPendingReviewForRecord()
        {
            try
            {
                var record = CreateValidEntities.Record(null);
                record.Status = new Status{NameOption = Status.Option.PendingReview};
                _recordBLL.Submit(record, _principal);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Record must be have either the current or disapproved status in order to be submitted"
                    , message.Message);
                throw;
            }
        }

        /// <summary>
        /// Submit throws exception when status is pending review.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void SubmitThrowsExceptionWhenStatusIsPendingReviewForTimeRecord()
        {
            try
            {
                var record = CreateValidEntities.TimeRecord(null);
                record.Status = new Status { NameOption = Status.Option.PendingReview };
                record.User = CurrentUser;
                _timeRecordBLL.Submit(record, _principal);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Record must be have either the current or disapproved status in order to be submitted"
                    , message.Message);
                throw;
            }
        }

        /// <summary>
        /// Submit throws exception when status is approved for time record.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void SubmitThrowsExceptionWhenStatusIsApprovedForTimeRecord()
        {
            try
            {
                var record = CreateValidEntities.TimeRecord(null);
                record.Status = new Status { NameOption = Status.Option.Approved };
                record.User = CurrentUser;
                _timeRecordBLL.Submit(record, _principal);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Record must be have either the current or disapproved status in order to be submitted"
                    , message.Message);
                throw;
            }
        }

        /// <summary>
        /// Submit for time record sets status to pending review and persist record with tracking.
        /// </summary>
        [TestMethod]
        public void SubmitForTimeRecordSetsStatusToPendingReviewAndPersistRecordWithTracking()
        {
            var record = CreateValidEntities.TimeRecord(null);
            record.Status = new Status { NameOption = Status.Option.Current };
            record.User = CurrentUser;

            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();
            var timeRecordRepository = MockRepository.GenerateStub<IRepository<TimeRecord>>();
            _repository.Expect(a => a.OfType<TimeRecord>()).Return(timeRecordRepository).Repeat.Any();

            FakeStatusQuery();

            _timeRecordBLL.Submit(record, _principal);            

            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<TimeRecord>().AssertWasCalled(a => a.EnsurePersistent(Arg<TimeRecord>.Is.Anything));

            Assert.AreEqual(Status.Option.PendingReview, record.Status.NameOption);
        }

        #endregion Submit Tests

        #region GetReviewableAndCurrentRecords Tests

        /// <summary>
        /// Get reviewable and current records returns only expected records in correct order for time record.
        /// </summary>
        [TestMethod]
        public void GetReviewableAndCurrentRecordsReturnsOnlyExpectedRecordsInCorrectOrderForTimeRecord()
        {
            FakeTimeRecordsToCheckWithGetReviewableAndCurrentRecords();
            var timeRecords = _timeRecordBLL.GetReviewableAndCurrentRecords(_principal);
            Assert.IsNotNull(timeRecords);
            Assert.AreEqual(9, timeRecords.Count());
            foreach (var timeRecord in timeRecords)
            {
                Assert.AreEqual(CurrentUser, timeRecord.User.Supervisor);
            }
            var timeRecordList = timeRecords.ToList();
            //Abby
            Assert.AreEqual("ReviewComment13", timeRecordList[0].ReviewComment);
            Assert.AreEqual("ReviewComment12", timeRecordList[1].ReviewComment);
            Assert.AreEqual("ReviewComment11", timeRecordList[2].ReviewComment);
            //Chancy
            Assert.AreEqual("ReviewComment7", timeRecordList[3].ReviewComment);
            Assert.AreEqual("ReviewComment5", timeRecordList[4].ReviewComment);
            Assert.AreEqual("ReviewComment6", timeRecordList[5].ReviewComment);
            //Zeb
            Assert.AreEqual("ReviewComment8", timeRecordList[6].ReviewComment);
            Assert.AreEqual("ReviewComment9", timeRecordList[7].ReviewComment);
            Assert.AreEqual("ReviewComment10", timeRecordList[8].ReviewComment);            
        }            

        #endregion GetReviewableAndCurrentRecords Tests

        #endregion TimeRecord Tests

        #region CostShare Tests

        #region IsEditable Tests

        [TestMethod]
        public void CostShareIsEditableReturnsTrueIfStatusIsCurrent()
        {
            var status = new Status { NameOption = Status.Option.Current };

            //var record = new Record {Status = status};
            var costShare = CreateValidEntities.CostShare(null);
            costShare.Status = status;

            var editable = costShare.IsEditable;

            Assert.AreEqual(true, editable);
        }
        
        [TestMethod]
        public void CostShareIsEditableReturnsTrueIfStatusIsDisapproved()
        {
            var status = new Status { NameOption = Status.Option.Disapproved };

            //var record = new Record { Status = status };
            var costShare = CreateValidEntities.CostShare(null);
            costShare.Status = status;

            var editable = costShare.IsEditable;

            Assert.AreEqual(true, editable);
        }

        /// <summary>
        /// Determines whether [is editable returns true if status name is default].
        /// NameOption defaults to Current when the Name isn't a valid enum value.
        /// </summary>
        [TestMethod]
        public void CostShareIsEditableReturnsTrueIfStatusNameIsDefault()
        {
            var status = new Status { Name = "Junk data" };

            //var record = new Record { Status = status };
            var costShare = CreateValidEntities.CostShare(null);
            costShare.Status = status;

            var editable = costShare.IsEditable;

            Assert.AreEqual(true, editable);
        }

        [TestMethod]
        public void CostShareIsEditableReturnsFalseIfStatusIsApproved()
        {
            var status = new Status { NameOption = Status.Option.Approved };

            //var record = new Record { Status = status };
            var costShare = CreateValidEntities.CostShare(null);
            costShare.Status = status;

            var editable = costShare.IsEditable;

            Assert.AreEqual(false, editable);
        }

        [TestMethod]
        public void CostShareIsEditableReturnsPendingReviewIfStatusIsPendingReview()
        {
            var status = new Status { NameOption = Status.Option.PendingReview };

            //var record = new Record { Status = status };
            var costShare = CreateValidEntities.CostShare(null);
            costShare.Status = status;

            var editable = costShare.IsEditable;

            Assert.AreEqual(false, editable);
        }
         
        #endregion IsEditable Tests

        #region HasAccess Tests

        /// <summary>
        /// Determines whether [has access returns true if record has same name as current user].
        /// </summary>
        [TestMethod]
        public void CostShareHasAccessReturnsTrueIfRecordHasSameNameAsCurrentUser()
        {
            //var record = new Record {User = CurrentUser};
            var record = CreateValidEntities.CostShare(null);
            record.User = CurrentUser;
            Assert.IsTrue(_recordBLL.HasAccess(_principal, record));
        }

        /// <summary>
        /// Determines whether [has access returns false if record has different name as current user].
        /// </summary>
        [TestMethod]
        public void CostShareHasAccessReturnsFalseIfRecordHasDifferentNameAsCurrentUser()
        {
            FakeCostShareRecordsToCheck();
            Assert.IsFalse(_costShareBLL.HasAccess(_principal, CostShareRecords[1]));
        }
        
        /// <summary>
        /// Determines whether [has access throws exception if record is null].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void CostShareHasAccessThrowsExceptionIfRecordIsNull()
        {
            try
            {
                Assert.IsFalse(_costShareBLL.HasAccess(_principal, null));
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Precondition failed.", message.Message);
                throw;
            }

        }
        
        #endregion HasAccess Tests

        #region HasReviewAccess Tests

        /// <summary>
        /// Determines whether [has review access returns true if current user is in record].
        /// </summary>
        [TestMethod]
        public void CostShareHasReviewAccessReturnsTrueIfCurrentUserIsInRecord()
        {
            //var record = new Record { User = CurrentUser };
            var record = CreateValidEntities.CostShare(null);
            record.User = CurrentUser;
            _principal.Expect(a => a.IsInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            Assert.IsTrue(_costShareBLL.HasReviewAccess(_principal, record));
        }

        /// <summary>
        /// Determines whether [has review access returns true if current user is users supervisor in record].
        /// </summary>
        [TestMethod]
        public void CostShareHasReviewAccessReturnsTrueIfCurrentUserIsUsersSupervisorInRecord()
        {
            var newUser = CreateValidUser();
            newUser.UserName = "NewUser";
            newUser.Supervisor = CurrentUser;

            //var record = new Record { User = newUser }; //We are not passing the current user, but this use has the current user as a supervisor.
            var record = CreateValidEntities.CostShare(null);
            record.User = newUser;
            _principal.Expect(a => a.IsInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            Assert.IsTrue(_costShareBLL.HasReviewAccess(_principal, record));
        }

        /// <summary>
        /// Determines whether [has review access returns fasle if current user is not users supervisor in record].
        /// </summary>
        [TestMethod]
        public void CostShareHasReviewAccessReturnsFasleIfCurrentUserIsNotUsersSupervisorInRecord()
        {
            var newUser = CreateValidUser();
            newUser.UserName = "NewUser";
            newUser.Supervisor = CreateValidUser();

            //var record = new Record { User = newUser }; //We are not passing the current user, but this use has the current user as a supervisor.
            var record = CreateValidEntities.CostShare(null);
            record.User = newUser;
            _principal.Expect(a => a.IsInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            Assert.IsFalse(_costShareBLL.HasReviewAccess(_principal, record));
        }

        #endregion HasReviewAccess Tests

        #region GetCurrentRecord Tests

        /// <summary>
        /// Get current record returns null when the current user has no records.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsNullWhenTheCurrentUserHasNoRecords()
        {
            FakeCostShareRecordsToCheck();

            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns the current users record.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsTheCurrentUsersRecord()
        {
            FakeCostShareRecordsToCheck();
            var record = CreateValidEntities.CostShare(null);
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test1.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest1()
        {
            FakeCostShareRecordsToCheck();
            var record = CreateValidEntities.CostShare(null);
            record.Status = new Status { NameOption = Status.Option.Approved };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test2.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest2()
        {
            FakeCostShareRecordsToCheck();
            var record = CreateValidEntities.CostShare(null);
            record.Status = new Status { NameOption = Status.Option.Disapproved };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test3.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest3()
        {
            FakeCostShareRecordsToCheck();
            var record = CreateValidEntities.CostShare(null);
            record.Status = new Status { NameOption = Status.Option.PendingReview };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);
            
            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Get current record returns null when current users record status is not current test4.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsNullWhenCurrentUsersRecordStatusIsNotCurrentTest4()
        {
            FakeCostShareRecordsToCheck();
            var record = CreateValidEntities.CostShare(null);
            record.Status = new Status { Name = "Junk" };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);
         
            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNull(currentRecord);
        }

        /// <summary>
        /// Gets the current record returns correct record first by date order test1.
        /// If this fails, it may be ok.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsCorrectRecordFirstByDateOrderTest1()
        {
            FakeCostShareRecordsToCheck();
            //We expect This one
            var record = CreateValidEntities.CostShare(null);
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            record = CreateValidEntities.CostShare(null);
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "OrReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
  
            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment); //We expect This one
        }

        /// <summary>
        /// Gets the current record returns correct record first by date order test2.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentRecordReturnsCorrectRecordFirstByDateOrderTest2()
        {
            FakeCostShareRecordsToCheck();

            var record = CreateValidEntities.CostShare(null);
            record.Month = 12;
            record.Year = 2008;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "NotThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            //We expect This one
            record = CreateValidEntities.CostShare(null);
            record.Month = 10;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            record = CreateValidEntities.CostShare(null);
            record.Month = 09;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "NotThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);
         
            var currentRecord = _costShareBLL.GetCurrentRecord(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment); //We expect This one
        }

        #endregion GetCurrentRecord Tests

        #region GetCurrentSheetDate Tests

        /// <summary>
        /// Get current sheet date returns expected date for January 2009.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentSheetDateReturnsExpectedDateForJanuary2009()
        {
            var january = new DateTime(2008, 12, 31); //Prime to December 

            for (int i = 0; i < 30; i++)
            {
                january = january.AddDays(1);
                DateTime time = january;
                SystemTime.Now = () => time;

                DateTime result = _costShareBLL.GetCurrentSheetDate();
                Assert.IsNotNull(result, "Was null. Position " + i);
                Assert.AreEqual(2008, result.Year, "Year was not 2008. Position " + i);
                Assert.AreEqual(12, result.Month, "Month was not 12. Position " + i);
                Assert.AreEqual(january.Day, result.Day, "Day was not Expected. Position " + i);
            }
            Assert.AreEqual(1, january.Month);
            Assert.AreEqual(30, january.Day);

            january = january.AddDays(1); //January 31
            SystemTime.Now = () => january;
            DateTime nextResult = _costShareBLL.GetCurrentSheetDate();
            Assert.IsNotNull(nextResult);
            Assert.AreEqual(2009, nextResult.Year);
            Assert.AreEqual(01, nextResult.Month);
            Assert.AreEqual(january.Day, nextResult.Day);
        }
        
        /// <summary>
        /// Get current sheet date returns expected date for February 2009.
        /// All of Feb returns Jan, March 1 returns Feb
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentSheetDateReturnsExpectedDateForFebruary2009()
        {
            var february = new DateTime(2009, 01, 31); //Prime to january 

            for (int i = 0; i < 28; i++) //28 days in Feb 2009
            {
                february = february.AddDays(1);
                DateTime time = february;
                SystemTime.Now = () => time;

                DateTime result = _costShareBLL.GetCurrentSheetDate();
                Assert.IsNotNull(result, "Was null. Position " + i);
                Assert.AreEqual(2009, result.Year, "Year was not 2009. Position " + i);
                Assert.AreEqual(1, result.Month, "Month was not 1. Position " + i);
                Assert.AreEqual(february.Day, result.Day, "Day was not Expected. Position " + i);
            }
            Assert.AreEqual(2, february.Month);
            Assert.AreEqual(28, february.Day);

            february = february.AddDays(1); //March 1
            SystemTime.Now = () => february;
            DateTime nextResult = _costShareBLL.GetCurrentSheetDate();
            Assert.IsNotNull(nextResult);
            Assert.AreEqual(2009, nextResult.Year);
            Assert.AreEqual(02, nextResult.Month);
            Assert.AreEqual(1, nextResult.Day); //March 1st
        }
        
        /// <summary>
        /// Get current sheet date returns expected date for march 2009.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentSheetDateReturnsExpectedDateForMarch2009()
        {
            var march = new DateTime(2009, 02, 28); //Prime to End of Feb 

            for (int i = 0; i < 30; i++)
            {
                march = march.AddDays(1);
                DateTime time = march;
                SystemTime.Now = () => time;

                DateTime result = _costShareBLL.GetCurrentSheetDate();
                Assert.IsNotNull(result, "Was null. Position " + i);
                Assert.AreEqual(2009, result.Year, "Year was not 2009. Position " + i);
                Assert.AreEqual(2, result.Month, "Month was not 2. Position " + i);
                if (march.Day > 28)
                {
                    Assert.AreEqual(28, result.Day, "Day was not Expected. Position " + i);
                }
                else
                {
                    Assert.AreEqual(march.Day, result.Day, "Day was not Expected. Position " + i);
                }

            }
            Assert.AreEqual(03, march.Month);
            Assert.AreEqual(30, march.Day);

            march = march.AddDays(1); //March 31
            SystemTime.Now = () => march;
            DateTime nextResult = _costShareBLL.GetCurrentSheetDate();
            Assert.IsNotNull(nextResult);
            Assert.AreEqual(2009, nextResult.Year);
            Assert.AreEqual(03, nextResult.Month);
            Assert.AreEqual(march.Day, nextResult.Day); //March 31st
        }
      
        #endregion GetCurrentSheetDate Tests

        #region GetCurrent Tests

        /// <summary>
        /// Get current returns the existing record.
        /// This is because the sheet date will be 2009/11 
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentReturnsTheExistingRecord1()
        {
            //This should make the month comparison the same for the 2009/11 date in the record below.
            var fakeDate = new DateTime(2009, 12, 01);
            SystemTime.Now = () => fakeDate;

            FakeCostShareRecordsToCheck();
            var record = CreateValidEntities.CostShare(null);
            record.Month = 11;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            var currentRecord = _costShareBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment);
        }
        
        /// <summary>
        /// Get current returns the existing record.
        /// This is because the sheet date will be 2009/10 
        /// (The last day of the month 31, will allow a record to be created for the next month )
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentReturnsTheExistingRecord2()
        {
            //This should make the month comparison less than the 2009/11 date in the record below.
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeCostShareRecordsToCheck();
            var record = CreateValidEntities.CostShare(null);
            record.Month = 11;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Current };
            record.ReviewComment = "ReturnThisRecord";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            var currentRecord = _costShareBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            Assert.AreEqual("ReturnThisRecord", currentRecord.ReviewComment);
        }



        /// <summary>
        /// Get current creates and returns A new record.
        /// Date has current month because "current date" is the 31st
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentCreatesAndReturnsANewRecord1()
        {
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeCostShareRecordsToCheck(); //No records for the current user.

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _costShareBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<CostShare>().AssertWasCalled(a => a.EnsurePersistent(Arg<CostShare>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(10, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
        }
        
        /// <summary>
        /// Gets the current creates and returns A new time record.
        /// New Time Record has salary of current user.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentCreatesAndReturnsANewTimeRecord()
        {
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeCostShareRecordsToCheck(); //No records for the current user.

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _costShareBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<CostShare>().AssertWasCalled(a => a.EnsurePersistent(Arg<CostShare>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(10, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
        }


        /// <summary>
        /// Get current creates and returns A new record.
        /// Date has previous month because "Current date" is less than the 30th
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentCreatesAndReturnsANewRecord2()
        {
            var fakeDate = new DateTime(2009, 10, 25);
            SystemTime.Now = () => fakeDate;

            FakeCostShareRecordsToCheck(); //No records for the current user.

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _costShareBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<CostShare>().AssertWasCalled(a => a.EnsurePersistent(Arg<CostShare>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(09, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
            Assert.IsNull(currentRecord.ReviewComment); //Because it is new
        }

        /// <summary>
        /// Get current creates and returns null because a sheet for that already exists (Pending review).
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentCreatesAndReturnsANewRecord3()
        {
            var fakeDate = new DateTime(2009, 10, 31);
            SystemTime.Now = () => fakeDate;

            FakeCostShareRecordsToCheck(); //No records for the current user.
            var record = CreateValidEntities.CostShare(null);
            record.Month = 09;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Approved };
            record.ReviewComment = "ReturnThisRecord1";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            record = CreateValidEntities.CostShare(null);
            record.Month = 10;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.PendingReview };
            record.ReviewComment = "ReturnThisRecord2";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);
      
            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _costShareBLL.GetCurrent(_principal);
            Assert.IsNull(currentRecord);
            recordTrackingRepository.AssertWasNotCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<CostShare>().AssertWasNotCalled(a => a.EnsurePersistent(Arg<CostShare>.Is.Anything));
        }

        /// <summary>
        /// Get current creates and returns a new record because a sheet for that 
        /// month does not yet exist, but one for the previous month does exist, 
        /// but it is pending review an not editable.
        /// </summary>
        [TestMethod]
        public void CostShareGetCurrentCreatesAndReturnsANewRecord4()
        {
            var fakeDate = new DateTime(2009, 11, 01);
            SystemTime.Now = () => fakeDate;

            FakeCostShareRecordsToCheck(); //No records for the current user.
            var record = CreateValidEntities.CostShare(null);
            record.Month = 09;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.Approved };
            record.ReviewComment = "ReturnThisRecord1";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            record = CreateValidEntities.CostShare(null);
            record.Month = 10;
            record.Year = 2009;
            record.Status = new Status { NameOption = Status.Option.PendingReview };
            record.ReviewComment = "ReturnThisRecord2";
            record.Entries = new List<Entry>();
            record.User = CurrentUser;
            CostShareRecords.Add(record);

            FakeStatusQuery();
            FakeUserQuery();
            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();

            var currentRecord = _costShareBLL.GetCurrent(_principal);
            Assert.IsNotNull(currentRecord);
            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<CostShare>().AssertWasCalled(a => a.EnsurePersistent(Arg<CostShare>.Is.Anything));
            Assert.AreEqual(CurrentUser, currentRecord.User);
            Assert.AreEqual(Status.Option.Current, currentRecord.Status.NameOption);
            Assert.AreEqual(11, currentRecord.Month);
            Assert.AreEqual(2009, currentRecord.Year);
            Assert.IsNull(currentRecord.ReviewComment); //Because it is new
        }
  
        #endregion GetCurrent Tests

        #region Submit Tests

        /// <summary>
        /// Submit throws exception when status is pending review.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void SubmitThrowsExceptionWhenStatusIsPendingReviewForCostShare()
        {
            try
            {
                var record = CreateValidEntities.CostShare(null);
                record.Status = new Status { NameOption = Status.Option.PendingReview };
                record.User = CurrentUser;
                _costShareBLL.Submit(record, _principal);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Record must be have either the current or disapproved status in order to be submitted"
                    , message.Message);
                throw;
            }
        }
        
        /// <summary>
        /// Submit throws exception when status is approved for cost share.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void SubmitThrowsExceptionWhenStatusIsApprovedForCostShare()
        {
            try
            {
                var record = CreateValidEntities.CostShare(null);
                record.Status = new Status { NameOption = Status.Option.Approved };
                record.User = CurrentUser;
                _costShareBLL.Submit(record, _principal);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Record must be have either the current or disapproved status in order to be submitted"
                    , message.Message);
                throw;
            }
        }

        /// <summary>
        /// Submit for time record sets status to pending review and persist record with tracking.
        /// </summary>
        [TestMethod]
        public void SubmitForcostShareSetsStatusToPendingReviewAndPersistRecordWithTracking()
        {
            var record = CreateValidEntities.CostShare(null);
            record.Status = new Status { NameOption = Status.Option.Current };
            record.User = CurrentUser;

            var recordTrackingRepository = MockRepository.GenerateStub<IRepository<RecordTracking>>();
            _repository.Expect(a => a.OfType<RecordTracking>()).Return(recordTrackingRepository).Repeat.Any();
            var costShareRepository = MockRepository.GenerateStub<IRepository<CostShare>>();
            _repository.Expect(a => a.OfType<CostShare>()).Return(costShareRepository).Repeat.Any();

            FakeStatusQuery();

            _costShareBLL.Submit(record, _principal);

            recordTrackingRepository.AssertWasCalled(a => a.EnsurePersistent(Arg<RecordTracking>.Is.Anything));
            _repository.OfType<CostShare>().AssertWasCalled(a => a.EnsurePersistent(Arg<CostShare>.Is.Anything));

            Assert.AreEqual(Status.Option.PendingReview, record.Status.NameOption);
        }

        #endregion Submit Tests

        #region GetReviewableAndCurrentRecords Tests

        /// <summary>
        /// Get reviewable and current records returns only expected records in correct order for cost share.
        /// </summary>
        [TestMethod]
        public void GetReviewableAndCurrentRecordsReturnsOnlyExpectedRecordsInCorrectOrderForCostShare()
        {
            FakeCostShareRecordsToCheckWithGetReviewableAndCurrentRecords();
            var costShareRecords = _costShareBLL.GetReviewableAndCurrentRecords(_principal);
            Assert.IsNotNull(costShareRecords);
            Assert.AreEqual(9, costShareRecords.Count());
            foreach (var timeRecord in costShareRecords)
            {
                Assert.AreEqual(CurrentUser, timeRecord.User.Supervisor);
            }
            var costShareRecordList = costShareRecords.ToList();
            //Abby
            Assert.AreEqual("ReviewComment13", costShareRecordList[0].ReviewComment);
            Assert.AreEqual("ReviewComment12", costShareRecordList[1].ReviewComment);
            Assert.AreEqual("ReviewComment11", costShareRecordList[2].ReviewComment);
            //Chancy
            Assert.AreEqual("ReviewComment7", costShareRecordList[3].ReviewComment);
            Assert.AreEqual("ReviewComment5", costShareRecordList[4].ReviewComment);
            Assert.AreEqual("ReviewComment6", costShareRecordList[5].ReviewComment);
            //Zeb
            Assert.AreEqual("ReviewComment8", costShareRecordList[6].ReviewComment);
            Assert.AreEqual("ReviewComment9", costShareRecordList[7].ReviewComment);
            Assert.AreEqual("ReviewComment10", costShareRecordList[8].ReviewComment);
        }

        #endregion GetReviewableAndCurrentRecords Tests

        //WIP: Other CostShare Tests

        #endregion CostShare Tests

        #region Helper Methods

        private void FakeUserQuery()
        {
            var nonCurrentUser = CreateValidUser();
            nonCurrentUser.UserName = "NonCurrent";
            var userRepository = MockRepository.GenerateStub<IRepository<User>>();
            var users = new List<User> {CurrentUser, nonCurrentUser};
            _repository.Expect(a => a.OfType<User>()).Return(userRepository).Repeat.Any();
            userRepository.Expect(a => a.Queryable).Return(users.AsQueryable()).Repeat.Any();
        }

        private void FakeStatusQuery()
        {
            var status = new List<Status>
                             {
                                 new Status {NameOption = Status.Option.Current},
                                 new Status {NameOption = Status.Option.Approved},
                                 new Status {NameOption = Status.Option.Disapproved},
                                 new Status {NameOption = Status.Option.PendingReview}
                             };


            var statusRepository = MockRepository.GenerateStub<IRepository<Status>>();
            _repository.Expect(a => a.OfType<Status>()).Return(statusRepository).Repeat.Any();
            statusRepository.Expect(a => a.Queryable).Return(status.AsQueryable()).Repeat.Any();
        }

        private void FakeRecordsToCheck()
        {
            var nonCurrentUser = CreateValidUser();
            nonCurrentUser.UserName = "NonCurrent";

            var statusCurrent = new Status {NameOption = Status.Option.Current};
            var statusApproved = new Status { NameOption = Status.Option.Approved };
            var statusDisapproved = new Status { NameOption = Status.Option.Disapproved };
            var statusPendingReview = new Status { NameOption = Status.Option.PendingReview };

            Records = new List<Record>();

            for (int i = 0; i < 5; i++)
            {
                var record = CreateValidEntities.Record(i+1);
                record.Month = 12;
                record.Year = 2009;
                record.Status = statusCurrent;
                record.Entries = new List<Entry>();
                record.User = nonCurrentUser;
                Records.Add(record);

                //Records.Add(new Record
                //                 {
                //                     Month = 12,
                //                     Year = 2009,
                //                     User = nonCurrentUser,
                //                     Status = statusCurrent,
                //                     ReviewComment = "Comment" + (i + 1),
                //                     Entries = new List<Entry>()
                //                 });
            }
            Records[1].Status = statusApproved;
            Records[2].Status = statusDisapproved;
            Records[3].Status = statusPendingReview;

            var recordRepository = MockRepository.GenerateStub<IRepository<Record>>();
            _repository.Expect(a => a.OfType<Record>()).Return(recordRepository).Repeat.Any();
            recordRepository.Expect(a => a.Queryable).Return(Records.AsQueryable()).Repeat.Any();
        }

        private void FakeCostShareRecordsToCheck()
        {
            var nonCurrentUser = CreateValidUser();
            nonCurrentUser.UserName = "NonCurrent";

            var statusCurrent = new Status { NameOption = Status.Option.Current };
            var statusApproved = new Status { NameOption = Status.Option.Approved };
            var statusDisapproved = new Status { NameOption = Status.Option.Disapproved };
            var statusPendingReview = new Status { NameOption = Status.Option.PendingReview };

            CostShareRecords = new List<CostShare>();

            for (int i = 0; i < 5; i++)
            {
                var record = CreateValidEntities.CostShare(i + 1);
                record.Month = 12;
                record.Year = 2009;
                record.Status = statusCurrent;
                record.Entries = new List<Entry>();
                record.User = nonCurrentUser;
                CostShareRecords.Add(record);

            }
            CostShareRecords[1].Status = statusApproved;
            CostShareRecords[2].Status = statusDisapproved;
            CostShareRecords[3].Status = statusPendingReview;

            var costShareRepository = MockRepository.GenerateStub<IRepository<CostShare>>();
            _repository.Expect(a => a.OfType<CostShare>()).Return(costShareRepository).Repeat.Any();
            costShareRepository.Expect(a => a.Queryable).Return(CostShareRecords.AsQueryable()).Repeat.Any();
        }

        private void FakeTimeRecordsToCheck()
        {
            var nonCurrentUser = CreateValidUser();
            nonCurrentUser.UserName = "NonCurrent";

            var statusCurrent = new Status { NameOption = Status.Option.Current };
            var statusApproved = new Status { NameOption = Status.Option.Approved };
            var statusDisapproved = new Status { NameOption = Status.Option.Disapproved };
            var statusPendingReview = new Status { NameOption = Status.Option.PendingReview };

            var timeRecords = new List<TimeRecord>();

            for (int i = 0; i < 5; i++)
            {
                timeRecords.Add(new TimeRecord
                {
                    Month = 12,
                    Year = 2009,
                    User = nonCurrentUser,
                    Status = statusCurrent,
                    ReviewComment = "Comment" + (i + 1),
                    Entries = new List<Entry>(),
                    Salary = 1
                });
            }
            timeRecords[1].Status = statusApproved;
            timeRecords[2].Status = statusDisapproved;
            timeRecords[3].Status = statusPendingReview;

            var timeRecordRepository = MockRepository.GenerateStub<IRepository<TimeRecord>>();
            _repository.Expect(a => a.OfType<TimeRecord>()).Return(timeRecordRepository).Repeat.Any();
            timeRecordRepository.Expect(a => a.Queryable).Return(timeRecords.AsQueryable()).Repeat.Any();
        }

        /// <summary>
        /// Fakes the time records to check for getReviewableAndCurrentRecords method.
        /// </summary>
        private void FakeTimeRecordsToCheckWithGetReviewableAndCurrentRecords()
        {
            var differentSupervisor = CreateValidEntities.User(null);
            differentSupervisor.UserName = "SomeOtherSupervisor";
            var userList = new List<User>();
            for (int i = 0; i < 4; i++)
            {
                userList.Add(CreateValidEntities.User(i+1));
                userList[i].Supervisor = CurrentUser;
            }
            userList[1].Supervisor = differentSupervisor;

            userList[3].LastName = "Abby";            
            userList[0].LastName = "Chancy";
            userList[2].LastName = "Zeb";

            var statusCurrent = new Status { NameOption = Status.Option.Current };
            var statusApproved = new Status { NameOption = Status.Option.Approved };
            var statusDisapproved = new Status { NameOption = Status.Option.Disapproved };
            var statusPendingReview = new Status { NameOption = Status.Option.PendingReview };

            var timeRecords = new List<TimeRecord>();

            for (int i = 0; i < 13; i++)
            {
                timeRecords.Add(CreateValidEntities.TimeRecord(i+1));
            }

            #region regardless of status, non of these time records will be returned because they have a different supervisor
            timeRecords[0].User = userList[1];
            timeRecords[0].Status = statusApproved;
            timeRecords[1].User = userList[1];
            timeRecords[1].Status = statusCurrent;
            timeRecords[2].User = userList[1];
            timeRecords[2].Status = statusDisapproved;
            timeRecords[3].User = userList[1];
            timeRecords[3].Status = statusPendingReview;
            #endregion regardless of status, non of these time records will be returned because they have a different supervisor

            #region TimeRecords for Chancy (Order 2)
            //Order 2
            timeRecords[4].User = userList[0];
            timeRecords[4].Year = 2009;
            timeRecords[4].Month = 6;
            timeRecords[4].Status = statusPendingReview;

            //Order 3
            timeRecords[5].User = userList[0];
            timeRecords[5].Year = 2009;
            timeRecords[5].Month = 12;
            timeRecords[5].Status = statusCurrent;

            //Order 1
            timeRecords[6].User = userList[0];
            timeRecords[6].Year = 2008;
            timeRecords[6].Month = 5;
            timeRecords[6].Status = statusCurrent;
            #endregion TimeRecords for Chancy

            #region TimeRecords for Zeb (Order 3)
            //Order 1
            timeRecords[7].User = userList[2];
            timeRecords[7].Year = 2007;
            timeRecords[7].Month = 6;
            timeRecords[7].Status = statusPendingReview;

            //Order 2
            timeRecords[8].User = userList[2];
            timeRecords[8].Year = 2008;
            timeRecords[8].Month = 12;
            timeRecords[8].Status = statusPendingReview;

            //Order 3
            timeRecords[9].User = userList[2];
            timeRecords[9].Year = 2009;
            timeRecords[9].Month = 5;
            timeRecords[9].Status = statusCurrent;
            #endregion TimeRecords for Zeb 

            #region TimeRecords for Abby (Order 1)
            //Order 3
            timeRecords[10].User = userList[3];
            timeRecords[10].Year = 2009;
            timeRecords[10].Month = 6;
            timeRecords[10].Status = statusCurrent;

            //Order 2
            timeRecords[11].User = userList[3];
            timeRecords[11].Year = 2008;
            timeRecords[11].Month = 12;
            timeRecords[11].Status = statusCurrent;

            //Order 1
            timeRecords[12].User = userList[3];
            timeRecords[12].Year = 2007;
            timeRecords[12].Month = 5;
            timeRecords[12].Status = statusPendingReview;
            #endregion TimeRecords for Abby

            #region Status of Approved and Disapproved are filtered out

            for (int i = 0; i < 6; i++)
            {
                timeRecords.Add(CreateValidEntities.TimeRecord(i + 13));
                timeRecords[i + 13].Status = i%2==0 ? statusApproved : statusDisapproved;
            }
            timeRecords[13].User = userList[0];
            timeRecords[14].User = userList[0];
            timeRecords[15].User = userList[2];
            timeRecords[16].User = userList[2];
            timeRecords[17].User = userList[3];
            timeRecords[18].User = userList[3];

            #endregion Status of Approved and Disapproved are filtered out

            var timeRecordRepository = MockRepository.GenerateStub<IRepository<TimeRecord>>();
            _repository.Expect(a => a.OfType<TimeRecord>()).Return(timeRecordRepository).Repeat.Any();
            timeRecordRepository.Expect(a => a.Queryable).Return(timeRecords.AsQueryable()).Repeat.Any();
        }

        /// <summary>
        /// Fakes the cost share records to check for getReviewableAndCurrentRecords method.
        /// </summary>
        private void FakeCostShareRecordsToCheckWithGetReviewableAndCurrentRecords()
        {
            var differentSupervisor = CreateValidEntities.User(null);
            differentSupervisor.UserName = "SomeOtherSupervisor";
            var userList = new List<User>();
            for (int i = 0; i < 4; i++)
            {
                userList.Add(CreateValidEntities.User(i + 1));
                userList[i].Supervisor = CurrentUser;
            }
            userList[1].Supervisor = differentSupervisor;

            userList[3].LastName = "Abby";
            userList[0].LastName = "Chancy";
            userList[2].LastName = "Zeb";

            var statusCurrent = new Status { NameOption = Status.Option.Current };
            var statusApproved = new Status { NameOption = Status.Option.Approved };
            var statusDisapproved = new Status { NameOption = Status.Option.Disapproved };
            var statusPendingReview = new Status { NameOption = Status.Option.PendingReview };

            var costShareRecords = new List<CostShare>();

            for (int i = 0; i < 13; i++)
            {
                costShareRecords.Add(CreateValidEntities.CostShare(i + 1));
            }

            #region regardless of status, non of these costShareRecords will be returned because they have a different supervisor
            costShareRecords[0].User = userList[1];
            costShareRecords[0].Status = statusApproved;
            costShareRecords[1].User = userList[1];
            costShareRecords[1].Status = statusCurrent;
            costShareRecords[2].User = userList[1];
            costShareRecords[2].Status = statusDisapproved;
            costShareRecords[3].User = userList[1];
            costShareRecords[3].Status = statusPendingReview;
            #endregion regardless of status, non of these costShareRecords will be returned because they have a different supervisor

            #region TimeRecords for Chancy (Order 2)
            //Order 2
            costShareRecords[4].User = userList[0];
            costShareRecords[4].Year = 2009;
            costShareRecords[4].Month = 6;
            costShareRecords[4].Status = statusPendingReview;

            //Order 3
            costShareRecords[5].User = userList[0];
            costShareRecords[5].Year = 2009;
            costShareRecords[5].Month = 12;
            costShareRecords[5].Status = statusCurrent;

            //Order 1
            costShareRecords[6].User = userList[0];
            costShareRecords[6].Year = 2008;
            costShareRecords[6].Month = 5;
            costShareRecords[6].Status = statusCurrent;
            #endregion TimeRecords for Chancy

            #region TimeRecords for Zeb (Order 3)
            //Order 1
            costShareRecords[7].User = userList[2];
            costShareRecords[7].Year = 2007;
            costShareRecords[7].Month = 6;
            costShareRecords[7].Status = statusPendingReview;

            //Order 2
            costShareRecords[8].User = userList[2];
            costShareRecords[8].Year = 2008;
            costShareRecords[8].Month = 12;
            costShareRecords[8].Status = statusPendingReview;

            //Order 3
            costShareRecords[9].User = userList[2];
            costShareRecords[9].Year = 2009;
            costShareRecords[9].Month = 5;
            costShareRecords[9].Status = statusCurrent;
            #endregion TimeRecords for Zeb

            #region TimeRecords for Abby (Order 1)
            //Order 3
            costShareRecords[10].User = userList[3];
            costShareRecords[10].Year = 2009;
            costShareRecords[10].Month = 6;
            costShareRecords[10].Status = statusCurrent;

            //Order 2
            costShareRecords[11].User = userList[3];
            costShareRecords[11].Year = 2008;
            costShareRecords[11].Month = 12;
            costShareRecords[11].Status = statusCurrent;

            //Order 1
            costShareRecords[12].User = userList[3];
            costShareRecords[12].Year = 2007;
            costShareRecords[12].Month = 5;
            costShareRecords[12].Status = statusPendingReview;
            #endregion TimeRecords for Abby

            #region Status of Approved and Disapproved are filtered out

            for (int i = 0; i < 6; i++)
            {
                costShareRecords.Add(CreateValidEntities.CostShare(i + 13));
                costShareRecords[i + 13].Status = i % 2 == 0 ? statusApproved : statusDisapproved;
            }
            costShareRecords[13].User = userList[0];
            costShareRecords[14].User = userList[0];
            costShareRecords[15].User = userList[2];
            costShareRecords[16].User = userList[2];
            costShareRecords[17].User = userList[3];
            costShareRecords[18].User = userList[3];

            #endregion Status of Approved and Disapproved are filtered out

            var costShareRepository = MockRepository.GenerateStub<IRepository<CostShare>>();
            _repository.Expect(a => a.OfType<CostShare>()).Return(costShareRepository).Repeat.Any();
            costShareRepository.Expect(a => a.Queryable).Return(costShareRecords.AsQueryable()).Repeat.Any();
        }


        /// <summary>
        /// Create and return a valid user.
        /// </summary>
        /// <returns></returns>
        private static User CreateValidUser()
        {
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 100,
                BenefitRate = 2,
                FTE = 1,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);

            return user;
        }

        private static IList<FundType> FakeFundTypes()
        {
            var fundTypes = new List<FundType>
                                {
                                    new FundType {Name = "Name1"},
                                    new FundType {Name = "Name2"},
                                    new FundType {Name = "Name3"}
                                };

            return fundTypes;
        }

        private static IList<Project> FakeProjects()
        {
            var projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project{Name = "Name2", IsActive = true}
                               };
            return projects;

        }

        #endregion Helper Methods

        #region Mocks
        /// <summary>
        /// Mock the Identity. Used for getting the current user name
        /// </summary>
        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "CurrentUser";
                }
            }
        }


        /// <summary>
        /// Mock the Principal. Used for getting the current user name
        /// </summary>
        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;

            public IIdentity Identity
            {
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public virtual bool IsInRole(string role)
            {
                return false;
            }
        }

        ///// <summary>
        ///// Mock the HttpContext. Used for getting the current user name
        ///// </summary>
        //public class MockHttpContext : HttpContextBase
        //{
        //    private IPrincipal _user;

        //    public override IPrincipal User
        //    {
        //        get
        //        {
        //            if (_user == null)
        //            {
        //                _user = new MockPrincipal(false);
        //            }
        //            return _user;
        //        }
        //        set
        //        {
        //            _user = value;
        //        }
        //    }
        //}
        #endregion Mocks
    }
}
