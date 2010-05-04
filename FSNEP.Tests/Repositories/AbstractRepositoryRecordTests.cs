using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Testing.Extensions;
using RepositoryTestBase = FSNEP.Tests.Core.RepositoryTestBase;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public abstract class AbstractRepositoryRecordTests<T> : RepositoryTestBase where T : Record
    {
        #region Init

        /// <summary>
        /// Gets the valid entity with Record as the base type.
        /// </summary>
        /// <param name="counter">The counter.</param>
        /// <returns>A valid entity of type T</returns>
        protected abstract T GetValid(int? counter);

        /// <summary>
        /// Loads the records for CRUD Tests.
        /// </summary>
        /// <returns></returns>
        protected virtual void LoadRecords()
        {
            for (int i = 0; i < 5; i++)
            {
                var record = GetValid(i + 1);
                Repository.OfType<T>().EnsurePersistent(record);
            }
        }

        #endregion Init

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can save valid record].
        /// </summary>
        [TestMethod]
        public void CanSaveValidRecord()
        {
            var record = GetValid(null);
            Repository.OfType<T>().EnsurePersistent(record);

            Assert.AreEqual(false, record.IsTransient());
        }

        /// <summary>
        /// Determines whether this instance [can read records].
        /// </summary>
        [TestMethod]
        public void CanReadRecords()
        {
            var records = Repository.OfType<T>().GetAll().ToList();
            Assert.IsNotNull(records);
            Assert.AreEqual(5, records.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("ReviewComment" + (i + 1), records[i].ReviewComment);
            }
        }

        /// <summary>
        /// Determines whether this instance [can query records].
        /// </summary>
        [TestMethod]
        public void CanQueryRecords()
        {
            var records =
                Repository.OfType<T>().Queryable.Where(a => a.ReviewComment.EndsWith("3")).ToList();
            Assert.IsNotNull(records);
            Assert.AreEqual(1, records.Count);
            Assert.AreEqual("ReviewComment3", records[0].ReviewComment);
        }

        /// <summary>
        /// Determines whether this instance [can update record].
        /// </summary>
        [TestMethod]
        public void CanUpdateRecord()
        {
            var recordToUpdate =
                Repository.OfType<T>().Queryable.Where(a => a.ReviewComment.EndsWith("3")).ToList()[0];

            Assert.AreEqual("ReviewComment3", recordToUpdate.ReviewComment);

            recordToUpdate.ReviewComment = "Updated";
            Repository.OfType<T>().EnsurePersistent(recordToUpdate);

            var records = Repository.OfType<T>().GetAll().ToList();
            Assert.AreEqual(5, records.Count);
            Assert.AreEqual("Updated", records[2].ReviewComment);
            Assert.AreEqual("ReviewComment4", records[3].ReviewComment);
        }

        /// <summary>
        /// Determines whether this instance [can delete cost share record].
        /// </summary>
        [TestMethod]
        public void CanDeleteRecord()
        {
            var recordToDelete =
                Repository.OfType<T>().Queryable.Where(a => a.ReviewComment.EndsWith("3")).ToList()[0];

            Assert.AreEqual("ReviewComment3", recordToDelete.ReviewComment);

            using (var ts = new TransactionScope())
            {
                Repository.OfType<T>().Remove(recordToDelete);

                ts.CommitTransaction();
            }

            var records = Repository.OfType<T>().GetAll().ToList();
            Assert.AreEqual(4, records.Count);
            Assert.AreEqual("ReviewComment1", records[0].ReviewComment);
            Assert.AreEqual("ReviewComment2", records[1].ReviewComment);
            Assert.AreEqual("ReviewComment4", records[2].ReviewComment);
            Assert.AreEqual("ReviewComment5", records[3].ReviewComment);
        }

        #endregion CRUD Tests

        #region Valid Tests

        #region Month Tests

        /// <summary>
        /// Determines whether this instance [can save with month of 1].
        /// </summary>
        [TestMethod]
        public void CanSaveWithMonthOf1()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var record = GetValid(null);
            record.Month = 1;
            Repository.OfType<T>().EnsurePersistent(record);
            Assert.IsFalse(record.IsTransient(), typeof(T).Name + " Did not save with a month of 1");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }

        /// <summary>
        /// Determines whether this instance [can save with month of 12].
        /// </summary>
        [TestMethod]
        public void CanSaveWithMonthOf12()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var record = GetValid(null);
            record.Month = 12;
            Repository.OfType<T>().EnsurePersistent(record);
            Assert.IsFalse(record.IsTransient(), typeof(T).Name + " Did not save with a month of 12");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }

        #endregion MonthTests

        #region Year Tests

        /// <summary>
        /// Determines whether this instance [can save with year of 1].
        /// </summary>
        [TestMethod]
        public void CanSaveWithYearOf1()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var record = GetValid(null);
            record.Year = 1;
            Repository.OfType<T>().EnsurePersistent(record);
            Assert.IsFalse(record.IsTransient(), typeof(T).Name + " Did not save with a Year of 1");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }

        /// <summary>
        /// Determines whether this instance [can save with year of 9999].
        /// </summary>
        [TestMethod]
        public void CanSaveWithYearOf9999()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var record = GetValid(null);
            record.Year = 9999;
            Repository.OfType<T>().EnsurePersistent(record);
            Assert.IsFalse(record.IsTransient(), typeof(T).Name + " Did not save with a Year of 9999");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }

        #endregion Year Tests

        #region ReviewComments Tests

        /// <summary>
        /// Determines whether this instance [can save with null review comments].
        /// </summary>
        [TestMethod]
        public void CanSaveWithNullReviewComments()
        {
            var record = GetValid(null);
            record.ReviewComment = null;

            Repository.OfType<T>().EnsurePersistent(record);

            Assert.AreEqual(false, record.IsTransient());
        }

        #endregion ReviewComments Tests

        #region Entry Tests

        /// <summary>
        /// Determines whether this instance [can save with entries].
        /// </summary>
        [TestMethod]
        public void CanSaveWithEntries()
        {
            var record = GetValid(null);
            record.AddEntry(new Entry
            {
                Comment = "Valid",
                Record = Repository.OfType<Record>().Queryable.First(),
                FundType = Repository.OfType<FundType>().Queryable.First(),
                Project = Repository.OfType<Project>().Queryable.First(),
                Account = Repository.OfType<Account>().Queryable.First()
            });
            record.AddEntry(new Entry
            {
                Comment = "AnotherValid",
                Record = Repository.OfType<Record>().Queryable.First(),
                FundType = Repository.OfType<FundType>().Queryable.First(),
                Project = Repository.OfType<Project>().Queryable.First(),
                Account = Repository.OfType<Account>().Queryable.First()
            });
            Repository.OfType<T>().EnsurePersistent(record);
            Assert.AreEqual(false, record.IsTransient());
        }
        
        #endregion Entry Tests

        #endregion Valid Tests

        #region Invalid Tests

        #region Month Tests

        /// <summary>
        /// Determines whether this instance [can not save with month of zero].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithMonthOfZero()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.Month = 0;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Month: must be between 1 and 12");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not save with month less than zero].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithMonthLessThanZero()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.Month = -1;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Month: must be between 1 and 12");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not save with month greater than12].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithMonthGreaterThan12()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.Month = 13;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Month: must be between 1 and 12");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());

                throw;
            }
        }
        #endregion Month Tests

        #region Year Tests

        /// <summary>
        /// Determines whether this instance [can not save with year of zero].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithYearOfZero()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.Year = 0;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Year: must be greater than or equal to 1");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());

                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not save with year less than zero].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithYearLessThanZero()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.Year = -1;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Year: must be greater than or equal to 1");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());

                throw;
            }
        }
        #endregion Year Tests

        #region User Tests

        /// <summary>
        /// Determines whether this instance [can not save with null user].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithNullUser()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.User = null;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("User: may not be empty");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());

                throw;
            }
        }

        #endregion User Tests

        #region Status Tests

        /// <summary>
        /// Determines whether this instance [can not save with null status].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithNullStatus()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.Status = null;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Status: may not be empty");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());

                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not commit with new status value].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CanNotCommitWithNewStatusValue()
        {
            T record = null;
            try
            {
                Repository.OfType<T>().DbContext.BeginTransaction();
                record = GetValid(null);
                record.Status = new Status();
                Repository.OfType<T>().EnsurePersistent(record);
                Assert.IsFalse(record.IsTransient());

                Repository.OfType<T>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(record);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Status, Entity: FSNEP.Core.Domain.Status", message.Message);
                throw;
            }
        }
        #endregion Status Tests

        #region Review Comment Tests

        /// <summary>
        /// Determines whether this instance [can not save with review comments too long].
        /// Max length of 512 characters
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithReviewCommentsTooLong()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                var sb = new StringBuilder();
                for (int i = 0; i < 51; i++)
                {
                    sb.Append("1234567890");
                }
                sb.Append("123");
                record.ReviewComment = sb.ToString();
                Assert.AreEqual(513, record.ReviewComment.Length);

                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("ReviewComment: length must be between 0 and 512");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());

                throw;
            }
        }
        #endregion Review Comment Tests

        #region Entries Tests

        /// <summary>
        /// Determines whether this instance [can not save with null entries].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithNullEntries()
        {
            T record = null;
            try
            {
                record = GetValid(null);
                record.Entries = null;
                Repository.OfType<T>().EnsurePersistent(record);
            }
            catch (Exception)
            {
                Assert.IsNotNull(record);
                var results = record.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Entries: may not be empty");
                Assert.IsTrue(record.IsTransient());
                Assert.IsFalse(record.IsValid());

                throw;
            }
        }

        #endregion Entries Tests

        #endregion Invalid Tests

        #region CanBeApprovedOrDenied Tests

        /// <summary>
        /// Determines whether this instance [can be approved or denied is true when status is pending review].
        /// </summary>
        [TestMethod]
        public void CanBeApprovedOrDeniedIsTrueWhenStatusIsPendingReview()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.PendingReview))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.PendingReview, record.Status.NameOption);
            Assert.IsTrue(record.CanBeApprovedOrDenied);
        }

        /// <summary>
        /// Determines whether this instance [can be approved or denied is false when status is current].
        /// </summary>
        [TestMethod]
        public void CanBeApprovedOrDeniedIsFalseWhenStatusIsCurrent()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.Current))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.Current, record.Status.NameOption);
            Assert.IsFalse(record.CanBeApprovedOrDenied);
        }

        /// <summary>
        /// Determines whether this instance [can be approved or denied is false when status is approved].
        /// </summary>
        [TestMethod]
        public void CanBeApprovedOrDeniedIsFalseWhenStatusIsApproved()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.Approved))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.Approved, record.Status.NameOption);
            Assert.IsFalse(record.CanBeApprovedOrDenied);
        }
        
        /// <summary>
        /// Determines whether this instance [can be approved or denied is false when status is disapproved].
        /// </summary>
        [TestMethod]
        public void CanBeApprovedOrDeniedIsFalseWhenStatusIsDisapproved()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.Disapproved))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.Disapproved, record.Status.NameOption);
            Assert.IsFalse(record.CanBeApprovedOrDenied);
        }
        


        #endregion CanBeApprovedOrDenied Tests

        #region IsEditable Tests
        
        /// <summary>
        /// Determines whether [is editable is true when status is current].
        /// </summary>
        [TestMethod]
        public void IsEditableIsTrueWhenStatusIsCurrent()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.Current))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.Current, record.Status.NameOption);
            Assert.IsTrue(record.IsEditable);
        }

        /// <summary>
        /// Determines whether [is editable is true when status is disapproved].
        /// </summary>
        [TestMethod]
        public void IsEditableIsTrueWhenStatusIsDisapproved()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.Disapproved))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.Disapproved, record.Status.NameOption);
            Assert.IsTrue(record.IsEditable);
        }
        
        /// <summary>
        /// Determines whether [is editable is false when status is approved].
        /// </summary>
        [TestMethod]
        public void IsEditableIsFalseWhenStatusIsApproved()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.Approved))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.Approved, record.Status.NameOption);
            Assert.IsFalse(record.IsEditable);
        }

        /// <summary>
        /// Determines whether [is editable is false when status is pending review].
        /// </summary>
        [TestMethod]
        public void IsEditableIsFalseWhenStatusIsPendingReview()
        {
            var record = GetValid(null);
            record.Status =
                Repository.OfType<Status>()
                .Queryable
                .Where(a => a.Name == Status.GetName(Status.Option.PendingReview))
                .FirstOrDefault();
            Assert.AreEqual(Status.Option.PendingReview, record.Status.NameOption);
            Assert.IsFalse(record.IsEditable);
        }

        #endregion IsEditable Tests
         
    }
}
