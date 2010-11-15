using System;
using System.Linq;
using System.Text;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class RecordTrackingRepositoryTests : RepositoryTestBase
    {
        private const int ValidYear = 2009;
        private const int ValidMonth = 6;
        private const string ValidUserName = "ValidUser";

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

        #region Valid Record Tracking tests
        /// <summary>
        /// Determines whether this instance [can save valid record tracking entry].
        /// </summary>
        [TestMethod]
        public void CanSaveValidRecordTrackingEntry()
        {
            var recordTracking = CreateValidRecordTracking();

            Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);

            Assert.AreEqual(false, recordTracking.IsTransient());
        }
        #endregion Valid Record Tracking tests


        #region UserName Tests
        /// <summary>
        /// Record tracking does not save with a null user name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void RecordTrackingDoesNotSaveWithNullUserName()
        {
            RecordTracking recordTracking = null;
            try
            {
                recordTracking = CreateValidRecordTracking();
                recordTracking.UserName = null;
                Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);
            }
            catch (Exception)
            {
                Assert.IsNotNull(recordTracking);
                if (recordTracking != null)
                {
                    var results = recordTracking.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("UserName: may not be null or empty");
                    Assert.IsTrue(recordTracking.IsTransient());
                    Assert.IsFalse(recordTracking.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Record tracking does not save with an empty user name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void RecordTrackingDoesNotSaveWithEmptyUserName()
        {
            RecordTracking recordTracking = null;
            try
            {
                recordTracking = CreateValidRecordTracking();
                recordTracking.UserName = string.Empty;
                Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);
            }
            catch (Exception)
            {
                Assert.IsNotNull(recordTracking);
                if (recordTracking != null)
                {
                    var results = recordTracking.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("UserName: may not be null or empty");
                    Assert.IsTrue(recordTracking.IsTransient());
                    Assert.IsFalse(recordTracking.IsValid());
                }

                throw;
            }
        }
        /// <summary>
        /// Record tracking does not save with spaces only in user name.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void RecordTrackingDoesNotSaveWithSpacesOnlyUserName()
        {
            RecordTracking recordTracking = null;
            try
            {
                recordTracking = CreateValidRecordTracking();
                recordTracking.UserName = " ";
                Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);
            }
            catch (Exception)
            {
                Assert.IsNotNull(recordTracking);
                if (recordTracking != null)
                {
                    var results = recordTracking.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("UserName: may not be null or empty");
                    Assert.IsTrue(recordTracking.IsTransient());
                    Assert.IsFalse(recordTracking.IsValid());
                }

                throw;
            }
        }

        /// <summary>
        /// Record tracking does not save with a user name that is too long.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void RecordTrackingDoesNotSaveWithTooLongUserName()
        {
            RecordTracking recordTracking = null;
            try
            {
                recordTracking = CreateValidRecordTracking();
                var sb = new StringBuilder();
                for (int i = 0; i < 10; i++)
                {
                    sb.Append("123456789 123456789 12345");
                }
                sb.Append("1234567");
                recordTracking.UserName = sb.ToString();

                Assert.AreEqual(257, recordTracking.UserName.Length);

                
                Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);
            }
            catch (Exception)
            {
                Assert.IsNotNull(recordTracking);
                if (recordTracking != null)
                {
                    var results = recordTracking.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("UserName: length must be between 0 and 256");
                    Assert.IsTrue(recordTracking.IsTransient());
                    Assert.IsFalse(recordTracking.IsValid());
                }

                throw;
            }
        }
        #endregion UserName Tests

        #region ActionDate Tests
        /// <summary>
        /// Record tracking saves with action date not set (defaults to a date).
        /// </summary>
        [TestMethod]
        public void RecordTrackingSavesWithActionDateNotSet()
        {
            var recordTracking = new RecordTracking
                                                {
                                                    UserName = ValidUserName,
                                                    DigitalSignature = new byte[1],
                                                    Record = Repository.OfType<TimeRecord>().Queryable.First(),
                                                    Status = Repository.OfType<Status>().Queryable.First()
                                                };

            Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);
            Assert.AreEqual(false, recordTracking.IsTransient());
        }        
        #endregion ActionDate Tests

        #region Record Tests
        /// <summary>
        /// Record tracking does not save with null record.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void RecordTrackingDoesNotSaveWithNullRecord()
        {
            RecordTracking recordTracking = null;
            try
            {
                recordTracking = CreateValidRecordTracking();
                recordTracking.Record = null;

                Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);
            }
            catch (Exception)
            {
                Assert.IsNotNull(recordTracking);
                if (recordTracking != null)
                {
                    var results = recordTracking.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Record: may not be null");
                    Assert.IsTrue(recordTracking.IsTransient());
                    Assert.IsFalse(recordTracking.IsValid());
                }

                throw;
            }
        }

        #endregion Record Tests

        #region Status Tests
        /// <summary>
        /// Record tracking does not save with null status.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void RecordTrackingDoesNotSaveWithNullStatus()
        {
            RecordTracking recordTracking = null;
            try
            {
                recordTracking = CreateValidRecordTracking();
                recordTracking.Status = null;

                Repository.OfType<RecordTracking>().EnsurePersistent(recordTracking);
            }
            catch (Exception)
            {
                Assert.IsNotNull(recordTracking);
                if (recordTracking != null)
                {
                    var results = recordTracking.ValidationResults().AsMessageList();
                    results.AssertErrorsAre("Status: may not be null");
                    Assert.IsTrue(recordTracking.IsTransient());
                    Assert.IsFalse(recordTracking.IsValid());
                }

                throw;
            }
        }
        #endregion Status Tests

        #region Helper Methods
        private RecordTracking CreateValidRecordTracking()
        {
            return new RecordTracking
            {
                ActionDate = DateTime.Now,
                UserName = ValidUserName,
                DigitalSignature = new byte[2],
                Record = Repository.OfType<TimeRecord>().Queryable.First(),
                Status = Repository.OfType<Status>().Queryable.First()
            };
        }

        #endregion Helper Methods
    }
}