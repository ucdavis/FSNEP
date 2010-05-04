using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FSNEP.Controllers;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Core.PersistanceSupport;


namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest : Core.ControllerTestBase<HomeController>
    {
        private IRepository<User> UserRepository { get; set; }
        private readonly IPrincipal _principal = MockRepository.GenerateStub<MockPrincipal>();
        private readonly IIdentity _identity  = MockRepository.GenerateStub<MockIdentity>();
        private readonly List<User> _users = new List<User>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeControllerTest"/> class.
        /// </summary>
        public HomeControllerTest()
        {
            UserRepository = FakeRepository<User>();
            var fakeContext = MockRepository.GenerateStub<IDbContext>();
            UserRepository.Expect(a => a.DbContext).Return(fakeContext).Repeat.Any();

            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();
            Controller.ControllerContext.HttpContext.User = _principal;

            _principal.Expect(a => a.Identity).Return(_identity).Repeat.Any();
            
            FakeUsers();
        }

        [TestMethod]
        public void Index()
        {
            // Act
            var result = Controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            var viewData = result.ViewData;
            Assert.AreEqual("Welcome to ASP.NET MVC!", viewData["Message"]);
        }

        /// <summary>
        /// Homes controller error.
        /// </summary>
        [TestMethod]
        public void HomeControllerError()
        {
            var controller = new HomeController();
            var result = controller.Error("This is a test error!") as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("This is a test error!", result.MasterName);
        }

        [TestMethod]
        public void About()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        #region SemiAnnualCertification Test

        #region Valid Test
        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenValid()
        {
            var fakeDate = new DateTime(2009, 04, 10);
            SystemTime.Now = () => fakeDate;
            
            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();            
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();
            
            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }
        #endregion Valid Test

        #region Date Range Tests
        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsAprilFirst()
        {
            var fakeDate = new DateTime(2009, 04, 01);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsAprilSecond()
        {
            var fakeDate = new DateTime(2009, 04, 02);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsApril14()
        {
            var fakeDate = new DateTime(2009, 04, 14);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsApril15()
        {
            var fakeDate = new DateTime(2009, 04, 15);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns false when date is april16.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsFalseWhenDateIsApril16()
        {
            var fakeDate = new DateTime(2009, 04, 16);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsFalse(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns false when date is March 31.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsFalseWhenDateIsMarch31()
        {
            var fakeDate = new DateTime(2009, 03, 31);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsFalse(result.ShouldShowCertificationLink);
        }


        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsOctoberFirst()
        {
            var fakeDate = new DateTime(2009, 10, 01);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsOctoberSecond()
        {
            var fakeDate = new DateTime(2009, 10, 02);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsOctober14()
        {
            var fakeDate = new DateTime(2009, 10, 14);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns true when valid.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsTrueWhenDateIsOctober15()
        {
            var fakeDate = new DateTime(2009, 10, 15);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsTrue(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns false when date is october16.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsFalseWhenDateIsOctober16()
        {
            var fakeDate = new DateTime(2009, 10, 16);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsFalse(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns false when date is october16.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsFalseWhenDateIsSeptember30()
        {
            var fakeDate = new DateTime(2009, 09, 30);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();


            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsFalse(result.ShouldShowCertificationLink);
        }

        #endregion Date Range Tests

        #region Tests where we expect false


        /// <summary>
        /// Tests the semi annual certification returns false when fte is not one.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsFalseWhenFteIsNotOne()
        {
            var fakeDate = new DateTime(2009, 04, 10);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            _users[0].FTE = .5;


            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsFalse(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the name of the semi annual certification returns false when user name is not the current user.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsFalseWhenUserNameIsNotTheCurrentUserName()
        {
            var fakeDate = new DateTime(2009, 04, 10);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Once();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            _users[0].UserName = "UserName0";


            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsFalse(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns false when user is not in the time sheet role.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsFalseWhenUserIsNotInTheTimeSheetRole()
        {
            var fakeDate = new DateTime(2009, 04, 10);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(false).Repeat.Any();
            _identity.Expect(a => a.IsAuthenticated).Return(true).Repeat.Once();

            _users[0].UserName = "UserName";


            var result = Controller.SemiAnnualCertification()
                .AssertViewRendered()
                .WithViewData<SemiAnnualCertificationViewModel>();
            Assert.IsFalse(result.ShouldShowCertificationLink);
        }

        /// <summary>
        /// Tests the semi annual certification returns false when user is not in the time sheet role.
        /// </summary>
        [TestMethod]
        public void TestSemiAnnualCertificationReturnsEmptyContentIfNotAuthorized()
        {
            var fakeDate = new DateTime(2009, 04, 10);
            SystemTime.Now = () => fakeDate;

            _principal.Expect(a => a.IsInRole(RoleNames.RoleTimeSheet)).Return(true).Repeat.Any();
            _identity.Expect(a => a.IsAuthenticated).Return(false).Repeat.Once();


            _users[0].UserName = "UserName";


            var result = Controller.SemiAnnualCertification();               
            Assert.IsNotNull(result);
        }

        #endregion Tests where we expect false

        #endregion SemiAnnualCertification Test

        #region Helper Methods

        /// <summary>
        /// Fakes the users.
        /// </summary>
        private void FakeUsers()
        {
            for (int i = 0; i < 3; i++)
            {
                _users.Add(CreateValidEntities.User(i + 2));
            }
            _users[0].UserName = "UserName";
            _users[1].UserName = "UserName1";
            _users[2].UserName = "UserName2";
            _users[0].FTE = 1.0;

            UserRepository.Expect(a => a.Queryable).Return(_users.AsQueryable()).Repeat.Any();
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

            public virtual bool IsAuthenticated
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

            public virtual IIdentity Identity
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

        #endregion Mocks
    }
}
