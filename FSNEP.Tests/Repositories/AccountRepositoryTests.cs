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
    public class AccountRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<Account> _accountRepository = new Repository<Account>();

        //Don't load any data
        protected override void LoadData() { }

        [TestMethod]
        public void CanSaveCompleteAndValidAccount()
        {
            var accountCategory = new Account
              {
                  Name = "123456789 123456789 123456789 123456789 1234567890",
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
            var accountCategory = new Account()
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
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: Name, The value cannot be null.\r\nName, The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AccountDoesNotSaveWithNameGreaterThan50Characters()
        {
            var accountCategory = new Account()
            {
                Name = "123456789 123456789 123456789 123456789 123456789 1",
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
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: Name, The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AccountDoesNotSaveWithIndirectCostLessThanZero()
        {
            var accountCategory = new Account()
            {
                Name = "123456789 123456789 123456789 123456789 1234567890",
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
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: IndirectCost, The value must fall within the range \"0\" (Inclusive) - \"0.3\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void AccountDoesNotSaveWithIndirectCostGreaterThanPoint3()
        {
            var accountCategory = new Account
              {
                  Name = "123456789 123456789 123456789 123456789 1234567890",
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
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.Account could not be persisted\n\n\r\nValidation Errors: IndirectCost, The value must fall within the range \"0\" (Inclusive) - \"0.3\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }
    }
}
