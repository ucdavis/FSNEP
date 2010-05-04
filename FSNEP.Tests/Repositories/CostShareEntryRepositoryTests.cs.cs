﻿using System.Linq;
using FSNEP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
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
            var record = new Record
                             {
                                 Month = 12,
                                 ReviewComment = "reviewComment",
                                 Status = Repository.OfType<Status>().Queryable.First(),
                                 User = Repository.OfType<User>().Queryable.First(),
                                 Year = 2009
                             };

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
