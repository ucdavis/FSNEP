using System;
using CAESArch.BLL;
using CAESArch.BLL.Repositories;
using CAESArch.Core.DataInterfaces;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Repositories
{
    /// <summary>
    /// Is FundType used anywhere/Static Data?
    /// </summary>
    [TestClass]
    public class FundTypeRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<FundType> _fundTypeRepository = new Repository<FundType>();

        //Don't load any data
        protected override void LoadData() { }

        [TestMethod]
        public void CanSaveCompleteAndValidFundType()
        {
            var fundType = new FundType { Name = "123456789 123456789 123456789 123456789 1234567890" };

            using (var ts = new TransactionScope())
            {
                _fundTypeRepository.EnsurePersistent(fundType);

                ts.CommitTransaction();
            }

            Assert.AreEqual(false, fundType.IsTransient());
        }
        
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void FundTypeDoesNotSaveWithNullName()
        {
            var fundType = new FundType { Name = null};

            try
            {
                using (var ts = new TransactionScope())
                {
                    _fundTypeRepository.EnsurePersistent(fundType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.FundType could not be persisted\n\n\r\nValidation Errors: Name, The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).\r\nName, The value cannot be null.\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void FundTypeDoesNotSaveWithNameGreaterThan50Characters()
        {
            var fundType = new FundType { Name = "123456789 123456789 123456789 123456789 123456789 1" };

            try
            {
                using (var ts = new TransactionScope())
                {
                    _fundTypeRepository.EnsurePersistent(fundType);

                    ts.CommitTransaction();
                }
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.FundType could not be persisted\n\n\r\nValidation Errors: Name, The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }   
    }
}
