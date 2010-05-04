using System;
using System.Collections.Generic;
using CAESArch.BLL;
using FSNEP.BLL.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Core
{
    public abstract class RepositoryTestBase
    {
        public List<Guid> UserIds { get; set; }

        protected RepositoryTestBase()
        {
            UserIds = new List<Guid>();
        }

        [TestInitialize]
        public void CreateDB()
        {
            Configuration config = new Configuration().Configure();
            new NHibernate.Tool.hbm2ddl.SchemaExport(config).Execute(false, true, false,
                                                                    CAESArch.Data.NHibernate.NHibernateSessionManager.
                                                                        Instance.GetSession().Connection, null);

            LoadData();
        }

        protected virtual void LoadData()
        {
            using (var ts = new TransactionScope())
            {
                //load base data
                CreateUsers();

                ts.CommitTransaction();
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

            userBLL.Repository.EnsurePersistent(supervisor, true);

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

                userBLL.Repository.EnsurePersistent(user, true);
            }
        }
    }
}