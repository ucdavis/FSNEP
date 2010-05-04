using System;
using System.Linq;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class TimeRecordEntryRepositoryTests : RepositoryTestBase
    {
        private const int ValidYear = 2009;
        private const int ValidMonth = 6;
        private const int ValidDate = 25;
        private const string ValidComment = "Comment";
        private const double ValidHours = 6.5;

        #region Init
        protected override void LoadData()
        {
            base.LoadData();

            LoadTimeRecords();
        }

        private void LoadTimeRecords()
        {
            var record = new TimeRecord
            {
                Month = ValidMonth,
                Year = ValidYear,
                Salary = 200,
                Status = Repository.OfType<Status>().Queryable.First(),
                User = Repository.OfType<User>().Queryable.First()
            };

            Repository.OfType<TimeRecord>().EnsurePersistent(record);
        }
        #endregion Init

        #region Valid Time record Entry Tests
        /// <summary>
        /// Determines whether this instance [can save valid time record entry].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntry()
        {
            var timerecordEntry = CreateValidTimeRecordEntry();

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }


        /// <summary>
        /// Determines whether this instance [can save valid time record entry zero hours].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryZeroHours()
        {
            var timerecordEntry = CreateValidTimeRecordEntry();
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
            var timerecordEntry = CreateValidTimeRecordEntry();
            timerecordEntry.Hours = 24;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }
        /// <summary>
        /// Determines whether this instance [can save valid time record entry date of 1].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryDateOf1()
        {
            var timerecordEntry = CreateValidTimeRecordEntry();
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
            var timerecordEntry = CreateValidTimeRecordEntry();
            timerecordEntry.Date = 31;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }
        /// <summary>
        /// Determines whether this instance [can save valid time record entry null adjustment date].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordEntryNullAdjustmentDate()
        {
            var timerecordEntry = CreateValidTimeRecordEntry();
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
            var timerecordEntry = CreateValidTimeRecordEntry();
            timerecordEntry.AdjustmentDate = DateTime.Now;

            Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);

            Assert.AreEqual(false, timerecordEntry.IsTransient());
        }
        #endregion Valid Time record Entry Tests


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
                timerecordEntry = CreateValidTimeRecordEntry();
                timerecordEntry.Date = 0;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);
                if (timerecordEntry != null)
                {
                    var results = timerecordEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Date: must be between 1 and 31");
                    Assert.IsTrue(timerecordEntry.IsTransient());
                    Assert.IsFalse(timerecordEntry.IsValid());
                }

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
                timerecordEntry = CreateValidTimeRecordEntry();
                timerecordEntry.Date = 32;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);
                if (timerecordEntry != null)
                {
                    var results = timerecordEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Date: must be between 1 and 31");
                    Assert.IsTrue(timerecordEntry.IsTransient());
                    Assert.IsFalse(timerecordEntry.IsValid());
                }

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
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = CreateValidTimeRecordEntry();
                timerecordEntry.Hours = 24.01;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);
                if (timerecordEntry != null)
                {
                    var results = timerecordEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Hours: must be between 0 and 24");
                    Assert.IsTrue(timerecordEntry.IsTransient());
                    Assert.IsFalse(timerecordEntry.IsValid());
                }

                throw;
            }
        }
        #endregion Hour Tests

        #region Record Tests
        /// <summary>
        /// Time record entry does not save with null record.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithNullRecord()
        {
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = CreateValidTimeRecordEntry();
                timerecordEntry.Record = null;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);
                if (timerecordEntry != null)
                {
                    var results = timerecordEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Record: may not be empty");
                    Assert.IsTrue(timerecordEntry.IsTransient());
                    Assert.IsFalse(timerecordEntry.IsValid());
                }

                throw;
            }
        }
        #endregion Record Tests

        #region Comment Tests
        /// <summary>
        /// Time record entry does not save with null comment.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithNullComment()
        {
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = CreateValidTimeRecordEntry();
                timerecordEntry.Comment = null;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);
                if (timerecordEntry != null)
                {
                    var results = timerecordEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Comment: may not be null or empty");
                    Assert.IsTrue(timerecordEntry.IsTransient());
                    Assert.IsFalse(timerecordEntry.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Time record entry does not save with empty comment.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithEmptyComment()
        {
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = CreateValidTimeRecordEntry();
                timerecordEntry.Comment = string.Empty;
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);
                if (timerecordEntry != null)
                {
                    var results = timerecordEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Comment: may not be null or empty");
                    Assert.IsTrue(timerecordEntry.IsTransient());
                    Assert.IsFalse(timerecordEntry.IsValid());
                }

                throw;
            }
        }
        /// <summary>
        /// Time record entry does not save with spaces only in comment.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordEntryDoesNotSaveWithSpacesOnlyComment()
        {
            TimeRecordEntry timerecordEntry = null;
            try
            {
                timerecordEntry = CreateValidTimeRecordEntry();
                timerecordEntry.Comment = " ";
                Repository.OfType<TimeRecordEntry>().EnsurePersistent(timerecordEntry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timerecordEntry);
                if (timerecordEntry != null)
                {
                    var results = timerecordEntry.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Comment: may not be null or empty");
                    Assert.IsTrue(timerecordEntry.IsTransient());
                    Assert.IsFalse(timerecordEntry.IsValid());
                }

                throw;
            }
        }
        #endregion Comment Tests

        #region Helper Methods
        /// <summary>
        /// Creates the valid timerecord entry.
        /// </summary>
        /// <returns></returns>
        private TimeRecordEntry CreateValidTimeRecordEntry()
        {
            return new TimeRecordEntry
                       {
                           Date = ValidDate,
                           Hours = ValidHours,
                           Comment = ValidComment,
                           Record = Repository.OfType<TimeRecord>().Queryable.First(),
                           FundType = Repository.OfType<FundType>().Queryable.First(),
                           Project = Repository.OfType<Project>().Queryable.First(),
                           Account = Repository.OfType<Account>().Queryable.First()
                       };
        }

        #endregion Helper Methods
    }
}