using System;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class ActivityCategoryRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<ActivityCategory> _activityCategoryRepository = new Repository<ActivityCategory>();
        /// <summary>
        /// 50 characters for Name
        /// </summary>
        public const string ValidValueName = "123456789 123456789 123456789 123456789 1234567890";
        /// <summary>
        /// 51 characters for Name
        /// </summary>
        private const string InvalidValueName = "123456789 123456789 123456789 123456789 123456789 1";

        //Don't load any data
        protected override void LoadData() { }

        [TestMethod]
        public void CanSaveCompleteAndValidActivityCategory()
        {
            var activityCategory = new ActivityCategory { Name = ValidValueName };

            using (var ts = new TransactionScope())
            {
                _activityCategoryRepository.EnsurePersistent(activityCategory);

                ts.CommitTransaction();
            }

            Assert.AreEqual(false, activityCategory.IsTransient());
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityCategoryDoesNotSaveWithNullName()
        {
            var activityCategory = new ActivityCategory { Name = null };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityCategoryRepository.EnsurePersistent(activityCategory);

                    ts.CommitTransaction();
                }
            }
            catch (Exception)
            {
                var results = activityCategory.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Name: may not be null or empty");
                //results.AssertContains("Name: length must be between 0 and 50");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityCategory could not be persisted\n\n\r\nValidation Errors: Name, may not be null or empty\r\nName, length must be between 0 and 50\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityCategoryDoesNotSaveWithNameGreaterThan50Characters()
        {
            var activityCategory = new ActivityCategory { Name = InvalidValueName };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityCategoryRepository.EnsurePersistent(activityCategory);

                    ts.CommitTransaction();
                }
            }
            catch (Exception)
            {
                var results = activityCategory.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Name: length must be between 0 and 50");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityCategory could not be persisted\n\n\r\nValidation Errors: Name, length must be between 0 and 50\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }
    }
}
