using System;
using CAESArch.BLL;
using CAESArch.BLL.Repositories;
using CAESArch.Core.DataInterfaces;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class ActivityTypeRepositiryTests : RepositoryTestBase
    {
        private readonly IRepository<ActivityType> _activityTypeRepository = new Repository<ActivityType>();        
        private ActivityCategory ActivityCategory { get; set; }


        //Only Load ActivityCategory Data
        protected override void LoadData()
        {
            var activityCategoryRepository = new Repository<ActivityCategory>();

            ActivityCategory = new ActivityCategory { Name = "123456789 123456789 123456789 123456789 1234567890" };
            using (var ts = new TransactionScope())
            {
                activityCategoryRepository.EnsurePersistent(ActivityCategory);

                ts.CommitTransaction();
            }
            Assert.AreEqual(false, ActivityCategory.IsTransient());  
        }
     

        [TestMethod]
        public void CanSaveCompleteAndValidActivityType()
        {          
            var activityType = new ActivityType
               {
                   ActivityCategory = ActivityCategory,
                   Indicator = "12",
                   Name = "123456789 123456789 123456789 123456789 1234567890"
               };

            using (var ts = new TransactionScope())
            {
                _activityTypeRepository.EnsurePersistent(activityType);

                ts.CommitTransaction();
            }

            Assert.AreEqual(false, activityType.IsTransient());
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityTypeDoesNotSaveWithNullName()
        {
            var activityType = new ActivityType
            {
                ActivityCategory = ActivityCategory,
                Indicator = "12",
                Name = null
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityTypeRepository.EnsurePersistent(activityType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityType could not be persisted\n\n\r\nValidation Errors: Name, The value cannot be null.\r\nName, The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }               

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityTypeDoesNotSaveWithNameGreaterThan50Characters()
        {
            var activityType = new ActivityType
            {
                ActivityCategory = ActivityCategory,
                Indicator = "12",
                Name = "123456789 123456789 123456789 123456789 123456789 1"
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityTypeRepository.EnsurePersistent(activityType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityType could not be persisted\n\n\r\nValidation Errors: Name, The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityTypeDoesNotSaveWithNullIndicator()
        {
            var activityType = new ActivityType
            {
                ActivityCategory = ActivityCategory,
                Indicator = null,
                Name = "123456789 123456789 123456789 123456789 123456789"
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityTypeRepository.EnsurePersistent(activityType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityType could not be persisted\n\n\r\nValidation Errors: Indicator, The value cannot be null.\r\nIndicator, The length of the value must fall within the range \"2\" (Inclusive) - \"2\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityTypeDoesNotSaveWithIndicatorWith1Character()
        {
            var activityType = new ActivityType
            {
                ActivityCategory = ActivityCategory,
                Indicator = "1",
                Name = "123456789 123456789 123456789 123456789 123456789"
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityTypeRepository.EnsurePersistent(activityType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityType could not be persisted\n\n\r\nValidation Errors: Indicator, The length of the value must fall within the range \"2\" (Inclusive) - \"2\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityTypeDoesNotSaveWithIndicatorWithGreaterThan2Characters()
        {
            var activityType = new ActivityType
            {
                ActivityCategory = ActivityCategory,
                Indicator = "123",
                Name = "123456789 123456789 123456789 123456789 123456789"
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityTypeRepository.EnsurePersistent(activityType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityType could not be persisted\n\n\r\nValidation Errors: Indicator, The length of the value must fall within the range \"2\" (Inclusive) - \"2\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ActivityTypeDoesNotSaveWithNullActivityCategory()
        {
            //TODO: Review. The indicator here allows 2 spaces, but the UI doesn't appear to allow it. Do we want to add the Required Validator?
            var activityType = new ActivityType
            {
                ActivityCategory = null,
                Indicator = "  ",
                Name = "123456789 123456789 123456789 123456789 123456789"
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _activityTypeRepository.EnsurePersistent(activityType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.ActivityType could not be persisted\n\n\r\nValidation Errors: ActivityCategory, The value cannot be null.\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }
    }
}
