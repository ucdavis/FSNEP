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

            var result = Controller.AddEntry(timeRecord.Id, timeRecordEntry);
            _timeRecordBll.AssertWasCalled(a => a.EnsurePersistent(timeRecord));
            Assert.IsNotNull(result);
            Assert.AreEqual("{ id = 24 }", result.Data.ToString());
        }

        //TODO: Failure (ie. invalid timerecord.id)

        #endregion AddEntry Tests


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
