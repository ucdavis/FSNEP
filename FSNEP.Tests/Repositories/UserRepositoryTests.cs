using CAESArch.BLL;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;
using FSNEP.BLL.Impl;
using Rhino.Mocks;
using FSNEP.BLL.Interfaces;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class UserRepositoryTests : RepositoryTestBase
    {
        public IUserBLL UserBLL { get; set; }

        public UserRepositoryTests()
        {
            var userAuth = MockRepository.GenerateStub<IUserAuth>();

            UserBLL = new UserBLL(userAuth);
        }

        [TestMethod]
        public void Test()
        {
            var activityType = new ActivityType {Name = "Test", Indicator = "IN"};
            
            using (var ts = new TransactionScope())
            {
                new GenericBLL<ActivityType,int>().Repository.EnsurePersistent(activityType);
                
                ts.CommitTransaction();
            }
        }
    }
}