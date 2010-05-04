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
        private IRepository _repository;
        private readonly IPrincipal _principal = new MockPrincipal();
        private List<Record> Records { get; set; }

        private User CurrentUser { get; set; }

        [TestInitialize]
        public void Setup()
        {
            _repository = MockRepository.GenerateStub<IRepository>();
            _recordBLL = new RecordBLL<Record>(_repository);

            CurrentUser = CreateValidUser();
            CurrentUser.UserName = "CurrentUser";
        }

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

        [TestMethod]
        public void GetCurrentRecordReturnsCorrectRecordFirstByDateOrderTest1()
        {
            FakeRecordsToCheck();

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

        #region Helper Methods

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

        #region mocks
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

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        /// <summary>
        /// Mock the HttpContext. Used for getting the current user name
        /// </summary>
        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal();
                    }
                    return _user;
                }
                set
                {
                    _user = value;
                }
            }
        }
        #endregion
    }
}