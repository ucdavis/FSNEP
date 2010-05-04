using System;
using System.Linq;
using FSNEP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;
using UCDArch.Testing.Extensions;
using RepositoryTestBase = FSNEP.Tests.Core.RepositoryTestBase;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class CostShareRepositoryTests : RepositoryTestBase
    {
        #region Init
        protected override void LoadData()
        {
            base.LoadData();

            using (var ts = new TransactionScope())
            {
                LoadRecords();

                ts.CommitTransaction();
            }

            NHibernateSessionManager.Instance.GetSession().Flush();
        }

        private void LoadRecords()
        {
            for (int i = 0; i < 5; i++)
            {
                var record = CreateValidCostShare();
                record.ReviewComment = "ReviewComment" + (i + 1);
                Repository.OfType<CostShare>().EnsurePersistent(record); 
            }
            
        }
        

        #endregion Init

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can save valid cost share].
        /// </summary>
        [TestMethod]
        public void CanSaveValidCostShare()
        {
            CostShare costShare = CreateValidCostShare();
            Repository.OfType<CostShare>().EnsurePersistent(costShare);

            Assert.AreEqual(false, costShare.IsTransient());
        }

        /// <summary>
        /// Determines whether this instance [can read cost share records].
        /// </summary>
        [TestMethod]
        public void CanReadCostShareRecords()
        {
            var costShareRecords = Repository.OfType<CostShare>().GetAll().ToList();
            Assert.IsNotNull(costShareRecords);
            Assert.AreEqual(5, costShareRecords.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("ReviewComment" + (i + 1), costShareRecords[i].ReviewComment);
            }
        }

        /// <summary>
        /// Determines whether this instance [can query cost share records].
        /// </summary>
        [TestMethod]
        public void CanQueryCostShareRecords()
        {
            var costShareRecords =
                Repository.OfType<CostShare>().Queryable.Where(a => a.ReviewComment.EndsWith("3")).ToList();
            Assert.IsNotNull(costShareRecords);
            Assert.AreEqual(1, costShareRecords.Count);
            Assert.AreEqual("ReviewComment3", costShareRecords[0].ReviewComment);
        }

        /// <summary>
        /// Determines whether this instance [can update cost share record].
        /// </summary>
        [TestMethod]
        public void CanUpdateCostShareRecord()
        {
            var costShareRecord =
                Repository.OfType<CostShare>().Queryable.Where(a => a.ReviewComment.EndsWith("3")).ToList()[0];

            Assert.AreEqual("ReviewComment3", costShareRecord.ReviewComment);

            costShareRecord.ReviewComment = "Updated";
            Repository.OfType<CostShare>().EnsurePersistent(costShareRecord);

            var costShareRecords = Repository.OfType<CostShare>().GetAll().ToList();
            Assert.AreEqual(5, costShareRecords.Count);
            Assert.AreEqual("Updated", costShareRecords[2].ReviewComment);
            Assert.AreEqual("ReviewComment4", costShareRecords[3].ReviewComment);
        }

        /// <summary>
        /// Determines whether this instance [can delete cost share record].
        /// </summary>
        [TestMethod]
        public void CanDeleteCostShareRecord()
        {
            var costShareRecord =
                Repository.OfType<CostShare>().Queryable.Where(a => a.ReviewComment.EndsWith("3")).ToList()[0];

            Assert.AreEqual("ReviewComment3", costShareRecord.ReviewComment);
            
            using (var ts = new TransactionScope())
            {
                Repository.OfType<CostShare>().Remove(costShareRecord);

                ts.CommitTransaction();
            }

            var costShareRecords = Repository.OfType<CostShare>().GetAll().ToList();
            Assert.AreEqual(4, costShareRecords.Count);
            Assert.AreEqual("ReviewComment1", costShareRecords[0].ReviewComment);
            Assert.AreEqual("ReviewComment2", costShareRecords[1].ReviewComment);
            Assert.AreEqual("ReviewComment4", costShareRecords[2].ReviewComment);
            Assert.AreEqual("ReviewComment5", costShareRecords[3].ReviewComment);
        }

        #endregion CRUD Tests

        #region validation Tests

        

        #endregion validation Tests

        #region Helper Methods

        private CostShare CreateValidCostShare()
        {
            return new CostShare
            {
                Month = 10,
                Year = 2009,
                Status = Repository.OfType<Status>().Queryable.First(),
                User = Repository.OfType<User>().Queryable.First(),
                ReviewComment = "A review is a review except when it isn't."
            };
        }

        #endregion Helper Methods
        
    }
}
