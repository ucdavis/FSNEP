using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using FSNEP.BLL.Dev;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
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
        private IRepository _repository;
        private IPrincipal _principal = MockRepository.GenerateStub<MockPrincipal>();
        private List<Record> Records { get; set; }

        private User CurrentUser { get; set; }

        [TestInitialize]
        public void Setup()
        {
            _repository = MockRepository.GenerateStub<IRepository>();
            _recordBLL = new RecordBLL<Record>(_repository);
            _timeRecordBLL = new RecordBLL<TimeRecord>(_repository);

            CurrentUser = CreateValidUser();
            CurrentUser.UserName = "CurrentUser";
        }

        #region IsEditable Tests

        [TestMethod]
        public void IsEditableReturnsTrueIfStatusIsCurrent()
        {
            var status = new Status {NameOption = Status.Option.Current};

            var record = new Record {Status = status};

            var editable = _recordBLL.IsEditable(record);

            Assert.AreEqual(true, editable);
        }

        [TestMethod]
        public void IsEditableReturnsTrueIfStatusIsDisapproved()
        {
            var status = new Status { NameOption = Status.Option.Disapproved };

            var record = new Record { Status = status };

            var editable = _recordBLL.IsEditable(record);

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

            var record = new Record { Status = status };

            var editable = _recordBLL.IsEditable(record);

            Assert.AreEqual(true, editable);
        }

        [TestMethod]
        public void IsEditableReturnsFalseIfStatusIsApproved()
        {
            var status = new Status { NameOption = Status.Option.Approved };

            var record = new Record { Status = status };

            var editable = _recordBLL.IsEditable(record);

            Assert.AreEqual(false, editable);
        }

        [TestMethod]
        public void IsEditableReturnsPendingReviewIfStatusIsPendingReview()
        {
            var status = new Status { NameOption = Status.Option.PendingReview };

            var record = new Record { Status = status };

            var editable = _recordBLL.IsEditable(record);

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
            var record = new Record {User = CurrentUser};
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
        /// Determines whether [has review access returns true if current user is in role admin].
        /// </summary>
        [TestMethod]
        public void HasReviewAccessReturnsTrueIfCurrentUserIsInRoleAdmin()
        {
            FakeRecordsToCheck(); //None of these have the Current user.
            _principal.Expect(a => a.IsInRole(RoleNames.RoleAdmin)).Return(true).Repeat.Once();
            Assert.IsTrue(_recordBLL.HasReviewAccess(_principal, Records[1]));
        }

        /// <summary>
        /// Determines whether [has review access returns true if current user is in record].
        /// </summary>
        [TestMethod]
        public void HasReviewAccessReturnsTrueIfCurrentUserIsInRecord()
        {
            var record = new Record { User = CurrentUser };
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

            var record = new Record { User = newUser }; //We are not passing the current user, but this use has the current user as a supervisor.
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

            var record = new Record { User = newUser }; //We are not passing the current user, but this use has the current user as a supervisor.
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

            Records.Add(new Record
            {
                Month = 12,
                Year = 2009,
                User = CurrentUser,
                Status = new Status{NameOption = Status.Option.Current},
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 12,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Approved },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 12,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Disapproved },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 12,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.PendingReview },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 12,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { Name= "Junk" },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

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
            Records.Add(new Record
            {
                Month = 12,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Current },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

            Records.Add(new Record
            {
                Month = 12,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Current },
                ReviewComment = "OrReturnThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 12,
                Year = 2008,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Current },
                ReviewComment = "NotThisRecord",
                Entries = new List<Entry>()
            });

            //We expect This one
            Records.Add(new Record
            {
                Month = 10,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Current },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

            Records.Add(new Record
            {
                Month = 09,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Current },
                ReviewComment = "NotThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 11,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Current },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 11,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Current },
                ReviewComment = "ReturnThisRecord",
                Entries = new List<Entry>()
            });

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

            Records.Add(new Record
            {
                Month = 09,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Approved },
                ReviewComment = "ReturnThisRecord1",
                Entries = new List<Entry>()
            });
            Records.Add(new Record
            {
                Month = 10,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.PendingReview },
                ReviewComment = "ReturnThisRecord2",
                Entries = new List<Entry>()
            });
         

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

            Records.Add(new Record
            {
                Month = 09,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.Approved },
                ReviewComment = "ReturnThisRecord1",
                Entries = new List<Entry>()
            });
            Records.Add(new Record
            {
                Month = 10,
                Year = 2009,
                User = CurrentUser,
                Status = new Status { NameOption = Status.Option.PendingReview },
                ReviewComment = "ReturnThisRecord2",
                Entries = new List<Entry>()
            });


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
                Records.Add(new Record
                                 {
                                     Month = 12,
                                     Year = 2009,
                                     User = nonCurrentUser,
                                     Status = statusCurrent,
                                     ReviewComment = "Comment" + (i + 1),
                                     Entries = new List<Entry>()
                                 });
            }
            Records[1].Status = statusApproved;
            Records[2].Status = statusDisapproved;
            Records[3].Status = statusPendingReview;

            var recordRepository = MockRepository.GenerateStub<IRepository<Record>>();
            _repository.Expect(a => a.OfType<Record>()).Return(recordRepository).Repeat.Any();
            recordRepository.Expect(a => a.Queryable).Return(Records.AsQueryable()).Repeat.Any();
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