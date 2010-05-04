using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class TimeRecordRepositoryTests : RepositoryTestBase
    {
        private const int ValidYear = 2009;
        private const int ValidMonth = 6;

        #region Init
        protected override void LoadData()
        {
            base.LoadData();

            using (var ts = new TransactionScope())
            {
                LoadStatus();
                LoadRecords();

                ts.CommitTransaction();
            }

            NHibernateSessionManager.Instance.GetSession().Flush();
        }

        private void LoadRecords()
        {
            var record = CreateValidRecord();

            Repository.OfType<Record>().EnsurePersistent(record);
        }

        private Record CreateValidRecord()
        {
            var record = new Record
            {
                Month = ValidMonth,
                Year = ValidYear,
                Status = Repository.OfType<Status>().Queryable.First(),
                User = Repository.OfType<User>().Queryable.First()
            };

            return record;
        }

        public void LoadStatus()
        {
            var status1 = new Status { NameOption = Status.Option.Approved };
            var status2 = new Status { NameOption = Status.Option.Current };

            var statusRepository = Repository.OfType<Status>();

            statusRepository.EnsurePersistent(status1);
            statusRepository.EnsurePersistent(status2);
        }
        #endregion Init

        #region Valid Time Record Tests
        /// <summary>
        /// Determines whether this instance [can save valid time record].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecord()
        {
            var timeRecord = CreateValidTimeRecord();

            Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);

            Assert.AreEqual(false, timeRecord.IsTransient());
        }

        /// <summary>
        /// Determines whether this instance [can save valid time record with null review comments].
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordWithNullReviewComments()
        {
            var timeRecord = CreateValidTimeRecord();
            timeRecord.ReviewComment = null;

            Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);

            Assert.AreEqual(false, timeRecord.IsTransient());
        }

        /// <summary>
        /// Time record saves with entries.
        /// </summary>
        [TestMethod]
        public void CanSaveValidTimeRecordWithEntries()
        {
            var timeRecord = CreateValidTimeRecord();
            timeRecord.AddEntry(new Entry
                                    {
                Comment = "Valid",
                Record = Repository.OfType<Record>().Queryable.First(),
                FundType = Repository.OfType<FundType>().Queryable.First(),
                Project = Repository.OfType<Project>().Queryable.First(),
                Account = Repository.OfType<Account>().Queryable.First()
            });
            timeRecord.AddEntry(new Entry
                                    {
                Comment = "AnotherValid",
                Record = Repository.OfType<Record>().Queryable.First(),
                FundType = Repository.OfType<FundType>().Queryable.First(),
                Project = Repository.OfType<Project>().Queryable.First(),
                Account = Repository.OfType<Account>().Queryable.First()
            });
            Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            Assert.AreEqual(false, timeRecord.IsTransient());

        }
        #endregion Valid Time Record Tests

        #region Salary Tests
        /// <summary>
        /// Time record does not save with salary of zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithSalaryOfZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Salary = 0;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Salary: Must be greater than zero");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Time record does not save with salary of 0.001.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithSalaryOf001()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Salary = 0.001;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Salary: Must be greater than zero");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Time record does not save with salary less than zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithSalaryLessThanZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Salary = -1;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Salary: Must be greater than zero");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        #endregion Salary Tests

        #region Month Tests
        /// <summary>
        /// Time record does not save with month of zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithMonthOfZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Month = 0;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Month: must be between 1 and 12");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        /// <summary>
        /// Time record does not save with month less than zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithMonthLessThanZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Month = -1;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Month: must be between 1 and 12");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        /// <summary>
        /// Time record does not save with month greater than 12.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithMonthGreaterThan12()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Month = 13;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Month: must be between 1 and 12");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        #endregion Month Tests

        #region Year Tests
        /// <summary>
        /// Time record does not save with year of zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithYearOfZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Year = 0;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Year: must be greater than or equal to 1");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Time record does not save with year less than zero.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithYearLessThanZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Year = -1;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Year: must be greater than or equal to 1");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        #endregion Year Tests

        #region User Tests
        /// <summary>
        /// Time record does not save with null user.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithNullUser()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.User = null;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("User: may not be empty");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        #endregion User Tests

        #region Status Tests
        /// <summary>
        /// Time record does not save with null status.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithNullStatus()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Status = null;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Status: may not be empty");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        #endregion Status Tests

        #region Review Comment Tests
        /// <summary>
        /// Time record does not save with review comments too long.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithReviewCommentsTooLong()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                var sb = new StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    sb.Append(
                    "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ");
                }
                sb.Append("123456789 123");                
                timeRecord.ReviewComment = sb.ToString();
                Assert.AreEqual(513, timeRecord.ReviewComment.Length); 

                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("ReviewComment: length must be between 0 and 512");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        #endregion Review Comment Tests

        #region Entries Tests
        /// <summary>
        /// Time record does not save with null entries.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void TimeRecordDoesNotSaveWithNullEntries()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = CreateValidTimeRecord();
                timeRecord.Entries = null;
                Repository.OfType<TimeRecord>().EnsurePersistent(timeRecord);
            }
            catch (Exception)
            {
                Assert.IsNotNull(timeRecord);
                if (timeRecord != null)
                {
                    var results = timeRecord.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Entries: may not be empty");
                    Assert.IsTrue(timeRecord.IsTransient());
                    Assert.IsFalse(timeRecord.IsValid());
                }

                throw;
            }
        }
        
        #endregion Entries Tests

        #region Helper Methods
        /// <summary>
        /// Creates the valid time record.
        /// </summary>
        /// <returns></returns>
        private TimeRecord CreateValidTimeRecord()
        {
            return new TimeRecord
            {
                Month = ValidMonth,
                Year = ValidYear,
                Salary = 200,
                Status = Repository.OfType<Status>().Queryable.First(),
                User = Repository.OfType<User>().Queryable.First(),
                ReviewComment = "A review is a review except when it isn't."
            };
        }

        #endregion Helper Methods
    }
}