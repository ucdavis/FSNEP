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

            

            HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
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

        private void CreateStatus()
        {
            var status1 = new Status { Name = "S1" };
            var status2 = new Status { Name = "S2" };

            var statusRepository = Repository.OfType<Status>();

            statusRepository.EnsurePersistent(status1);
            statusRepository.EnsurePersistent(status2);
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
                                     UserName = "Supervisor"
                                 };

            supervisor.Supervisor = supervisor; //i'm my own boss
            supervisor.SetUserID(userId);

            #region test
            
            supervisor.Projects.Add(Repository.OfType<Project>().GetById(1));

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
                                   UserName = "UserName" + i
                               };
                user.Projects.Add(Repository.OfType<Project>().GetById(5));
                user.FundTypes = fundTypes;
                userId = Guid.NewGuid();

                user.SetUserID(userId);

                UserIds.Add(userId);

                userBLL.EnsurePersistent(user, true);
            }
        }
    }
}