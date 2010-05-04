using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
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

            //NHibernateSessionManager.Instance.GetSession().Flush();
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

        #region Valid Tests
        /// <summary>
        /// Determines whether this instance [can save valid cost share with null review comments].
        /// </summary>
        [TestMethod]
        public void CanSaveValidCostShareWithNullReviewComments()
        {
            var costShareRecord = CreateValidCostShare();
            costShareRecord.ReviewComment = null;

            Repository.OfType<CostShare>().EnsurePersistent(costShareRecord);

            Assert.AreEqual(false, costShareRecord.IsTransient());
        }


        /// <summary>
        /// Determines whether this instance [can save valid cost shared with entries].
        /// </summary>
        [TestMethod]
        public void CanSaveValidCostSharedWithEntries()
        {
            var costShareRecord = CreateValidCostShare();
            costShareRecord.AddEntry(new Entry
            {
                Comment = "Valid",
                Record = Repository.OfType<Record>().Queryable.First(),
                FundType = Repository.OfType<FundType>().Queryable.First(),
                Project = Repository.OfType<Project>().Queryable.First(),
                Account = Repository.OfType<Account>().Queryable.First()
            });
            costShareRecord.AddEntry(new Entry
            {
                Comment = "AnotherValid",
                Record = Repository.OfType<Record>().Queryable.First(),
                FundType = Repository.OfType<FundType>().Queryable.First(),
                Project = Repository.OfType<Project>().Queryable.First(),
                Account = Repository.OfType<Account>().Queryable.First()
            });
            Repository.OfType<CostShare>().EnsurePersistent(costShareRecord);
            Assert.AreEqual(false, costShareRecord.IsTransient());

        }
        #endregion Valid Tests

        #region Invalid Tests

        /// <summary>
        /// Cost share with invalid month throws exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareWithInvalidMonthThrowsException()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Month = 0;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Month: must be between 1 and 12");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }
                throw;
            }
        }

        #region Month Tests

        /// <summary>
        /// Cost share record does not save with month of zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithMonthOfZero()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Month = 0;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Month: must be between 1 and 12");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }
                throw;
            }
        }
        /// <summary>
        /// CostShare not save with month less than zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithMonthLessThanZero()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Month = -1;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Month: must be between 1 and 12");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }
        /// <summary>
        /// CostShare record does not save with month greater than 12.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithMonthGreaterThan12()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Month = 13;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Month: must be between 1 and 12");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }
        #endregion Month Tests

        #region Year Tests
        /// <summary>
        /// CostShare record does not save with year of zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithYearOfZero()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Year = 0;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Year: must be greater than or equal to 1");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// CostShare record does not save with year less than zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithYearLessThanZero()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Year = -1;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Year: must be greater than or equal to 1");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }
        #endregion Year Tests

        #region User Tests
        /// <summary>
        /// CostShare record does not save with null user.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithNullUser()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.User = null;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("User: may not be empty");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Cost share record does not commit with new user value.
        /// This would not throw an exception
        /// </summary>
        //[TestMethod, Ignore]
        //[ExpectedException(typeof(NHibernate.TransientObjectException))]
        //public void CostShareRecordDoesNotCommitWithNewUserValue()
        //{
        //    CostShare costShare = null;
        //    try
        //    {
        //        costShare = CreateValidCostShare();
        //        costShare.User = new User();
        //        Repository.OfType<CostShare>().EnsurePersistent(costShare);
        //        Assert.IsFalse(costShare.IsTransient());
        //        Repository.OfType<CostShare>().DbContext.CommitChanges();
        //    }
        //    catch (Exception message)
        //    {
        //        Assert.IsNotNull(costShare);
        //        Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Record, Entity: FSNEP.Core.Domain.Record", message.Message);
        //        throw;
        //    }
        //}
        #endregion User Tests

        #region Status Tests
        /// <summary>
        /// CostShare record does not save with null status.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithNullStatus()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Status = null;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Status: may not be empty");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Cost share record does not commit with new Status value.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CostShareRecordDoesNotCommitWithNewStatusValue()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Status = new Status();
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
                Assert.IsFalse(costShare.IsTransient());

                Repository.OfType<CostShare>().DbContext.CommitChanges();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(costShare);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Status, Entity: FSNEP.Core.Domain.Status", message.Message);
                throw;
            }
        }
        #endregion Status Tests

        #region Review Comment Tests
        /// <summary>
        /// CostShare record does not save with review comments too long.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithReviewCommentsTooLong()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                var sb = new StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    sb.Append(
                    "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ");
                }
                sb.Append("123456789 123");
                costShare.ReviewComment = sb.ToString();
                Assert.AreEqual(513, costShare.ReviewComment.Length);

                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("ReviewComment: length must be between 0 and 512");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }
        #endregion Review Comment Tests

        #region Entries Tests
        /// <summary>
        /// CostShare record does not save with null entries.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareRecordDoesNotSaveWithNullEntries()
        {
            CostShare costShare = null;
            try
            {
                costShare = CreateValidCostShare();
                costShare.Entries = null;
                Repository.OfType<CostShare>().EnsurePersistent(costShare);
            }
            catch (Exception)
            {
                Assert.IsNotNull(costShare);
                if (costShare != null)
                {
                    var results = costShare.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Entries: may not be empty");
                    Assert.IsTrue(costShare.IsTransient());
                    Assert.IsFalse(costShare.IsValid());
                }

                throw;
            }
        }

        #endregion Entries Tests


        #endregion Invalid Tests
        

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
