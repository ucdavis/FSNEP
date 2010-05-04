using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers;
using FSNEP.Core.Calendar;
using FSNEP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;


namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class TimeRecordControllerTests : Core.ControllerTestBase<TimeRecordController>
    {
        private static readonly User User = CreateValidUser();
        private readonly ITimeRecordBLL _timeRecordBll = MockRepository.GenerateStub<ITimeRecordBLL>();
        private readonly TimeRecord _timeRecord = CreateValidTimeRecord();

        /// <summary>
        /// Override this method to setup a controller that doesn't have an empty ctor
        /// </summary>
        protected override void SetupController()
        {
            var userBll = MockRepository.GenerateStub<IUserBLL>();
            var timeRecordCalendarGenerator = new TimeRecordCalendarGenerator();
            _timeRecord.SetIdTo(1);
            _timeRecordBll.Expect(a => a.GetNullableByID(_timeRecord.Id)).Return(_timeRecord).Repeat.Any();
            _timeRecordBll.Expect(a => a.HasAccess("UserName", _timeRecord)).Return(true).Repeat.Any();

            CreateController(_timeRecordBll, userBll, timeRecordCalendarGenerator);

            var fakeContext =
                MockRepository.GenerateStub<UCDArch.Core.PersistanceSupport.IDbContext>();
            _timeRecordBll.Expect(a => a.DbContext).Return(fakeContext).Repeat.Any();
        }

        #region Routing maps
        /// <summary>
        /// Routings the time record entry maps entry.
        /// </summary>
        [TestMethod]
        public void RoutingTimeRecordEntryMapsEntry()
        {
            const int id = 5;
            "~/TimeRecord/TimeRecordEntry/5"
                .ShouldMapTo<TimeRecordController>(a => a.TimeRecordEntry(id));
        }

        #endregion Routing maps

        #region AddEntry Tests

        /// <summary>
        /// Determines whether this instance [can add valid time record entry].
        /// </summary>
        [TestMethod]
        public void CanAddValidTimeRecordEntry()
        {
            var timeRecordEntry = CreateValidTimeRecordEntry();
            timeRecordEntry.SetIdTo(24);
            IPrincipal userPrincipal = new MockPrincipal();
            Controller.ControllerContext.HttpContext.User = userPrincipal;

            var timeRecord = _timeRecordBll.GetNullableByID(1);
            _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(timeRecord));

            var result = Controller.AddEntry(timeRecord.Id, timeRecordEntry);

            _timeRecordBll.AssertWasCalled(a => a.EnsurePersistent(timeRecord));
            Assert.IsNotNull(result);
            Assert.AreEqual("{ id = 24 }", result.Data.ToString());
        }

        /// <summary>
        /// Invalid time record id causes time record entry to fail.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void InvalidTimeRecordIdCausesTimeRecordEntryToFail()
        {
            const int invalidTimeRecordId = 123; //Does not exist and isn't mocked.
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;                

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                result = Controller.AddEntry(invalidTimeRecordId, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(_timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Invalid time record indentifier", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                throw;
            }
        }



        /// <summary>
        /// Time record with different username causes time record entry to fail.
        /// This test creates a timeRecord with an attached use with the usename "WrongOne".
        /// The mock that checks the currect user is hard coded to "UserName".
        /// That causes this to fail.
        /// Also, the GetNullableByID is set to use this timeRecord when it is called from within AddEntry. 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TimeRecordWithDifferentUsernameCausesTimeRecordEntryToFail()
        {
            TimeRecord timeRecord = null;
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;
                timeRecord = CreateValidTimeRecord();
                timeRecord.User = CreateValidUser("WongOne");
                timeRecord.SetIdTo(13);
                _timeRecordBll.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(timeRecord));
                result = Controller.AddEntry(timeRecord.Id, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Current user does not have access to this record", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(timeRecord));
                throw;
            }
        }

        /// <summary>
        /// Invalid time record entry causes time record entry to fail.
        /// Leonidas test (300th unit test created) :)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void InvalidTimeRecordEntryCausesTimeRecordEntryToFail()
        {
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.ActivityType = null; //makes this invalid
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                result = Controller.AddEntry(_timeRecord.Id, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(_timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Entry is not valid", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                throw;
            }
        }

        /// <summary>
        /// Invalid time record entry hours out of range causes time record entry to fail.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void InvalidTimeRecordEntryHoursOutOfRangeCausesTimeRecordEntryToFail()
        {
            JsonNetResult result = null;
            try
            {
                var timeRecordEntry = CreateValidTimeRecordEntry();
                timeRecordEntry.Hours = 25; //makes this invalid
                timeRecordEntry.SetIdTo(24);
                IPrincipal userPrincipal = new MockPrincipal();
                Controller.ControllerContext.HttpContext.User = userPrincipal;

                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                result = Controller.AddEntry(_timeRecord.Id, timeRecordEntry);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(_timeRecord);
                Assert.IsNull(result);
                Assert.AreEqual("Entry is not valid", message.Message);
                _timeRecordBll.AssertWasNotCalled(a => a.EnsurePersistent(_timeRecord));
                throw;
            }
        } 

        #endregion AddEntry Tests

        #region TimeRecordEntry Tests
        //TODO: These tests for TimeRecordEntry Tests

        #endregion TimeRecordEntry Tests

        #region Helper Methods

        /// <summary>
        /// Creates the valid time record.
        /// </summary>
        /// <returns></returns>
        private static TimeRecord CreateValidTimeRecord()
        {
            return new TimeRecord
                       {
                           Month = DateTime.Now.Month,
                           Year = DateTime.Now.Year,
                           Salary = 200,
                           Status = new Status { Name = "S1" },
                           User = User,
                           ReviewComment = "A review is a review except when it isn't."
                       };
        }

        /// <summary>
        /// Creates the valid user.
        /// </summary>
        /// <returns></returns>
        private static User CreateValidUser()
        {
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 2,
                FTE = 1,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = new List<Project> {new Project {Name = "Project1"}};
            user.FundTypes = new List<FundType>{new FundType{Name = "FundType1"}};

            var userId = Guid.NewGuid();
            user.SetUserID(userId);

            return user;
        }

        /// <summary>
        /// Creates the valid user. With a specific UserName
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        private static User CreateValidUser(string username)
        {
            var user = CreateValidUser();
            user.UserName = username;
            return user;
        }

        /// <summary>
        /// Creates the valid time record entry.
        /// </summary>
        /// <returns></returns>
        private static TimeRecordEntry CreateValidTimeRecordEntry()
        {
            const int validDate = 25;
            const string validComment = "Comment";
            const double validHours = 6.5;
            //TODO: Verify Record, FundType, Project, and Account values.
            return new TimeRecordEntry
                       {
                           Date = validDate,
                           Hours = validHours,
                           Comment = validComment,
                           Record = new Record(),
                           FundType = new FundType(),
                           Project = new Project(),
                           Account = new Account(),
                           ActivityType = new ActivityType()
                       };
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
                    return "UserName";
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
