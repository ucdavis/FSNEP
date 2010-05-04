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
    public class AccountRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<Account> _accountRepository = new Repository<Account>();
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
        public void CanSaveCompleteAndValidAccount()
        {
            var accountCategory = new Account
              {
                  Name = ValidValueName,
                  IndirectCost = 0
              };


            using (var ts = new TransactionScope())
            {
                _accountRepository.EnsurePersistent(accountCategory);

                ts.CommitTransaction();
            }

            Assert.AreEqual(false, accountCategory.IsTransient());
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AccountDoesNotSaveWithNullName()
        {
            var accountCategory = new Account
              {
                  Name = null,
                  IndirectCost = 0.30
              };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _accountRepository.EnsurePersistent(accountCategory);

                    ts.CommitTransaction();
                }
            }
            catch (Exception)
            {
                var results = accountCategory.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Name: may not be null or empty");
                //results.AssertContains("Name: length must be between 0 and 50");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: Name, may not be null or empty\r\nName, length must be between 0 and 50\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AccountDoesNotSaveWithNameGreaterThan50Characters()
        {
            var accountCategory = new Account
            {
                Name = InvalidValueName,
                IndirectCost = 0.15
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _accountRepository.EnsurePersistent(accountCategory);

                    ts.CommitTransaction();
                }
            }
            catch (Exception)
            {
                var results = accountCategory.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Name: length must be between 0 and 50");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: Name, length must be between 0 and 50\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AccountDoesNotSaveWithIndirectCostLessThanZero()
        {
            var accountCategory = new Account
            {
                Name = ValidValueName,
                IndirectCost = -0.15
            };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _accountRepository.EnsurePersistent(accountCategory);

                    ts.CommitTransaction();
                }
            }
            catch (Exception)
            {
                var results = accountCategory.ValidationResults().AsMessageList();
                Assert.AreEqual(2, results.Count);
                results.AssertContains("IndirectCost: must be between 0 and 0.3");
                results.AssertContains("IndirectCostPercent: must be between 0 and 30");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: IndirectCost, The value must fall within the range \"0\" (Inclusive) - \"0.3\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AccountDoesNotSaveWithIndirectCostGreaterThanPoint3()
        {
            var accountCategory = new Account
              {
                  Name = ValidValueName,
                  IndirectCost = 0.300001
              };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _accountRepository.EnsurePersistent(accountCategory);

                    ts.CommitTransaction();
                }
            }
            catch (Exception)
            {
                var results = accountCategory.ValidationResults().AsMessageList();
                Assert.AreEqual(2, results.Count);
                results.AssertContains("IndirectCost: must be between 0 and 0.3");
                results.AssertContains("IndirectCostPercent: must be between 0 and 30");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: IndirectCost, The value must fall within the range \"0\" (Inclusive) - \"0.3\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        public void IndirectCostPercent()
        {
            var accountCategory = new Account
            {
                Name = "Test",
                IndirectCost = 0.29
            };
            
            //TODO:Review. Is the indirect cost percent a problem? Without the round, it returns 28.999999... Should it be a decimal?
            Assert.AreEqual(29, Math.Round(accountCategory.IndirectCostPercent,2));
            //Tostring rounds it correctly.
            Assert.AreEqual("Test (29%)", accountCategory.ToString());

            accountCategory.IndirectCost = .299;
            Assert.AreEqual("Test (29.9%)", accountCategory.ToString());

            accountCategory.IndirectCostPercent = 10;
            Assert.AreEqual(0.10, accountCategory.IndirectCost);

            accountCategory.IndirectCostPercent = 29.9;
            Assert.AreEqual(0.299, accountCategory.IndirectCost);
        }
    }
}
