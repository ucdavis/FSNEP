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
        /// Routings the cost share history maps to history.
        /// </summary>
        [TestMethod]
        public void RoutingCostShareHistoryMapsToHistory()
        {
            "~/CostShare/History".ShouldMapTo<CostShareController>(a => a.History());
        }

        #endregion Routing Tests

    }
}
