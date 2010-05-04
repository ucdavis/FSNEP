using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers;
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
    public class FileControllerTests : Core.ControllerTestBase<FileController>
    {
        private readonly IRepository<CostShare> _costShareRepository = MockRepository.GenerateStub<IRepository<CostShare>>();
        private readonly ICostShareBLL _costShareBLL = MockRepository.GenerateStub<ICostShareBLL>();
        private IRepository<CostShareEntry> CostShareEntryRepository { get; set; }

        private readonly User _currentUser = CreateValidEntities.User(null);
        private readonly IPrincipal _principal = new MockPrincipal();

        #region Init

        /// <summary>
        /// Initializes a new instance of the <see cref="FileControllerTests"/> class.
        /// </summary>
        public FileControllerTests()
        {
            Controller.ControllerContext.HttpContext.User = _principal;

            CostShareEntryRepository = FakeRepository<CostShareEntry>();
            Controller.Repository.Expect(a => a.OfType<CostShareEntry>()).Return(CostShareEntryRepository).Repeat.Any();
        }

        /// <summary>
        /// Override this method to setup a controller that doesn't have an empty ctor
        /// </summary>
        protected override void SetupController()
        {
            var fakeContext =
                MockRepository.GenerateStub<IDbContext>();

            _costShareRepository.Expect(a => a.DbContext).Return(fakeContext).Repeat.Any();

            CreateController(_costShareRepository, _costShareBLL);
        }

        #endregion Init

        #region Routing Tests

        /// <summary>
        /// Tests the routing view entry file maps to view entry file.
        /// </summary>
        [TestMethod]
        public void TestRoutingViewEntryFileMapsToViewEntryFile()
        {
            const int entryId = 5;
            "~/File/ViewEntryFile/5".ShouldMapTo<FileController>(a => a.ViewEntryFile(entryId),true);
        }

        #endregion Routing Tests

        #region Task 565 ViewEntryFile Tests

        /// <summary>
        /// Tests the view entry file returns file result when user has access.
        /// </summary>
        [TestMethod]
        public void TestViewEntryFileReturnsFileResultWhenUserHasReviewAccess()
        {
            const bool hasReviewAccess = true;

            var costShareEntries = new List<CostShareEntry>();
            FakeCostShareEntries(costShareEntries, 3);
            costShareEntries[0].Record.SetIdTo(2);
            CostShareEntryRepository.Expect(a => a.GetNullableByID(1)).Return(costShareEntries[0]).Repeat.Once();

            var costShareRecords = new List<CostShare>();
            FakeCostShareRecords(costShareRecords, 3);
            _costShareRepository.Expect(a => a.GetNullableByID(2)).Return(costShareRecords[1]).Repeat.Once();

            _costShareBLL.Expect(a => a.HasReviewAccess(_principal, costShareRecords[1])).Return(hasReviewAccess).Repeat.Once();

            var result = Controller.ViewEntryFile(1)
                .AssertResultIs<FileResult>();
            Assert.AreEqual("SomeFile1.pdf", result.FileDownloadName);
        }

        /// <summary>
        /// Tests the view entry file redirects to error page when user does not have access.
        /// </summary>
        [TestMethod]
        public void TestViewEntryFileRedirectsToErrorPageWhenUserDoesNotHaveAccess()
        {
            const bool hasAccess = false;

            var costShareEntries = new List<CostShareEntry>();
            FakeCostShareEntries(costShareEntries, 3);
            costShareEntries[0].Record.SetIdTo(2);
            CostShareEntryRepository.Expect(a => a.GetNullableByID(1)).Return(costShareEntries[0]).Repeat.Once();

            var costShareRecords = new List<CostShare>();
            FakeCostShareRecords(costShareRecords, 3);
            costShareRecords[1].User = _currentUser;
            _costShareRepository.Expect(a => a.GetNullableByID(2)).Return(costShareRecords[1]).Repeat.Once();

            _costShareBLL.Expect(a => a.HasAccess(_principal, costShareRecords[1])).Return(hasAccess).Repeat.Once();

            Controller.ViewEntryFile(1)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Error(string.Format("{0} does not have access to review this cost share", costShareRecords[1].User.UserName)));

            Assert.AreEqual(string.Format("{0} does not have access to review this cost share", costShareRecords[1].User.UserName),
                Controller.Message);
        }


        /// <summary>
        /// Tests the view entry file requires an entry id to be found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestViewEntryFileRequiresAnEntryIdToBeFound()
        { 
            try
            {
                CostShareEntryRepository.Expect(a => a.GetNullableByID(1)).Return(null).Repeat.Once();
                Controller.ViewEntryFile(1);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid entry indentifier", ex.Message);
                throw;
            } 
        }

        /// <summary>
        /// Tests the view entry file requires cost share record to be found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestViewEntryFileRequiresCostShareRecordToBeFound()
        {
            try
            {
                var costShareEntries = new List<CostShareEntry>();
                FakeCostShareEntries(costShareEntries, 3);
                costShareEntries[0].Record.SetIdTo(2);
                CostShareEntryRepository.Expect(a => a.GetNullableByID(1)).Return(costShareEntries[0]).Repeat.Once();

                _costShareRepository.Expect(a => a.GetNullableByID(2)).Return(null).Repeat.Once();

                Controller.ViewEntryFile(1);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid cost share indentifier", ex.Message);
                throw;
            }
        }

        #endregion Task 565 ViewEntryFile Tests

        #region Helper Methods

        /// <summary>
        /// Fakes the cost share records.
        /// </summary>
        /// <param name="costShareRecords">The cost share records.</param>
        /// <param name="count">The count.</param>
        private static void FakeCostShareRecords(IList<CostShare> costShareRecords, int count)
        {
            var offset = costShareRecords.Count;
            for (int i = 0; i < count; i++)
            {
                costShareRecords.Add(CreateValidEntities.CostShare(i + offset + 1));
                costShareRecords[i + offset].SetIdTo(i + offset + 1);
            }
        }

        /// <summary>
        /// Fakes the cost share entries.
        /// </summary>
        /// <param name="costShareEntries">The cost share entries.</param>
        /// <param name="count">The count.</param>
        private static void FakeCostShareEntries(IList<CostShareEntry> costShareEntries, int count)
        {
            var entryFile = CreateValidEntities.EntryFile(1);
            var offset = costShareEntries.Count;
            for (int i = 0; i < count; i++)
            {
                costShareEntries.Add(CreateValidEntities.CostShareEntry(i + offset + 1));
                costShareEntries[i + offset].SetIdTo(i + offset + 1);
                costShareEntries[i + offset].EntryFile = entryFile;
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
