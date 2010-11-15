using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class CostShareEntryRepositoryTests : AbstractRepositoryEntryTests<CostShareEntry>
    {        

        #region Init
        protected override CostShareEntry GetValid(int? counter)
        {
            var costShareEntry = CreateValidEntities.CostShareEntry(counter);
            costShareEntry.Record = Repository.OfType<Record>().Queryable.First();
            costShareEntry.Project = Repository.OfType<Project>().Queryable.First();
            costShareEntry.FundType = Repository.OfType<FundType>().Queryable.First();
            costShareEntry.Account = Repository.OfType<Account>().Queryable.First();
            costShareEntry.ExpenseType = Repository.OfType<ExpenseType>().Queryable.First();

            return costShareEntry;
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            base.LoadData();

            LoadExpenseType();
            LoadRecordRecords();

            Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
            LoadRecords();
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }

        private void LoadRecordRecords()
        {
            Repository.OfType<Record>().DbContext.BeginTransaction();
            var record = CreateValidEntities.Record(null);
            record.Status = Repository.OfType<Status>().Queryable.First();
            record.User = Repository.OfType<User>().Queryable.First();
            Repository.OfType<Record>().EnsurePersistent(record);
            Repository.OfType<Record>().DbContext.CommitTransaction();
        }

        private void LoadExpenseType()
        {
            Repository.OfType<ExpenseType>().DbContext.BeginTransaction();
            var expenseType = CreateValidEntities.ExpenseType(null);
            Repository.OfType<ExpenseType>().EnsurePersistent(expenseType);
            Repository.OfType<ExpenseType>().DbContext.CommitTransaction();
        }

        #endregion Init

        #region CostShareSpecific Valid Tests

        #region Amount Tests

        /// <summary>
        /// Determines whether this instance [can save cost share entry with valid amount values].
        /// </summary>
        [TestMethod] //Task 509, Ammount can now have negative values.
        public void CanSaveCostShareEntryWithValidAmountValues()
        {
            double[] validAmounts = { -100000, -2, -1, -0.0001, -0.01, 0, 0.01, 0.0001, 1, 2, 10000000 };
            foreach (var validAmount in validAmounts)
            {
                Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
                var costShareEntry = GetValid(null);
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
            var costShareEntry = GetValid(null);
            var sb = new StringBuilder();
            for (int i = 0; i < 12; i++)
            {
                sb.Append("1234567890");
            }
            sb.Append("12345678");
            costShareEntry.Description = sb.ToString();
            Assert.AreEqual(128, costShareEntry.Description.Length);
            Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            Assert.AreEqual(false, costShareEntry.IsTransient());
            Assert.IsTrue(costShareEntry.IsValid());
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }

        #endregion Description Tests

        #endregion CostShareSpecific Valid Tests

        #region CostShareSpecific Invalid Tests

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
                costShareEntry = GetValid(null);
                costShareEntry.ExpenseType = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ExpenseType: may not be null");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

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
                costShareEntry = GetValid(null);
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
                costShareEntry = GetValid(null);
                costShareEntry.Description = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: may not be null or empty");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

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
                costShareEntry = GetValid(null);
                costShareEntry.Description = string.Empty;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: may not be null or empty");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

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
                costShareEntry = GetValid(null);
                costShareEntry.Description = "   ";
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: may not be null or empty");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

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
                costShareEntry = GetValid(null);
                var sb = new StringBuilder();
                for (int i = 0; i < 12; i++)
                {
                    sb.Append("1234567890");
                }
                sb.Append("123456789");
                costShareEntry.Description = sb.ToString();
                Assert.AreEqual(129, costShareEntry.Description.Length);
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Description: length must be between 0 and 128");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

                throw;
            }
        }

        #endregion Description Tests

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
                costShareEntry = GetValid(null);
                costShareEntry.Project = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Project: may not be null");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

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
                costShareEntry = GetValid(null);
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
                costShareEntry = GetValid(null);
                costShareEntry.FundType = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FundType: may not be null");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

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
                costShareEntry = GetValid(null);
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
                costShareEntry = GetValid(null);
                costShareEntry.Account = null;
                Repository.OfType<CostShareEntry>().EnsurePersistent(costShareEntry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(costShareEntry);

                var results = costShareEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Account: may not be null");
                Assert.IsTrue(costShareEntry.IsTransient());
                Assert.IsFalse(costShareEntry.IsValid());

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
                costShareEntry = GetValid(null);
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

        #endregion CostShareSpecific Invalid Tests

    }
}