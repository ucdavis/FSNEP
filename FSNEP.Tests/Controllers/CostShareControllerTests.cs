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

        private User _currentUser = CreateValidEntities.User(null);
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

        [TestMethod]
        public void HistoryReturnsExpectedData()
        {
            Controller.ControllerContext.HttpContext.User = _principal;
            FakeCostShareRecords();


        }
        

        #endregion History Tests

        #region Helper Methods

        private void FakeCostShareRecords()
        {
            var differentUser = CreateValidEntities.User(null);
            differentUser.UserName = "DifferentUser";
            var differentUserWithCurrentUserAsSupervisor = CreateValidEntities.User(null);
            differentUserWithCurrentUserAsSupervisor.Supervisor = _currentUser;

            var costShareList = new List<CostShare>();
            for (int i = 0; i < 10; i++)
            {
                costShareList.Add(CreateValidEntities.CostShare(i));
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
