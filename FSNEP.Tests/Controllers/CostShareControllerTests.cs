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


    }
}
