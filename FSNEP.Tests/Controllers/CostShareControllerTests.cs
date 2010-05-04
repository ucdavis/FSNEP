using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Calendar;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Extensions;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;
using UCDArch.Web.ActionResults;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class CostShareControllerTests : Core.ControllerTestBase<CostShareController>
    {        
        private readonly IRepository<CostShare> _costShareRepository = MockRepository.GenerateStub<IRepository<CostShare>>();
        private readonly ICostShareBLL _costShareBLL = MockRepository.GenerateStub<ICostShareBLL>();
        private readonly IUserBLL _userBll = MockRepository.GenerateStub<IUserBLL>();

        private readonly User _currentUser = CreateValidEntities.User(null);
        private readonly IPrincipal _principal = new MockPrincipal();

        protected override void SetupController()
        {            
            //public CostShareController(IRepository<CostShare> costShareRepository, ICostShareBLL costShareBLL, IUserBLL userBLL)
            CreateController(_costShareRepository, _costShareBLL, _userBll);
        }

        #region Routing Tests

        /// <summary>
        /// Routing cost share history maps to history.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareHistoryMapsToHistory()
        {
            "~/CostShare/History".ShouldMapTo<CostShareController>(a => a.History());
        }

        /// <summary>
        /// Routing cost share current maps to current.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareCurrentMapsToCurrent()
        {
            "~/CostShare/Current".ShouldMapTo<CostShareController>(a => a.Current());
        }

        /// <summary>
        /// Routing cost share review maps to review.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareReviewMapsToReview()
        {
            const int id = 5;
            "~/CostShare/Review/5".ShouldMapTo<CostShareController>(a => a.Review(id));
        }

        /// <summary>
        /// Routing cost share entry maps to entry.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareEntryMapsToEntry()
        {
            const int id = 5;
            "~/CostShare/Entry/5".ShouldMapTo<CostShareController>(a => a.Entry(id));
        }

        /// <summary>
        /// Routing cost share entry with parameters maps to entry.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareEntryWithParametersMapsToEntry()
        {
            "~/CostShare/Entry".ShouldMapTo<CostShareController>(a => a.Entry(5, new CostShareEntry()), true);
        }

        /// <summary>
        /// Routing cost share remove entry maps to remove entry.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareRemoveEntryMapsToRemoveEntry()
        {
            const int id = 5;
            "~/CostShare/RemoveEntry/5".ShouldMapTo<CostShareController>(a => a.RemoveEntry(id), true);
        }

        /// <summary>
        /// Routing cost share submit maps to submit.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareSubmitMapsToSubmit()
        {
            const int id = 5;
            "~/CostShare/Submit/5".ShouldMapTo<CostShareController>(a => a.Submit(id));
        }
        

        #endregion Routing Tests

        #region History Tests
        /// <summary>
        /// History returns the cost share records for the current user, with the expected order.
        /// </summary>
        [TestMethod]
        public void HistoryReturnsExpectedData()
        {
            Controller.ControllerContext.HttpContext.User = _principal;
            FakeCostShareRecords();

            var result = Controller.History()
                .AssertViewRendered()
                .WithViewData<IEnumerable<CostShare>>().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(6, result.Count);
            foreach (CostShare costShare in result)
            {
                Assert.AreEqual(_currentUser, costShare.User);
            }
            Assert.AreEqual("ReviewComment10", result[0].ReviewComment);
            Assert.AreEqual("ReviewComment9", result[1].ReviewComment);

        }
        

        #endregion History Tests

        #region Current Tests

        /// <summary>
        /// CostShareController Current Will Redirect To History With Nice Message When Get Current Returns Null
        /// </summary>
        [TestMethod]
        public void CostShareControllerCurrentWillRedirectToHistoryWithNiceMessageWhenGetCurrentReturnsNull()
        {
            Controller.ControllerContext.HttpContext.User = _principal;

            _costShareBLL.Expect(a => a.GetCurrent(_principal)).Return(null).Repeat.Once();

            Controller.Current().AssertActionRedirect().ToAction<CostShareController>(a => a.History());

            Assert.AreEqual("No current cost share available.  You can view your cost share history here.",
                Controller.Message);
        }

        /// <summary>
        /// CostShareController Current Will Redirect To Entry When Get Curren tReturns CostShare Record
        /// </summary>
        [TestMethod]
        public void CostShareControllerCurrentWillRedirectToEntryWhenGetCurrentReturnsCostShareRecord()
        {
            const int id = 5;
            Controller.ControllerContext.HttpContext.User = _principal;
            var costShare = CreateValidEntities.CostShare(null);
            costShare.User = _currentUser;
            costShare.SetIdTo(id);

            _costShareBLL.Expect(a => a.GetCurrent(_principal)).Return(costShare).Repeat.Once();

            Controller.Current().AssertActionRedirect().ToAction<CostShareController>(a => a.Entry(id));

        }

        #endregion Current Tests

        #region Review Tests

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void CostShareControllerReviewThrowsExceptionWhenInvalidIdIsUsed()
        {
            try
            {
                _costShareRepository.Expect(a => a.GetNullableByID(5)).Return(null).Repeat.Once();
                Controller.Review(5);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Invalid cost share indentifier", message.Message);
                throw;
            }
        }

        [TestMethod]
        public void CostShareControllerReviewRedirctsWhenNoAccess()
        {
            Controller.ControllerContext.HttpContext.User = _principal;

            const int id = 5;
            var costShare = CreateValidEntities.CostShare(null);
            costShare.User = _currentUser;
            costShare.SetIdTo(id);

            _costShareRepository.Expect(a => a.GetNullableByID(id)).Return(costShare).Repeat.Once();
            _costShareBLL.Expect(a => a.HasAccess(_principal, costShare)).Return(false).Repeat.Once();

            Controller.Review(id)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Error(string.Format("{0} does not have access to review this cost share", costShare.User.UserName)));

            Assert.AreEqual(string.Format("{0} does not have access to review this cost share", costShare.User.UserName), 
                Controller.Message);
        }

        [TestMethod]
        public void CostShareControllerReviewReturnsViewWhenHasAccess()
        {
            Controller.ControllerContext.HttpContext.User = _principal;

            const int id = 5;
            var costShare = CreateValidEntities.CostShare(null);
            costShare.User = _currentUser;
            costShare.SetIdTo(id);

            FakeCostShareEntryRecords(costShare);


            _costShareRepository.Expect(a => a.GetNullableByID(id)).Return(costShare).Repeat.Once();
            _costShareBLL.Expect(a => a.HasAccess(_principal, costShare)).Return(true).Repeat.Once();

            var result = Controller.Review(id)
                .AssertViewRendered()
                .WithViewData<CostShareReviewViewModel>();
            Assert.IsNotNull(result);

            Assert.AreEqual(4, result.CostShareEntries.Count());

            ////Maybe we don't want to test specific values?
            Assert.AreEqual("Comment1", result.CostShareEntries.ToList()[0].Comment);
            Assert.AreEqual("Comment2", result.CostShareEntries.ToList()[1].Comment);
            Assert.AreEqual("Comment4", result.CostShareEntries.ToList()[2].Comment);
            Assert.AreEqual("Comment5", result.CostShareEntries.ToList()[3].Comment);
        }

        #endregion Review Tests

        #region Entry Tests

        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void CostShareControllerEntryThrowsExceptionWhenInvalidIdIsUsed()
        {
            try
            {
                _costShareRepository.Expect(a => a.GetNullableByID(5)).Return(null).Repeat.Once();
                Controller.Entry(5);
            }
            catch (Exception message)
            {
                Assert.IsNotNull(message);
                Assert.AreEqual("Invalid cost share indentifier", message.Message);
                throw;
            }
        }

        [TestMethod]
        public void CostShareControllerEntryRedirctsWhenNoAccess()
        {
            Controller.ControllerContext.HttpContext.User = _principal;

            const int id = 5;
            var costShare = CreateValidEntities.CostShare(null);
            costShare.User = _currentUser;
            costShare.SetIdTo(id);

            _costShareRepository.Expect(a => a.GetNullableByID(id)).Return(costShare).Repeat.Once();
            _costShareBLL.Expect(a => a.HasAccess(_principal, costShare)).Return(false).Repeat.Once();

            Controller.Entry(id)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Error(string.Format("{0} does not have access to this cost share", costShare.User.UserName)));

            Assert.AreEqual(string.Format("{0} does not have access to this cost share", costShare.User.UserName),
                Controller.Message);
        }


        [TestMethod]
        public void CostShareControllerEntryRedirctsWhenIsNotEditable()
        {
            Controller.ControllerContext.HttpContext.User = _principal;

            const int id = 5;
            var costShare = CreateValidEntities.CostShare(null);
            costShare.User = _currentUser;
            costShare.SetIdTo(id);


            _costShareRepository.Expect(a => a.GetNullableByID(id)).Return(costShare).Repeat.Once();
            _costShareBLL.Expect(a => a.HasAccess(_principal, costShare)).Return(true).Repeat.Once();
            _costShareBLL.Expect(a => a.IsEditable(costShare)).Return(false).Repeat.Once();

            Controller.Entry(id)
                .AssertActionRedirect()
                .ToAction<CostShareController>(a => a.Review(id));
        }

        //[TestMethod]
        //public void CostShareControllerEntryWhenHasAccessAndIsEditable()
        //{
        //    Controller.ControllerContext.HttpContext.User = _principal;

        //    const int id = 5;
        //    var costShare = CreateValidEntities.CostShare(null);
        //    costShare.User = _currentUser;
        //    costShare.SetIdTo(id);

        //    FakeCostShareEntryRecords(costShare);
        //    FakeProjects();
        //    FakeFundTypes();
        //    FakeExpenses();

        //    _costShareRepository.Expect(a => a.GetNullableByID(id)).Return(costShare).Repeat.Once();
        //    _costShareBLL.Expect(a => a.HasAccess(_principal, costShare)).Return(true).Repeat.Once();
        //    _costShareBLL.Expect(a => a.IsEditable(costShare)).Return(true).Repeat.Once();



        //    Controller.Entry(id)
        //        .AssertActionRedirect()
        //        .ToAction<CostShareController>(a => a.Review(id));
        //}

        

        #endregion Entry Tests

        #region Helper Methods

        private void FakeCostShareRecords()
        {
            var differentUser = CreateValidEntities.User(null);
            differentUser.UserName = "DifferentUser";
            var differentUserWithCurrentUserAsSupervisor = CreateValidEntities.User(null);
            differentUserWithCurrentUserAsSupervisor.UserName = "HasCurrentAsSupervisor";
            differentUserWithCurrentUserAsSupervisor.Supervisor = _currentUser;

            var costShareList = new List<CostShare>();
            for (int i = 0; i < 10; i++)
            {
                costShareList.Add(CreateValidEntities.CostShare(i+1));
                switch (i)
                {
                    case 2:
                    case 4:
                        costShareList[i].User = differentUser;
                        break;
                    case 6:
                    case 7:
                        costShareList[i].User = differentUserWithCurrentUserAsSupervisor;
                        break;
                    default:
                        costShareList[i].User = _currentUser;
                        break;
                }
            }

            //To change the order when queried
            costShareList[9].Year = 2011;
            costShareList[8].Year = 2010;

            _costShareRepository.Expect(q => q.Queryable).Return(costShareList.AsQueryable()).Repeat.Any();
        }

        private void FakeCostShareEntryRecords(CostShare share)
        {
            var costShareEntryList = new List<CostShareEntry>();

            for (int i = 0; i < 5; i++)
            {
                costShareEntryList.Add(CreateValidEntities.CostShareEntry(i+1));
                if (i == 2)
                {
                    costShareEntryList[i].Record.SetIdTo(999);
                }
                else
                {
                    costShareEntryList[i].Record.SetIdTo(share.Id);
                }
                costShareEntryList[i].SetIdTo(i + 1);
            }

            var costShareEntryRepository = FakeRepository<CostShareEntry>();

            Controller.Repository.Expect(a => a.OfType<CostShareEntry>()).Return(costShareEntryRepository).Repeat.Any();
            costShareEntryRepository.Expect(a => a.Queryable).Return(costShareEntryList.AsQueryable()).Repeat.Any();

        }

        //private void FakeExpenses()
        //{
        //    throw new NotImplementedException();
        //}

        //private void FakeFundTypes()
        //{
        //    var fundTypeList = new List<FundType>();

        //    for (int i = 0; i < 5; i++)
        //    {
        //        fundTypeList.Add(CreateValidEntities.FundType(i + 1));
        //        fundTypeList[i].SetIdTo(i + 1);
        //    }

        //    var fundTypeRepository = FakeRepository<FundType>();
        //    Controller.Repository.Expect(a => a.OfType<FundType>()).Return(fundTypeRepository).Repeat.Any();
        //    _currentUser.FundTypes = fundTypeList;
        //    _userBll.Expect(a => a.GetUser()).Return(_currentUser).Repeat.Any();
        //}

        //private void FakeProjects()
        //{
        //    var projectList = new List<Project>();

        //    for (int i = 0; i < 5; i++)
        //    {
        //        projectList.Add(CreateValidEntities.Project(i+1));
        //        if(i==2)
        //        {
        //            projectList[i].IsActive = false;
        //        }
        //        projectList[i].SetIdTo(i + 1);
        //    }

        //    var projectRepository = FakeRepository<Project>();
        //    Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();
        //    _userBll.Expect(a => a.GetAllProjectsByUser(projectRepository))
        //        .Return(projectList.AsQueryable().Where(a => a.IsActive)).Repeat.Any();
        //}

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
