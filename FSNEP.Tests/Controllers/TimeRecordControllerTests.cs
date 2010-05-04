using System;
using System.Collections.Generic;
using System.Linq;
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
        protected override void SetupController()
        {
            var timeRecordBll = MockRepository.GenerateStub<ITimeRecordBLL>();
            var userBll = MockRepository.GenerateStub<IUserBLL>();
            var timeRecordCalendarGenerator = MockRepository.GenerateStub<ITimeRecordCalendarGenerator>();

            CreateController(timeRecordBll, userBll, timeRecordCalendarGenerator);
        }

        #region Routing maps
        [TestMethod]
        public void RoutingTimeRecordEntryMapsEntry()
        {
            const int id = 5;
            "~/TimeRecord/TimeRecordEntry/5"
                .ShouldMapTo<TimeRecordController>(a => a.TimeRecordEntry(id));
        }

        #endregion Routing maps
    }
}
