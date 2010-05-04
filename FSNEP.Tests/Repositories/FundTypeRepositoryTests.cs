using System;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;

namespace FSNEP.Tests.Repositories
{
    /// <summary>
    /// Is FundType used anywhere/Static Data?
    /// </summary>
    [TestClass]
    public class FundTypeRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<FundType> _fundTypeRepository = new Repository<FundType>();
        /// <summary>
        /// 50 characters for Name
        /// </summary>
        public const string ValidValueName = "123456789 123456789 123456789 123456789 1234567890";
        /// <summary>
        /// 51 characters for Name
        /// </summary>
        public const string InvalidValueName = "123456789 123456789 123456789 123456789 123456789 1";

        //Don't load any data
        protected override void LoadData() { }

        [TestMethod]
        public void CanSaveCompleteAndValidFundType()
        {
            var fundType = new FundType { Name = ValidValueName };

            _fundTypeRepository.EnsurePersistent(fundType);

            Assert.AreEqual(false, fundType.IsTransient());
        }
        
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void FundTypeDoesNotSaveWithNullName()
        {
            var fundType = new FundType { Name = null};
            

            try
            {
                _fundTypeRepository.EnsurePersistent(fundType);
            }
            catch (Exception)
            {
                var results = fundType.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                //results.AssertContains("Name: length must be between 0 and 50");
                results.AssertContains("Name: may not be null or empty");
                //Assert.AreEqual(true, results.Contains("Name: length must be between 0 and 50"), "Expected the validation result to have \"Name: length must be between 0 and 50\"");
                //Assert.AreEqual(true, results.Contains("Name: may not be null or empty"), "Expected the validation result to have \"Name: may not be null or empty\"");                
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void FundTypeDoesNotSaveWithNameGreaterThan50Characters()
        {
            var fundType = new FundType { Name = InvalidValueName };

            try
            {
                _fundTypeRepository.EnsurePersistent(fundType);
            }
            catch (Exception)
            {
                var results = fundType.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Name: length must be between 0 and 50");
                //Assert.AreEqual(true, results.Contains("Name: length must be between 0 and 50"), "Expected the validation result to have \"Name: length must be between 0 and 50\"");                
                throw;
            }
        }   
    }
}
