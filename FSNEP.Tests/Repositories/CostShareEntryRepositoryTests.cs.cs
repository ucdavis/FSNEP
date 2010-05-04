using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing.Extensions;
using RepositoryTestBase = FSNEP.Tests.Core.RepositoryTestBase;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class CostShareEntryRepositoryTests : RepositoryTestBase
    {
        #region Init

        protected override void LoadData()
        {
            base.LoadData();

            using (var ts = new TransactionScope())
            {
                LoadRecords();
                LoadExpenses();
                LoadCostShareEntryRecords();
                ts.CommitTransaction();
            }
        }

        /// <summary>
        /// Loads the expenses.
        /// </summary>
        private void LoadExpenses()
        {
            var expense = new ExpenseType {IsActive = true, Name = "name"};
            Repository.OfType<ExpenseType>().EnsurePersistent(expense);
        }

        /// <summary>
        /// Loads the cost share entry records.
        /// </summary>
        private void LoadCostShareEntryRecords()
        {
            for (int i = 0; i < 5; i++)
            { 
                var costShareEntry = CreateValidCostShareEntry(i);
                
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
        }

        /// <summary>
        /// Loads the record entry.
        /// </summary>
        private void LoadRecords()
        {
            //var record = new Record
            //                 {
            //                     Month = 12,
            //                     ReviewComment = "reviewComment",
            //                     Status = Repository.OfType<Status>().Queryable.First(),
            //                     User = Repository.OfType<User>().Queryable.First(),
            //                     Year = 2009
            //                 };
            var record = CreateValidEntities.Record(null);
            record.Status = Repository.OfType<Status>().Queryable.First();
            record.User = Repository.OfType<User>().Queryable.First();

            Repository.OfType<Record>().EnsurePersistent(record);
        }
        #endregion Init

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can save valid cost share entry].
        /// </summary>
        [TestMethod]
        public void CanSaveValidCostShareEntry()
        {
            var costShareEntry = CreateValidCostShareEntry(null);
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);

            Assert.AreEqual(false, costShareEntry.IsTransient());
        }

        /// <summary>
        /// Determines whether this instance [can read cost share entry records].
        /// </summary>
        [TestMethod]
        public void CanReadCostShareEntryRecords()
        {
            var costShareEntryRecords = Repository.OfType<CostShareEntry>().GetAll().ToList();
            Assert.IsNotNull(costShareEntryRecords);
            Assert.AreEqual(5, costShareEntryRecords.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("Comment" + (i + 1), costShareEntryRecords[i].Comment);
            }
        }

        /// <summary>
        /// Determines whether this instance [can query cost share entry records].
        /// </summary>
        [TestMethod]
        public void CanQueryCostShareEntryRecords()
        {
            var costShareEntryRecords =
                Repository.OfType<CostShareEntry>().Queryable.Where(a => a.Comment.EndsWith("3")).ToList();
            Assert.IsNotNull(costShareEntryRecords);
            Assert.AreEqual(1, costShareEntryRecords.Count);
            Assert.AreEqual("Comment3", costShareEntryRecords[0].Comment);
        }


        /// <summary>
        /// Determines whether this instance [can update cost share entry record].
        /// </summary>
        [TestMethod]
        public void CanUpdateCostShareEntryRecord()
        {
            var costShareEntryRecord =
                Repository.OfType<CostShareEntry>().Queryable.Where(a => a.Comment.EndsWith("3")).ToList()[0];

            Assert.AreEqual("Comment3", costShareEntryRecord.Comment);

            costShareEntryRecord.Comment = "Updated";
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntryRecord);

            var costShareEntryRecords = Repository.OfType<CostShareEntry>().GetAll().ToList();
            Assert.AreEqual(5, costShareEntryRecords.Count);
            Assert.AreEqual("Updated", costShareEntryRecords[2].Comment);
            Assert.AreEqual("Comment4", costShareEntryRecords[3].Comment);
        }

        /// <summary>
        /// Determines whether this instance [can delete cost share entry record].
        /// </summary>
        [TestMethod]
        public void CanDeleteCostShareEntryRecord()
        {
            var costShareEntryRecord =
                Repository.OfType<CostShareEntry>().Queryable.Where(a => a.Comment.EndsWith("3")).ToList()[0];

            Assert.AreEqual("Comment3", costShareEntryRecord.Comment);

            using (var ts = new TransactionScope())
            {
                Repository.OfType<CostShareEntry>().Remove(costShareEntryRecord);

                ts.CommitTransaction();
            }

            var costShareEntryRecords = Repository.OfType<CostShareEntry>().GetAll().ToList();
            Assert.AreEqual(4, costShareEntryRecords.Count);
            Assert.AreEqual("Comment1", costShareEntryRecords[0].Comment);
            Assert.AreEqual("Comment2", costShareEntryRecords[1].Comment);
            Assert.AreEqual("Comment4", costShareEntryRecords[2].Comment);
            Assert.AreEqual("Comment5", costShareEntryRecords[3].Comment);
        }

        #endregion CRUD Tests

        #region Valid Data Tests

        #region Amount Tests

        /// <summary>
        /// Determines whether this instance [can save cost share entry with valid amount values].
        /// </summary>
        [TestMethod]
        public void CanSaveCostShareEntryWithValidAmountValues()
        {
            double[] validAmounts = {0, 0.01, 0.0001, 1, 2, 10000000};
            foreach (var validAmount in validAmounts)
            {
                Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
                var costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Amount = validAmount; 
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
                Assert.AreEqual(false, costShareEntry.IsTransient());
                Assert.IsTrue(costShareEntry.IsValid());
                Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
            }
        }
        #endregion Amount Tests

        #region Description Tests

        /// <summary>
        /// Cost share entry Save with description of 128 characters.
        /// </summary>
        [TestMethod]
        public void CostShareEntrySavesWithDescriptionOf128Characters()
        {
            Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
            var costShareEntry = CreateValidCostShareEntry(null);
            costShareEntry.Description = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 12345678";
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            Assert.AreEqual(false, costShareEntry.IsTransient());
            Assert.IsTrue(costShareEntry.IsValid());
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }
        
        #endregion Description Tests

        #region Comment Tests

        /// <summary>
        /// Costs the share entry saves with comment of 256 characters.
        /// </summary>
        [TestMethod]
        public void CostShareEntrySavesWithCommentOf256Characters()
        {
            Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
            var costShareEntry = CreateValidCostShareEntry(null);
            var sb = new StringBuilder();
            for (int i = 0; i < 25; i++)
            {
                sb.Append("1234567890");
            }
            sb.Append("123456");
            costShareEntry.Comment = sb.ToString();
            Assert.AreEqual(256, costShareEntry.Comment.Length);
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            Assert.AreEqual(false, costShareEntry.IsTransient());
            Assert.IsTrue(costShareEntry.IsValid());
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }

        [TestMethod]
        public void CostShareEntrySavesWithNullComment()
        {
            //TODO: Update validation to allow test to pass (Task 509)
            Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
            var costShareEntry = CreateValidCostShareEntry(null);
            costShareEntry.Comment = null;
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            Assert.AreEqual(false, costShareEntry.IsTransient());
            Assert.IsTrue(costShareEntry.IsValid());
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }

        [TestMethod]
        public void CostShareEntrySavesWithEmptyComment()
        {
            //TODO: Update validation to allow test to pass (Task 509)
            Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
            var costShareEntry = CreateValidCostShareEntry(null);
            costShareEntry.Comment = string.Empty;
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            Assert.AreEqual(false, costShareEntry.IsTransient());
            Assert.IsTrue(costShareEntry.IsValid());
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }

        [TestMethod]
        public void CostShareEntrySavesWithSpacesOnlyComment()
        {
            //TODO: Update validation to allow test to pass (Task 509)
            Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
            var costShareEntry = CreateValidCostShareEntry(null);
            costShareEntry.Comment = " ";
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            Assert.AreEqual(false, costShareEntry.IsTransient());
            Assert.IsTrue(costShareEntry.IsValid());
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }

        #endregion Comment Tests

        #endregion Valid Data Tests

        #region Invalid Data Tests

        #region ExpenseType Tests

        /// <summary>
        /// Expense type with null value does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithNullExpenseTypeDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.ExpenseType = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("ExpenseType: may not be empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// Expense type with new expense type value does not commit.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CostShareEntryWithNewExpenseTypeValueDoesNotCommit()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.ExpenseType = new ExpenseType();
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
                Assert.IsFalse(costShareEntry.IsTransient());

                Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(costShareEntry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.ExpenseType, Entity: ", message.Message);
                throw;
            }
        }

        #endregion ExpenseType Tests

        #region Amount Tests

        /// <summary>
        /// Amount does not save with amount less than zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryDoesNotSaveWithAmountLessThanZero()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Amount = -1;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Amount: must be greater than or equal to 0");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        #endregion Amount Tests

        #region Description Tests

        /// <summary>
        /// Cost share entry does not save with null description.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryDoesNotSaveWithNullDescription()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Description = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Description: may not be null or empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry does not save with empty description.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryDoesNotSaveWithEmptyDescription()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Description = string.Empty;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Description: may not be null or empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry does not save with spaces only in description.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryDoesNotSaveWithSpacesOnlyInDescription()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Description = "   ";
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Description: may not be null or empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry does not save with more than 128 characters in description.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryDoesNotSaveWithMoreThan128CharactersInDescription()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Description = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789";
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Description: length must be between 0 and 128");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        #endregion Description Tests

        #region Entry Record Tests

        #region Record Tests
        
        /// <summary>
        /// Cost share entry with null record entity does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithNullRecordDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Record = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Record: may not be empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry with new record value does not commit.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CostShareEntryWithNewRecordValueDoesNotCommit()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Record = new Record();
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
                Assert.IsFalse(costShareEntry.IsTransient());

                Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(costShareEntry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Record, Entity: FSNEP.Core.Domain.Record", message.Message);
                throw;
            }
        }
        #endregion Record Tests

        #region Comment Tests

        /// <summary>
        /// Cost share entry with null comment does not save.
        /// </summary>
        [TestMethod, Ignore] //Task 509, Test no longer valid
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithNullCommentDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Comment = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Comment: may not be null or empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry with empty comment does not save.
        /// </summary>
        [TestMethod, Ignore] //Task 509, Test no longer valid
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithEmptyCommentDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Comment = string.Empty;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Comment: may not be null or empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry with spaces only in coment does not save.
        /// </summary>
        [TestMethod, Ignore] //Task 509, Test no longer valid
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithSpacesOnlyCommentDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Comment = "   ";
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Comment: may not be null or empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry with coment of 257 characters does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithTooLongCommentDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                var sb = new StringBuilder();
                for (int i = 0; i < 25; i++)
                {
                    sb.Append("1234567890");
                }
                sb.Append("1234567");
                costShareEntry.Comment = sb.ToString();
                Assert.AreEqual(257, costShareEntry.Comment.Length);
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Comment: length must be between 0 and 256");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        #endregion Comment Tests

        #region Project Tests

        /// <summary>
        /// Cost share entry with null project entity does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithNullProjectDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Project = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Project: may not be empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry with new project value does not commit.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CostShareEntryWithNewProjectValueDoesNotCommit()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Project = new Project();
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
                Assert.IsFalse(costShareEntry.IsTransient());

                Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(costShareEntry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Project, Entity: ", message.Message);
                throw;
            }
        }

        #endregion Project Tests

        #region FundType Tests

        /// <summary>
        /// Cost share entry with null fundType entity does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithNullFundTypeDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.FundType = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("FundType: may not be empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry with new fundType value does not commit.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CostShareEntryWithNewFundTypeValueDoesNotCommit()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.FundType = new FundType();
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
                Assert.IsFalse(costShareEntry.IsTransient());

                Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(costShareEntry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.FundType, Entity: FSNEP.Core.Domain.FundType", message.Message);
                throw;
            }
        }

        #endregion FundType Tests

        #region Account Tests

        /// <summary>
        /// Cost share entry with null Account entity does not save.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CostShareEntryWithNullAccountDoesNotSave()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Account = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);
                if (costShareEntry != null)
                {
                    var results = costShareEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Account: may not be empty");
                    Assert.IsTrue(costShareEntry.IsTransient());
                    Assert.IsFalse(costShareEntry.IsValid());
                }
                throw;
            }
        }

        /// <summary>
        /// Cost share entry with new acount value does not commit.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CostShareEntryWithNewAccountValueDoesNotCommit()
        {
            CostShareEntry costShareEntry = null;

            try
            {
                Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
                costShareEntry = CreateValidCostShareEntry(null);
                costShareEntry.Account = new Account();
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
                Assert.IsFalse(costShareEntry.IsTransient());

                Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(costShareEntry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Account, Entity:  (0%)", message.Message);
                throw;
            }
        }

        #endregion Account Tests

        #endregion Entry Record Tests

        #endregion Invalid Data Tests

        #region Helper Methods

        /// <summary>
        /// Creates the valid cost share entry.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private CostShareEntry CreateValidCostShareEntry(int? i)
        {
            var costShareEntry = new CostShareEntry
                                     {
                                         Amount = 100,
                                         //ExpenseType = new ExpenseType(),
                                         ExpenseType = Repository.OfType<ExpenseType>().Queryable.First(),
                                         Account = Repository.OfType<Account>().Queryable.First(),
                                         FundType = Repository.OfType<FundType>().Queryable.First(),
                                         Project = Repository.OfType<Project>().Queryable.First(),
                                         Record = Repository.OfType<Record>().Queryable.First()
                                     };
            if(i == null)
            {
                costShareEntry.Comment = "Comment";
                costShareEntry.Description = "Description";
            }
            else
            {
                costShareEntry.Comment = "Comment" + (i + 1);
                costShareEntry.Description = "Description" + (i + 1); 
            }            
            

            return costShareEntry;
        }

        #endregion Helper Methods
    }
}
