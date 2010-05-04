using System.Collections.Generic;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;

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
            //Create 2 active accounts, 3 inactive
            for (int i = 1; i < 6; i++)
            {
                var account = new Account
                                  {
                                      Name = "Account" + i,                                       
                                  };
                account.IsActive = i%2==0;

                accountRepository.EnsurePersistent(account);
            }
        }

        /// <summary>
        /// A Valid Project can be persisted to the database.
        /// </summary>
        [TestMethod]
        public void CanSaveCompleteAndValidProject()
        {

            var allAccounts = AccountRepository.GetAll();
            var activeAccounts = new List<Account>();
            foreach (Account account in allAccounts)
            {
                if (account.IsActive)
                {
                    activeAccounts.Add(account);
                }
            }

            
            //TODO: Verify Account associations?
            var project = new Project
                              {
                                  IsActive = true, 
                                  Name = ValidValueName,
                                  Accounts = activeAccounts
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
