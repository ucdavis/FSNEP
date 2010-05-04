using System;
using System.Collections.Generic;
using FSNEP.BLL.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using FSNEP.Core.Domain;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Data.NHibernate;
using UCDArch.Testing;

namespace FSNEP.Tests.Core
{
    public abstract class RepositoryTestBase
    {
        public List<Guid> UserIds { get; set; }
        public IRepository Repository { get; set; }

        protected RepositoryTestBase()
        {
            UserIds = new List<Guid>();
            Repository = new Repository();
        }

        [TestInitialize]
        public void CreateDB()
        {
            Configuration config = new Configuration().Configure();
            new NHibernate.Tool.hbm2ddl.SchemaExport(config).Execute(false, true, false,
                                                                    NHibernateSessionManager.
                                                                        Instance.GetSession().Connection, null);

            
            ServiceLocatorInitializer.Init();
            LoadData();
        }

        protected virtual void LoadData()
        {
            using (var ts = new TransactionScope())
            {
                //load base data
                CreateProjects();
                CreateAccounts();
                CreateFundTypes();
                CreateUsers();
                CreateStatus();
                //CreateActivityCategory(); //TODO: Implement These?
                //CreateActivityTypes();

                ts.CommitTransaction();
            }
        }

        

        

        private void CreateFundTypes()
        {
            //Create 3 
            for (int i = 0; i < 6; i++)
            {
                var fundType = new FundType() { Name = "FundType" + i };

                Repository.OfType<FundType>().EnsurePersistent(fundType);
            }
        }

        private void CreateAccounts()
        {
            //Create 3
            for (int i = 0; i < 6; i++)
            {
                var account = new Account() { Name = "Account" + i, IsActive = true };

                Repository.OfType<Account>().EnsurePersistent(account);
            }
        }

        private void CreateActivityCategory()
        {
            throw new NotImplementedException();
        }

        private void CreateActivityTypes()
        {
            throw new NotImplementedException();
        }

        private void CreateStatus()
        {
            var status1 = new Status { NameOption = Status.Option.Current };
            var status2 = new Status { NameOption = Status.Option.PendingReview };
            var status3 = new Status {NameOption = Status.Option.Approved};
            var status4 = new Status { NameOption = Status.Option.Disapproved };

            var statusRepository = Repository.OfType<Status>();

            statusRepository.EnsurePersistent(status1);
            statusRepository.EnsurePersistent(status2);
            statusRepository.EnsurePersistent(status3);
            statusRepository.EnsurePersistent(status4);
        }

        private void CreateProjects()
        {
            //Create 3 projects
            for (int i = 0; i < 6; i++)
            {
                var proj = new Project {Name = "Project" + i, IsActive = true};

                Repository.OfType<Project>().EnsurePersistent(proj);
            }
        }

        private void CreateUsers()
        {
            var userId = Guid.NewGuid();
            
            var userBLL = new UserBLL(null);

            var supervisor = new User
                                 {
                                     FirstName = "FNameS",
                                     LastName = "LNameS",
                                     Salary = 10,
                                     FTE = 1,
                                     IsActive = true,
                                     UserName = "Supervisor",
                                     Email = "supervisor@testucdavis.edu"
                                 };

            supervisor.Supervisor = supervisor; //i'm my own boss
            supervisor.SetUserID(userId);

            #region test
            
            supervisor.Projects.Add(Repository.OfType<Project>().GetByID(1));

            var fundTypes = Repository.OfType<FundType>().GetAll();

            supervisor.FundTypes = fundTypes;

            #endregion

            UserIds.Add(userId);

            userBLL.EnsurePersistent(supervisor, true);

            //Create 5 users
            for (int i = 0; i < 5; i++)
            {
                var user = new User
                               {
                                   FirstName = "FName" + i,
                                   LastName = "LName" + i,
                                   Salary = i + 1,
                                   FTE = 1,
                                   IsActive = true,
                                   Supervisor = supervisor,
                                   UserName = "UserName" + i,
                                   Email = "username" + i + "@testucdavis.edu"
                               };
                user.Projects.Add(Repository.OfType<Project>().GetByID(5));
                user.FundTypes = fundTypes;
                userId = Guid.NewGuid();

                user.SetUserID(userId);

                UserIds.Add(userId);

                userBLL.EnsurePersistent(user, true);
            }
        }
    }
}