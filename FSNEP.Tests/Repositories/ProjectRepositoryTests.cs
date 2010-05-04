using System.Collections.Generic;
using CAESArch.BLL;
using CAESArch.BLL.Repositories;
using CAESArch.Core.DataInterfaces;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class ProjectRepositoryTests : RepositoryTestBase
    {
        private readonly IRepository<Project> _projectRepository = new Repository<Project>();
        private Repository<Account> AccountRepository { get; set; }

        /// <summary>
        /// 50 characters for Name
        /// </summary>
        public const string ValidValueName = "123456789 123456789 123456789 123456789 1234567890";
 

        /// <summary>
        /// Only Load Account data
        /// </summary>
        protected override void LoadData()
        {
           AccountRepository = new Repository<Account>();

            using (var ts = new TransactionScope())
            {
                CreateAccounts(AccountRepository);
                ts.CommitTransaction();
            }
        }


        /// <summary>
        /// Creates the accounts.
        /// </summary>
        /// <param name="accountRepository">The account repository.</param>
        private static void CreateAccounts(IRepository<Account> accountRepository)
        {
            //Create 3 accounts
            for (int i = 1; i < 4; i++)
            {
                var account = new Account
                                  {
                                      Name = "Account" + i, 
                                      IsActive = true
                                  };

                accountRepository.EnsurePersistent(account);
            }
        }

        /// <summary>
        /// A Valid Project can be persisted to the database.
        /// </summary>
        [TestMethod]
        public void CanSaveCompleteAndValidProject()
        {

            List<Account> allAccounts = AccountRepository.GetAll();
            
            //TODO: Verify Account associations?
            var project = new Project
                              {
                                  IsActive = true, 
                                  Name = ValidValueName,
                                  Accounts = allAccounts
                              };
            using (var ts = new TransactionScope())
            {
                _projectRepository.EnsurePersistent(project);
                ts.CommitTransaction();
            }

            Assert.AreEqual(false, project.IsTransient());
        }
    }
}
