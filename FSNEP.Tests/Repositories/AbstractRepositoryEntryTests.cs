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
    public abstract class AbstractRepositoryEntryTests<T> : RepositoryTestBase where T : Entry
    {
        #region Init

        /// <summary>
        /// Gets the valid entity with Entry as the base type.
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
                var entry = GetValid(i + 1);
                Repository.OfType<T>().EnsurePersistent(entry);
            }
        }

        #endregion Init

        #region CRUD Tests

        /// <summary>
        /// Determines whether this instance [can save valid Entry].
        /// </summary>
        [TestMethod]
        public void CanSaveValidEntry()
        {
            var entry = GetValid(null);
            Repository.OfType<T>().EnsurePersistent(entry);

            Assert.AreEqual(false, entry.IsTransient());
        }

        /// <summary>
        /// Determines whether this instance [can read Entry records].
        /// </summary>
        [TestMethod]
        public void CanReadEntryRecords()
        {
            var entryRecords = Repository.OfType<T>().GetAll().ToList();
            Assert.IsNotNull(entryRecords);
            Assert.AreEqual(5, entryRecords.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual("Comment" + (i + 1), entryRecords[i].Comment);
            }
        }

        /// <summary>
        /// Determines whether this instance [can query Entry records].
        /// </summary>
        [TestMethod]
        public void CanQueryEntryRecords()
        {
            var entryRecords =
                Repository.OfType<T>().Queryable.Where(a => a.Comment.EndsWith("3")).ToList();
            Assert.IsNotNull(entryRecords);
            Assert.AreEqual(1, entryRecords.Count);
            Assert.AreEqual("Comment3", entryRecords[0].Comment);
        }

        /// <summary>
        /// Determines whether this instance [can update Entry].
        /// </summary>
        [TestMethod]
        public void CanUpdateEntry()
        {
            var entryToUpdate =
                Repository.OfType<T>().Queryable.Where(a => a.Comment.EndsWith("3")).ToList()[0];

            Assert.AreEqual("Comment3", entryToUpdate.Comment);

            entryToUpdate.Comment = "Updated";
            Repository.OfType<T>().EnsurePersistent(entryToUpdate);

            var entryRecords = Repository.OfType<T>().GetAll().ToList();
            Assert.AreEqual(5, entryRecords.Count);
            Assert.AreEqual("Updated", entryRecords[2].Comment);
            Assert.AreEqual("Comment4", entryRecords[3].Comment);
        }

        /// <summary>
        /// Determines whether this instance [can delete cost share Entry].
        /// </summary>
        [TestMethod]
        public void CanDeleteEntry()
        {
            var entryToDelete =
                Repository.OfType<T>().Queryable.Where(a => a.Comment.EndsWith("3")).FirstOrDefault();

            Assert.AreEqual("Comment3", entryToDelete.Comment);

            using (var ts = new TransactionScope())
            {
                Repository.OfType<T>().Remove(entryToDelete);

                ts.CommitTransaction();
            }

            var entryRecords = Repository.OfType<T>().GetAll().ToList();
            Assert.AreEqual(4, entryRecords.Count);
            Assert.AreEqual("Comment1", entryRecords[0].Comment);
            Assert.AreEqual("Comment2", entryRecords[1].Comment);
            Assert.AreEqual("Comment4", entryRecords[2].Comment);
            Assert.AreEqual("Comment5", entryRecords[3].Comment);
        }

        #endregion CRUD Tests

        #region Valid Tests
        
        #region Comment Tests
        /// <summary>
        /// Determines whether this instance [can save with long Comment (256 Characters)].
        /// </summary>
        [TestMethod]
        public void CanSaveWithLongComment()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var entry = GetValid(null);
            var sb = new StringBuilder();
            for (int i = 0; i < 25; i++)
            {
                sb.Append("1234567890");
            }
            sb.Append("123456");
            entry.Comment = sb.ToString();
            Assert.AreEqual(256, entry.Comment.Length);
            Repository.OfType<T>().EnsurePersistent(entry);
            Assert.IsFalse(entry.IsTransient(), typeof(T).Name + " Did not save with comment 256 characters long");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }

        /// <summary>
        /// Determines whether this instance [can save with a null Comment].
        /// </summary>
        [TestMethod]
        public void CanSaveWithNullComment()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var entry = GetValid(null);
            entry.Comment = null;
            Repository.OfType<T>().EnsurePersistent(entry);
            Assert.IsFalse(entry.IsTransient(), typeof(T).Name + " Did not save with a null comment");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }

        /// <summary>
        /// Determines whether this instance [can save with an empty Comment].
        /// </summary>
        [TestMethod]
        public void CanSaveWithEmptyComment()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var entry = GetValid(null);
            entry.Comment = string.Empty;
            Repository.OfType<T>().EnsurePersistent(entry);
            Assert.IsFalse(entry.IsTransient(), typeof(T).Name + " Did not save with an empty comment");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }

        /// <summary>
        /// Determines whether this instance [can save with a spaces only Comment].
        /// </summary>
        [TestMethod]
        public void CanSaveWithSpcesOnlyComment()
        {
            Repository.OfType<T>().DbContext.BeginTransaction();
            var entry = GetValid(null);
            entry.Comment = string.Empty;
            Repository.OfType<T>().EnsurePersistent(entry);
            Assert.IsFalse(entry.IsTransient(), typeof(T).Name + " Did not save with a spaces only comment");
            Repository.OfType<T>().DbContext.CommitTransaction();
        }
        #endregion Comment Tests

        #endregion Valid Tests

        #region Invalid Tests

        #region Record Tests
        /// <summary>
        /// Determines whether this instance [can not save with null Record].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithNullRecord()
        {
            T entry = null;
            try
            {
                entry = GetValid(null);
                entry.Record = null;
                Repository.OfType<T>().EnsurePersistent(entry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(entry);
                var results = entry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Record: may not be null");
                Assert.IsTrue(entry.IsTransient());
                Assert.IsFalse(entry.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not commit with new record].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CanNotCommitWithNewRecord()
        {
            T entry = null;
            try
            {
                Repository.OfType<T>().DbContext.BeginTransaction();
                entry = GetValid(null);
                entry.Record = new Record();
                Repository.OfType<T>().EnsurePersistent(entry);
                Assert.IsFalse(entry.IsTransient());

                Repository.OfType<T>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {

                Assert.IsNotNull(entry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Record, Entity: FSNEP.Core.Domain.Record", message.Message);
                throw;
            }
        }
        #endregion Record Tests

        #region Project Tests
        /// <summary>
        /// Determines whether this instance [can not save with null project].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithNullProject()
        {
            T entry = null;
            try
            {
                entry = GetValid(null);
                entry.Project = null;
                Repository.OfType<T>().EnsurePersistent(entry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(entry);
                var results = entry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Project: may not be null");
                Assert.IsTrue(entry.IsTransient());
                Assert.IsFalse(entry.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not commit with new project].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CanNotCommitWithNewProject()
        {
            T entry = null;
            try
            {
                Repository.OfType<T>().DbContext.BeginTransaction();
                entry = GetValid(null);
                entry.Project = new Project();
                Repository.OfType<T>().EnsurePersistent(entry);
                Assert.IsFalse(entry.IsTransient());

                Repository.OfType<T>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {
                Assert.IsNotNull(entry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Project, Entity: ", message.Message);
                throw;
            }
        }
        #endregion Project Tests

        #region FundType Tests
        /// <summary>
        /// Determines whether this instance [can not save with null Fund Type].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithNullFundType()
        {
            T entry = null;
            try
            {
                entry = GetValid(null);
                entry.FundType = null;
                Repository.OfType<T>().EnsurePersistent(entry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(entry);
                var results = entry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("FundType: may not be null");
                Assert.IsTrue(entry.IsTransient());
                Assert.IsFalse(entry.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not commit with new Fund Type].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CanNotCommitWithNewFundType()
        {
            T entry = null;
            try
            {
                Repository.OfType<T>().DbContext.BeginTransaction();
                entry = GetValid(null);
                entry.FundType = new FundType();
                Repository.OfType<T>().EnsurePersistent(entry);
                Assert.IsFalse(entry.IsTransient());

                Repository.OfType<T>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {
                Assert.IsNotNull(entry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.FundType, Entity: FSNEP.Core.Domain.FundType", message.Message);
                throw;
            }
        }
        #endregion FundType Tests

        #region Account Tests
        /// <summary>
        /// Determines whether this instance [can not save with null Account].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithNullAccount()
        {
            T entry = null;
            try
            {
                entry = GetValid(null);
                entry.Account = null;
                Repository.OfType<T>().EnsurePersistent(entry);
            }
            catch (Exception)
            {
                Assert.IsNotNull(entry);
                var results = entry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Account: may not be null");
                Assert.IsTrue(entry.IsTransient());
                Assert.IsFalse(entry.IsValid());
                throw;
            }
        }

        /// <summary>
        /// Determines whether this instance [can not commit with new Account].
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NHibernate.TransientObjectException))]
        public void CanNotCommitWithNewAccount()
        {
            T entry = null;
            try
            {
                Repository.OfType<T>().DbContext.BeginTransaction();
                entry = GetValid(null);
                entry.Account = new Account();
                Repository.OfType<T>().EnsurePersistent(entry);
                Assert.IsFalse(entry.IsTransient());

                Repository.OfType<T>().DbContext.CommitTransaction();
            }
            catch (Exception message)
            {
                Assert.IsNotNull(entry);
                Assert.AreEqual("object references an unsaved transient instance - save the transient instance before flushing. Type: FSNEP.Core.Domain.Account, Entity:  (0%)", message.Message);
                throw;
            }
        }
        #endregion Account Tests

        #region Comment Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CanNotSaveWithTooLongComment()
        {
            T entry = null;

            try
            {
                entry = GetValid(null);
                var sb = new StringBuilder();
                for (int i = 0; i < 25; i++)
                {
                    sb.Append("1234567890");
                }
                sb.Append("1234567");
                entry.Comment = sb.ToString();
                Assert.AreEqual(257, entry.Comment.Length);
                Repository.OfType<T>().EnsurePersistent(entry);
            }
            catch (Exception)
            {

                Assert.IsNotNull(entry);

                var results = entry.ValidationResults().AsMessageList();
                results.AssertErrorsAre("Comment: length must be between 0 and 256");
                Assert.IsTrue(entry.IsTransient());
                Assert.IsFalse(entry.IsValid());

                throw;
            }
        }

        #endregion Comment Tests

        #endregion Invalid Tests
    }
}
