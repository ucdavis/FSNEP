using System;
using System.Collections.Generic;
using CAESArch.BLL;
using CAESArch.BLL.Repositories;
using CAESArch.Core.DataInterfaces;
using FSNEP.BLL.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using FSNEP.Core.Domain;

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
                                                                    CAESArch.Data.NHibernate.NHibernateSessionManager.
                                                                        Instance.GetSession().Connection, null);

            LoadData();

            HibernatingRhinos.NHibernate.Profiler.Appender.NHibernateProfiler.Initialize();
        }

        protected virtual void LoadData()
        {
            using (var ts = new TransactionScope())
            {
                //load base data
                CreateUsers();
                CreateProjects();

                ts.CommitTransaction();
            }
        }

        private void CreateProjects()
        {
            //Create 3 projects
            for (int i = 0; i < 4; i++)
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
                                     IsActive = true
                                 };

            supervisor.Supervisor = supervisor; //i'm my own boss
            supervisor.SetUserID(userId);

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
                                   Supervisor = supervisor
                               };

                userId = Guid.NewGuid();

                user.SetUserID(userId);

                UserIds.Add(userId);

                userBLL.EnsurePersistent(user, true);
            }
        }
    }
}