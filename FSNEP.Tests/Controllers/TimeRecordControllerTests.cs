using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers;
using FSNEP.Core.Calendar;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using FSNEP.Core.Domain;
using FSNEP.Core.Abstractions;
using System.Web.Security;
using FSNEP.Tests.Core.Extensions;
using UCDArch.Testing;
using Rhino.Mocks.Interfaces;


namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class TimeRecordControllerTests : Core.ControllerTestBase<TimeRecordController>
    {
        private User _user = CreateValidUser();
        /// <summary>
        /// Override this method to setup a controller that doesn't have an empty ctor
        /// </summary>
        protected override void SetupController()
        {
            var timeRecordBll = MockRepository.GenerateStub<ITimeRecordBLL>();
            var userBll = MockRepository.GenerateStub<IUserBLL>();
            //var timeRecordCalendarGenerator = MockRepository.GenerateStub<ITimeRecordCalendarGenerator>();            
            var timeRecordCalendarGenerator = new TimeRecordCalendarGenerator();
            FakeTimeRecord(timeRecordBll);

            timeRecordBll.Expect(a => a.HasAccess("UserName", new TimeRecord())).IgnoreArguments().Return(true).Repeat.
                Any();

            CreateController(timeRecordBll, userBll, timeRecordCalendarGenerator);

            var fakeContext =
                MockRepository.GenerateStub<UCDArch.Core.PersistanceSupport.IDbContext>();
            timeRecordBll.Expect(a => a.DbContext).Return(fakeContext).Repeat.Any();
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
            var timeRecordEntry = new TimeRecordEntry();            
            IPrincipal userPrincipal = new MockPrincipal();
            Controller.ControllerContext.HttpContext.User = userPrincipal;
         
            var result = Controller.AddEntry(1, CreateValidTimeRecordEntry());
        }

        #endregion AddEntry Tests


        #region Helper Methods

        /// <summary>
        /// Fakes the time record.
        /// </summary>
        /// <param name="bll">The BLL.</param>
        private void FakeTimeRecord(ITimeRecordBLL timeRecordBLL)
        {
            TimeRecord timeRecord = CreateValidTimeRecord();
            timeRecord.SetIdTo(1);

            timeRecordBLL.Expect(a => a.GetNullableByID(1)).Return(timeRecord).Repeat.Any();
            
        }

        /// <summary>
        /// Creates the valid time record.
        /// </summary>
        /// <returns></returns>
        private TimeRecord CreateValidTimeRecord()
        {
            return new TimeRecord
                       {
                           Month = DateTime.Now.Month,
                           Year = DateTime.Now.Year,
                           Salary = 200,
                           Status = new Status { Name = "S1" },
                           User = _user,
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
        private TimeRecordEntry CreateValidTimeRecordEntry()
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
                Account = new Account()
            };
        }

        #endregion Helper Methods

        #region mocks
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
        

        public class MockPrincipal : IPrincipal
        {
            IIdentity identity;

            public IIdentity Identity
            {
                get
                {
                    if (identity == null)
                    {
                        identity = new MockIdentity();
                    }
                    return identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal user;

            public override IPrincipal User
            {
                get
                {
                    if (user == null)
                    {
                        user = new MockPrincipal();
                    }
                    return user;
                }
                set
                {
                    user = value;
                }
            }
        }
        #endregion
    }
}
