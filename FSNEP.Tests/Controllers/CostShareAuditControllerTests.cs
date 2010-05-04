using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class CostShareAuditControllerTests : Core.ControllerTestBase<CostShareAuditController>
    {
        private IRepository<Project> ProjectRepository { get; set;}
        private IRepository<CostShare> CostShareRepository { get; set; }
        private IRepository<CostShareEntry> CostShareEntryRepository { get; set; }
        private IRepository<User> UserRepository { get; set; }
        private List<Project> Projects { get; set; }
        private List<User> Users { get; set; }
        private List<CostShare> CostShareRecords { get; set; }
        private List<CostShareEntry> CostShareEntryRecords { get; set; }

        #region Init

        public CostShareAuditControllerTests()
        {
            ProjectRepository = FakeRepository<Project>();
            CostShareRepository = FakeRepository<CostShare>();
            CostShareEntryRepository = FakeRepository<CostShareEntry>();
            UserRepository = FakeRepository<User>();
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(ProjectRepository).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<CostShare>()).Return(CostShareRepository).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<User>()).Return(UserRepository).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<CostShareEntry>()).Return(CostShareEntryRepository).Repeat.Any();

            Projects = new List<Project>();
            Users = new List<User>();
            CostShareRecords = new List<CostShare>();
            CostShareEntryRecords = new List<CostShareEntry>();
        }
        

        #endregion Init

        #region Route Tests

        /// <summary>
        /// Tests the routing cost share audit history maps to history.
        /// </summary>
        [TestMethod]
        public void TestRoutingCostShareAuditHistoryMapsToHistory()
        {
            const int id = 5;
            "~/CostShareAudit/History/5".ShouldMapTo<CostShareAuditController>(a => a.History(id));
        }

        /// <summary>
        /// Tests the routing cost share audit review maps to review.
        /// </summary>
        [TestMethod]
        public void TestRoutingCostShareAuditReviewMapsToReview()
        {
            const int id = 5;
            "~/CostShareAudit/Review/5".ShouldMapTo<CostShareAuditController>(a => a.Review(id));
        }

        /// <summary>
        /// Tests the routing cost share audit exclude maps to exclude.
        /// </summary>
        [TestMethod]
        public void TestRoutingCostShareAuditExcludeMapsToExclude()
        {            
            "~/CostShareAudit/Exclude".ShouldMapTo<CostShareAuditController>(a => a.Exclude(5, "Because"),true);
        }

        #endregion Route Tests

        #region History Tests

        /// <summary>
        /// Tests the cost share audit history returns view when id is not found.
        /// </summary>
        [TestMethod]
        public void TestCostShareAuditHistoryReturnsViewWhenIdIsNotFound()
        {
            FakeProjects();            

            var result = Controller.History(9)
                .AssertViewRendered()
                .WithViewData<CostShareAuditHistoryViewModel>();
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Projects.Count());
            Assert.IsNull(result.Project);
        }        


        /// <summary>
        /// Tests the cost share audit history returns view when id is found.
        /// </summary>
        [TestMethod]
        public void TestCostShareAuditHistoryReturnsViewWhenIdIsFound()
        {
            FakeProjects();
            FakeUsers();
            FakeCostShareRecords();

            var result = Controller.History(5)
                .AssertViewRendered()
                .WithViewData<CostShareAuditHistoryViewModel>();
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Projects.Count());
            Assert.AreSame(Projects[4], result.Project);
            Assert.AreEqual(3, result.Records.Count());
            var costShareRecordsFound = result.Records.OrderBy(a => a.ReviewComment).ToList();
            Assert.AreEqual("ReviewComment2", costShareRecordsFound[0].ReviewComment);
            Assert.AreEqual("ReviewComment3", costShareRecordsFound[1].ReviewComment);
            Assert.AreEqual("ReviewComment5", costShareRecordsFound[2].ReviewComment);
        }        
        

        #endregion History Tests

        #region Review Tests

        /// <summary>
        /// Tests the cost share audit review returns view.
        /// </summary>
        [TestMethod]
        public void TestCostShareAuditReviewReturnsView()
        {
            const int costShareId = 3;
            FakeProjects();
            FakeUsers();
            FakeCostShareRecords();
            FakeCostShareEntryRecords();

            CostShareRepository.Expect(a => a.GetNullableByID(costShareId)).Return(CostShareRecords[2]).Repeat.Any();

            var result = Controller.Review(costShareId)
                .AssertViewRendered()
                .WithViewData<CostShareAuditReviewViewModel>();
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Entries.Count());
            var costShareEntriesFound = result.Entries.OrderBy(a => a.Comment).ToList();
            Assert.AreEqual("Comment3", costShareEntriesFound[0].Comment);
            Assert.AreEqual("Comment4", costShareEntriesFound[1].Comment);
        }
        
        /// <summary>
        /// Tests the cost share audit review throws exception when cost share not found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCostShareAuditReviewThrowsExceptionWhenCostShareNotFound()
        {
            try
            {
                CostShareRepository.Expect(a => a.GetNullableByID(9)).Return(null).Repeat.Once();
                var result = Controller.Review(9)
                .AssertViewRendered()
                .WithViewData<CostShareAuditReviewViewModel>();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Precondition failed.", ex.Message);
                throw;
            }
        }
        

        #endregion Review Tests


        #region Exclude Tests

        /// <summary>
        /// Tests the cost share audit exclude saves.
        /// </summary>
        [TestMethod]
        public void TestCostShareAuditExcludeSaves()
        {
            FakeProjects();
            FakeUsers();
            FakeCostShareRecords();
            FakeCostShareEntryRecords();

            Assert.IsFalse(CostShareEntryRecords[2].Exclude);
            Assert.AreNotEqual("Because", CostShareEntryRecords[2].ExcludeReason);

            var result = Controller.Exclude(3, "Because");
            CostShareEntryRepository.AssertWasCalled(a => a.EnsurePersistent(CostShareEntryRecords[2]));
            Assert.IsNotNull(result);
            Assert.AreEqual("{ Success = True, EntryId = 3 }", result.Data.ToString());
            Assert.IsTrue(CostShareEntryRecords[2].Exclude);
            Assert.AreEqual("Because", CostShareEntryRecords[2].ExcludeReason);                
        }


        /// <summary>
        /// Tests the cost share audit exclude throws exception when cost share entry record not found.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCostShareAuditExcludeThrowsExceptionWhenCostShareEntryRecordNotFound()
        {
            try
            {
                CostShareEntryRepository.Expect(a => a.GetNullableByID(9)).Return(null).Repeat.Once();
                Controller.Exclude(9, "Because");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid Entry Identifier", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Tests the cost share audit exclude throws exception when exclude reason is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCostShareAuditExcludeThrowsExceptionWhenExcludeReasonIsNull()
        {
            try
            {
                FakeProjects();
                FakeUsers();
                FakeCostShareRecords();
                FakeCostShareEntryRecords();

                Assert.IsFalse(CostShareEntryRecords[2].Exclude);
                Assert.AreNotEqual("Because", CostShareEntryRecords[2].ExcludeReason);

                Controller.Exclude(3, null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Exclude Reason Is Not Valid", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Tests the cost share audit exclude throws exception when exclude reason is empty.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCostShareAuditExcludeThrowsExceptionWhenExcludeReasonIsEmpty()
        {
            try
            {
                FakeProjects();
                FakeUsers();
                FakeCostShareRecords();
                FakeCostShareEntryRecords();

                Assert.IsFalse(CostShareEntryRecords[2].Exclude);
                Assert.AreNotEqual("Because", CostShareEntryRecords[2].ExcludeReason);

                Controller.Exclude(3, string.Empty);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Exclude Reason Is Not Valid", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Tests the cost share audit exclude throws exception when exclude reason is spaces only.      
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PreconditionException))]
        public void TestCostShareAuditExcludeThrowsExceptionWhenExcludeReasonIsSpacesOnly()
        {
            //TODO: Fix in Controller or Change test so this is valid.
            try
            {
                FakeProjects();
                FakeUsers();
                FakeCostShareRecords();
                FakeCostShareEntryRecords();

                Assert.IsFalse(CostShareEntryRecords[2].Exclude);
                Assert.AreNotEqual("Because", CostShareEntryRecords[2].ExcludeReason);

                var result = Controller.Exclude(3, " ");
                CostShareEntryRepository.AssertWasCalled(a => a.EnsurePersistent(CostShareEntryRecords[2]));
                Assert.IsNotNull(result);
                Assert.AreEqual("{ Success = True, EntryId = 3 }", result.Data.ToString());
                Assert.IsTrue(CostShareEntryRecords[2].Exclude);
                Assert.AreEqual(" ", CostShareEntryRecords[2].ExcludeReason); 
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Exclude Reason Is Not Valid", ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Tests the cost share audit exclude does not save when exclude reason is too long.
        /// </summary>
        [TestMethod]
        public void TestCostShareAuditExcludeDoesNotSaveWhenExcludeReasonIsTooLong()
        {
            //TODO: Fix in controller or remove test.
            FakeProjects();
            FakeUsers();
            FakeCostShareRecords();
            FakeCostShareEntryRecords();

            var sb = new StringBuilder();
            for (int i = 0; i < 25; i++)
            {
                sb.Append("1234567890");
            }
            sb.Append("1234567");
            Assert.AreEqual(257, sb.ToString().Length);

            Assert.IsFalse(CostShareEntryRecords[2].Exclude);
            Assert.AreNotEqual(sb.ToString(), CostShareEntryRecords[2].ExcludeReason);

            var result = Controller.Exclude(3, sb.ToString());
            CostShareEntryRepository.AssertWasNotCalled(a => a.EnsurePersistent(CostShareEntryRecords[2]));
            Assert.Inconclusive("If this test passes to here, extra validation must be done to make sure errors are generated.");
        }

        #endregion Exclude Tests

        #region Helper Methods

        /// <summary>
        /// Fakes the projects.
        /// </summary>
        private void FakeProjects()
        {
            for (int i = 0; i < 7; i++)
            {
                Projects.Add(CreateValidEntities.Project(i+1));
                Projects[i].SetIdTo(i + 1);
            }
            ProjectRepository.Expect(a => a.Queryable).Return(Projects.AsQueryable()).Repeat.Any();
            ProjectRepository.Expect(a => a.GetNullableByID(5)).Return(Projects[4]).Repeat.Any();
        }

        /// <summary>
        /// Fakes the users.
        /// </summary>
        private void FakeUsers()
        {
            for (int i = 0; i < 5; i++)
            {
                Users.Add(CreateValidEntities.User(i+1));
                Users[i].Projects = new List<Project>();
                Users[i].Projects.Add(Projects[1]);
            }
            Users[1].Projects.Add(Projects[4]); 
            Users[3].Projects.Add(Projects[4]);
            UserRepository.Expect(a => a.Queryable).Return(Users.AsQueryable()).Repeat.Any();
        }

        /// <summary>
        /// Fakes the cost share records.
        /// </summary>
        private void FakeCostShareRecords()
        {
            for (int i = 0; i < 7; i++)
            {
                CostShareRecords.Add(CreateValidEntities.CostShare(i+1));
                CostShareRecords[i].User = Users[0];
                CostShareRecords[i].SetIdTo(i + 1);
            }
            CostShareRecords[1].User = Users[1];
            CostShareRecords[2].User = Users[1];            
            CostShareRecords[4].User = Users[3];

            CostShareRepository.Expect(a => a.Queryable).Return(CostShareRecords.AsQueryable()).Repeat.Any();
        }

        /// <summary>
        /// Fakes the cost share entry records.
        /// </summary>
        private void FakeCostShareEntryRecords()
        {
            for (int i = 0; i < 7; i++)
            {
                CostShareEntryRecords.Add(CreateValidEntities.CostShareEntry(i+1));
                CostShareEntryRecords[i].SetIdTo(i + 1);                
            }

            CostShareRecords[2].AddEntry(CostShareEntryRecords[2]);
            CostShareRecords[2].AddEntry(CostShareEntryRecords[3]);
            CostShareRecords[4].AddEntry(CostShareEntryRecords[5]);

            CostShareEntryRepository.Expect(a => a.Queryable).Return(CostShareEntryRecords.AsQueryable()).Repeat.Any();
            CostShareEntryRepository.Expect(a => a.GetNullableByID(3)).Return(CostShareEntryRecords[2]).Repeat.Any();
        }

        #endregion Helper Methods
    }
}
