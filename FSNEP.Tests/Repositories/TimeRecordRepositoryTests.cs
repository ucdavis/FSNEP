using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using FSNEP.Tests.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class TimeRecordRepositoryTests : AbstractRepositoryRecordTests<TimeRecord>
    {

        #region Init
        /// <summary>
        /// Gets the valid entity with Record as the base type.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected override TimeRecord GetValid(int? counter)
        {
            var timeRecord = CreateValidEntities.TimeRecord(counter);
            timeRecord.Status = Repository.OfType<Status>().Queryable.First();
            timeRecord.User = Repository.OfType<User>().Queryable.First();
            return timeRecord;
        }


        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            base.LoadData();

            Repository.OfType<TimeRecord>().DbContext.BeginTransaction();
            LoadRecords();
            Repository.OfType<TimeRecord>().DbContext.CommitTransaction();
        }

        #endregion Init

        #region Valid Tests

        #region TargetHours Tests

        [TestMethod]
        public void TargetHoursReturnsZeroWhenHoursInMonthIsNull()
        {
            var timeRecord = CreateValidEntities.TimeRecord(null);
            timeRecord.HoursInMonth = null;
            timeRecord.User.FTE = 1;

            Assert.AreEqual(0, timeRecord.TargetHours);
        }

        [TestMethod]
        public void TargetHoursReturnsNonZeroWhenHoursInMonthHasValues()
        {
            var hoursInMonth = CreateValidEntities.HoursInMonth();
            hoursInMonth.Hours = 40;

            var timeRecord = CreateValidEntities.TimeRecord(null);
            timeRecord.HoursInMonth = hoursInMonth;
            timeRecord.User.FTE = 0.5;

            Assert.AreEqual(20, timeRecord.TargetHours);
        }

        #endregion TargetHours Tests

        #endregion Valid Tests

        #region Invalid Salary Tests

        /// <summary>
        /// Determines whether this instance [can not save time record with salary of zero].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveTimeRecordWithSalaryOfZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = GetValid(null);
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
        /// Determines whether this instance [can not save time record with salary of 0.001].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveTimeRecordWithSalaryOf001()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = GetValid(null);
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
        /// Determines whether this instance [can not save time record with salary less than zero].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveTimeRecordWithSalaryLessThanZero()
        {
            TimeRecord timeRecord = null;
            try
            {
                timeRecord = GetValid(null);
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
        #endregion Invalid Salary Tests

    }
}