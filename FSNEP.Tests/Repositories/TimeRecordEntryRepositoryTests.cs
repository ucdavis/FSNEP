using System;
using System.Linq;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class TimeRecordEntryRepositoryTests : AbstractRepositoryEntryTests<TimeRecordEntry>
    {
        #region Init
        protected override TimeRecordEntry GetValid(int? counter)
        {
            var timeRecordEntry = CreateValidEntities.TimeRecordEntry(counter);
            timeRecordEntry.Record = Repository.OfType<Record>().Queryable.First();
            timeRecordEntry.Project = Repository.OfType<Project>().Queryable.First();
            timeRecordEntry.FundType = Repository.OfType<FundType>().Queryable.First();
            timeRecordEntry.Account = Repository.OfType<Account>().Queryable.First();
            timeRecordEntry.ActivityType = Repository.OfType<ActivityType>().Queryable.First();

            return timeRecordEntry;
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            base.LoadData();

            LoadRecordRecords();
            LoadActivityCategory();
            LoadActivityType();

            Repository.OfType<CostShareEntry>().DbContext.BeginTransaction();
            LoadRecords();
            Repository.OfType<CostShareEntry>().DbContext.CommitTransaction();
        }

        private void LoadActivityCategory()
        {
            Repository.OfType<ActivityCategory>().DbContext.BeginTransaction();
            var activityCategory = CreateValidEntities.ActivityCategory(null);

            Repository.OfType<ActivityCategory>().EnsurePersistent(activityCategory);
            Repository.OfType<ActivityCategory>().DbContext.CommitTransaction();
        }

        private void LoadActivityType()
        {
            Repository.OfType<ActivityType>().DbContext.BeginTransaction();
            var activityType = CreateValidEntities.ActivityType(null);
            activityType.ActivityCategory = Repository.OfType<ActivityCategory>().Queryable.First();
            Repository.OfType<ActivityType>().EnsurePersistent(activityType);
            Repository.OfType<ActivityType>().DbContext.CommitTransaction();
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


        #endregion Init

        #region Valid Tests

        #region Hours Tests

        /// <summary>
        /// Determines whether this instance [can save valid time record entry zero hours].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryZeroHours()
        {
            var timerecordEntry = GetValid(null);
            timerecordEntry.Hours = 0;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }
        /// <summary>
        /// Determines whether this instance [can save valid time record entry 24 hours].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntry24Hours()
        {
            var timerecordEntry = GetValid(null);
            timerecordEntry.Hours = 24;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }
        /// <summary>
        /// Determines whether this instance [can save valid time record entry minus 24 hours].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryMinus24Hours()
        {
            //(Task 509)
            var timerecordEntry = GetValid(null);
            timerecordEntry.Hours = -24;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }

        #endregion Hours Tests

        #region Date Tests (Day of Month)

        /// <summary>
        /// Determines whether this instance [can save valid time record entry date of 1].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryDateOf1()
        {
            var timerecordEntry = GetValid(null);
            timerecordEntry.Date = 1;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }
        /// <summary>
        /// Determines whether this instance [can save valid time record entry date of 31].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryDateOf31()
        {
            var timerecordEntry = GetValid(null);
            timerecordEntry.Date = 31;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }

        #endregion Date Tests (Day of Month)

        #region AdjustmentDate Tests

        /// <summary>
        /// Determines whether this instance [can save valid time record entry null adjustment date].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryNullAdjustmentDate()
        {
            var timerecordEntry = GetValid(null);
            timerecordEntry.AdjustmentDate = null;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }
        /// <summary>
        /// Determines whether this instance [can save valid time record entry null adjustment date].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryValidAdjustmentDate()
        {
            var timerecordEntry = GetValid(null);
            timerecordEntry.AdjustmentDate = DateTime.Now;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }

        #endregion AdjustmentDate Tests

        #endregion Valid Tests

        #region Invalid Tests

        #region Date Tests
        /// <summary>
        /// Time record entry does not save with date (day) of zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithDateOfZero()
        {
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = GetValid(null);
                timerecordEntry.Date = 0;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);

                var results = timerecordEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Date: must be between 1 and 31");
                Assert.IsTrue(timerecordEntry.IsTransient());
                Assert.IsFalse(timerecordEntry.IsValid());

                throw;
            }
        }
        /// <summary>
        /// Time record entry does not save with date (day) of 32.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithDateOf32()
        {
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = GetValid(null);
                timerecordEntry.Date = 32;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);

                var results = timerecordEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Date: must be between 1 and 31");
                Assert.IsTrue(timerecordEntry.IsTransient());
                Assert.IsFalse(timerecordEntry.IsValid());


                throw;
            }
        }
        #endregion Date Tests

        #region Hour Tests
        /// <summary>
        /// Time record entry does not save with hours greater than 24.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithHoursGreaterThan24()
        {
            //(Task 509)
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = GetValid(null);
                timerecordEntry.Hours = 24.01;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);

                var results = timerecordEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Hours: must be between -24 and 24");
                Assert.IsTrue(timerecordEntry.IsTransient());
                Assert.IsFalse(timerecordEntry.IsValid());


                throw;
            }
        }

        /// <summary>
        /// Time record entry does not save with hours greater than 24.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithHoursLessThanMinus24()
        {
            //(Task 509)
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = GetValid(null);
                timerecordEntry.Hours = -24.01;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);

                var results = timerecordEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Hours: must be between -24 and 24");
                Assert.IsTrue(timerecordEntry.IsTransient());
                Assert.IsFalse(timerecordEntry.IsValid());


                throw;
            }
        }
        #endregion Hour Tests

        #region ActivityType Tests

        /// <summary>
        /// Time record entry does not save with null activity.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithNullActivityType()
        {
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = GetValid(null);
                timerecordEntry.ActivityType = null;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);

                var results = timerecordEntry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ActivityType: may not be null");
                Assert.IsTrue(timerecordEntry.IsTransient());
                Assert.IsFalse(timerecordEntry.IsValid());


                throw;
            }
        }
        #endregion ActivityType Tests

        #endregion Invalid Tests
    }
}