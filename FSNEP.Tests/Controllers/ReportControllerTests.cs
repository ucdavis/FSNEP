using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FSNEP.BLL.Dev;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Extensions;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class ReportControllerTests : Core.ControllerTestBase<ReportController>
    {
        private readonly ITimeRecordBLL _timeRecordBLL = MockRepository.GenerateStub<ITimeRecordBLL>();
        private readonly IReportBLL _reportBLL = MockRepository.GenerateStub<IReportBLL>();
        private readonly IUserBLL _userBLL = MockRepository.GenerateStub<IUserBLL>();

        private readonly IRepository<TimeRecord> _timeRecordRepository;

        private readonly User _currentUser = CreateValidEntities.User(null);
        private readonly IPrincipal _principal = new MockPrincipal();

        #region Init

        public ReportControllerTests()
        {
            Controller.ControllerContext.HttpContext.User = _principal;
            _timeRecordRepository = FakeRepository<TimeRecord>();
            Controller.Repository.Expect(a => a.OfType<TimeRecord>()).Return(_timeRecordRepository);
        }

        protected override void SetupController()
        {
            //public ReportController(IReportBLL reportBLL, IUserBLL userBLL, ITimeRecordBLL timeRecordBLL)
            CreateController(_reportBLL, _userBLL, _timeRecordBLL);
        }

        #endregion Init



        #region Routing Tests

        [TestMethod]
        public void PrintOwnTimeRecordMapsToPrintOwnTimeRecord()
        {
            const int id = 5;
            "~/Report/PrintOwnTimeRecord/5"
                .ShouldMapTo<ReportController>(a => a.PrintOwnTimeRecord(id));
        }

        [TestMethod]
        public void PrintViewableTimeRecordShouldMapToPrintViewableTimeRecord()
        {
            const int id = 5;
            "~/Report/PrintViewableTimeRecord/5"
                .ShouldMapTo<ReportController>(a => a.PrintViewableTimeRecord(id));
        }

        [TestMethod]
        public void TimeRecordShouldMapToTimeRecord()
        {
            "~/Report/TimeRecord/5"
                .ShouldMapTo<ReportController>(a => a.TimeRecord());
        }

        [TestMethod]
        public void TimeRecordWithParameterShouldMapToTimeRecordWithParameter()
        {
            const int id = 5;
            "~/Report/TimeRecord/5"
                .ShouldMapTo<ReportController>(a => a.TimeRecord(id),true);
        }
        
        [TestMethod]
        public void GetRecordForUserShouldMapToGetRecordForUser()
        {
            "~/Report/GetRecordForUser/"
                .ShouldMapTo<ReportController>(a => a.GetRecordForUser(null));
        }


        [TestMethod]
        public void CostShareShouldMapToCostShare()
        {
            "~/Report/CostShare/"
                .ShouldMapTo<ReportController>(a => a.CostShare());
        }

        [TestMethod]
        public void CostShareWithParamatersShouldMapToCostShareWithParamaters()
        {
            "~/Report/CostShare/"
                .ShouldMapTo<ReportController>(a => a.CostShare(5,10), true);
        }

        #endregion Routing Tests

        #region PrintOwnTimeRecord Tests

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void PrintOwnTimeRecordThrowsExceptionWhenIdNotFound()
        {
            try
            {
                Controller.PrintOwnTimeRecord(5);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Record not found", ex.Message);
                throw;
            }
            
        }

        [TestMethod]
        public void PrintOwnTimeRecordWhenNoAccessReturnsHttpUnauthorizedResult()
        {
            var timeRecord = CreateValidEntities.TimeRecord(null);
            timeRecord.User = _currentUser;
            timeRecord.SetIdTo(5);
            _timeRecordRepository.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

            //We do not care about the comparison that HasAccess is doing in the BLL
            _timeRecordBLL.Expect(a => a.HasAccess(_principal, timeRecord)).Return(false).Repeat.Once();

            Controller.PrintOwnTimeRecord(timeRecord.Id).AssertResultIs<HttpUnauthorizedResult>();
        }

        [TestMethod]
        public void PrintOwnTimeRecordWhenHasAccessReturnsFileContentResult()
        {
            var timeRecord = CreateValidEntities.TimeRecord(null);
            timeRecord.User = _currentUser;
            timeRecord.SetIdTo(5);
            _timeRecordRepository.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

            //We do not care about the comparison that HasAccess is doing in the BLL
            _timeRecordBLL.Expect(a => a.HasAccess(_principal, timeRecord)).Return(true).Repeat.Once();
            _reportBLL.Expect(a => a.GenerateIndividualTimeRecordReport(timeRecord, ReportType.PDF)).Return(
                new ReportResult(new byte[1], "contentType")).Repeat.Once();

            Controller.PrintOwnTimeRecord(timeRecord.Id).AssertResultIs<FileContentResult>();
        }

        #endregion PrintOwnTimeRecord Tests

        #region PrintViewableTimeRecord Tests

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void PrintViewableTimeRecordThrowsExceptionWhenIdNotFound()
        {
            try
            {
                Controller.PrintViewableTimeRecord(5);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Record not found", ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void PrintViewableTimeRecordWhenNoViewableUsersFoundReturnsHttpUnauthorizedResult()
        {
            //TODO: Fix in Controller, and/or change test. (Maybe Have it do a Check.Require on the GetAllViewableUsers())
            var timeRecord = CreateValidEntities.TimeRecord(null);
            timeRecord.User = _currentUser;
            timeRecord.SetIdTo(5);
            _timeRecordRepository.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

            _userBLL.Expect(a => a.GetAllViewableUsers()).Return(new List<User>()).Repeat.Once();

            Controller.PrintViewableTimeRecord(timeRecord.Id).AssertResultIs<HttpUnauthorizedResult>();
        }


        [TestMethod]
        public void PrintViewableTimeRecordWhenViewableUsersDoesNotHaveCurrentUserReturnsHttpUnauthorizedResult()
        {            
            var timeRecord = CreateValidEntities.TimeRecord(null);
            timeRecord.User = _currentUser;
            timeRecord.SetIdTo(5);
            _timeRecordRepository.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

            var users = new List<User>();
            for (int i = 0; i < 5; i++)
            {
                users.Add(CreateValidEntities.User(i+1));
            }

            _userBLL.Expect(a => a.GetAllViewableUsers()).Return(users).Repeat.Once();

            Controller.PrintViewableTimeRecord(timeRecord.Id).AssertResultIs<HttpUnauthorizedResult>();
        }

        [TestMethod]
        public void PrintViewableTimeRecordWhenViewableUsersDoesHaveCurrentUserReturnsFileContentResult()
        {
            var timeRecord = CreateValidEntities.TimeRecord(null);
            timeRecord.User = _currentUser;
            timeRecord.SetIdTo(5);
            _timeRecordRepository.Expect(a => a.GetNullableByID(timeRecord.Id)).Return(timeRecord).Repeat.Once();

            var users = new List<User>();
            for (int i = 0; i < 5; i++)
            {
                users.Add(CreateValidEntities.User(i + 1));
            }

            users.Add(_currentUser); //So it finds it.

            _userBLL.Expect(a => a.GetAllViewableUsers()).Return(users).Repeat.Once();
            _reportBLL.Expect(a => a.GenerateIndividualTimeRecordReport(timeRecord, ReportType.PDF)).Return(
                new ReportResult(new byte[1], "contentType")).Repeat.Once();

            Controller.PrintViewableTimeRecord(timeRecord.Id).AssertResultIs<FileContentResult>();
        }

        #endregion PrintViewableTimeRecord Tests

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
