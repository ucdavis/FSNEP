using System;
using System.Collections.Generic;
using System.Linq;
using FSNEP.Tests.Core;
using FSNEP.Tests.Core.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;
using FSNEP.BLL.Impl;
using Rhino.Mocks;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;
using UCDArch.Core.PersistanceSupport;

namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class UserBllRepositoryTests : RepositoryTestBase
    {
        public IUserBLL UserBLL { get; set; }
        private User Supervisor { get; set;}
        private Project NewProject { get; set; }
        private User ProjAdmin { get; set; }
        private User[] Peons { get; set;}


        #region Init
        public UserBllRepositoryTests()
        {
            var userAuth = MockRepository.GenerateStub<IUserAuth>();        
            UserBLL = new UserBLL(userAuth);
        }


        /// <summary>
        /// Loads the data.
        /// </summary>
        protected override void LoadData()
        {
            using (var ts = new TransactionScope())
            {
                LoadProjects();
                LoadFundTypes();
                LoadUsers();
                ts.CommitTransaction();
            }
        }

        /// <summary>
        /// Loads the fund types.
        /// </summary>
        private void LoadFundTypes()
        {
            for (int i = 1; i <= 3; i++)
            {
                var fundType = new FundType { Name = "FundType" + i };

                Repository.OfType<FundType>().EnsurePersistent(fundType);
            }
        }

        /// <summary>
        /// Create 5 users
        /// 1) Supervisor with Admin role (Linked to 1 active project)
        /// 2) Normal user linked to 3 active projects 1 inactive project
        /// 3) Normal user linked to 2 active projects
        /// 4) Normal user linked to 2 inactive projects
        /// 5) Normal user linked all projects?
        /// </summary>
        private void LoadUsers()
        {
            var users = new User[4];
            Supervisor = CreateValidUser(1);
            Supervisor.Supervisor = Supervisor;
            Supervisor.UserName = "Supervisor1";
            Supervisor.Projects.Add(Repository.OfType<Project>().GetById(1));
            Repository.OfType<User>().EnsurePersistent(Supervisor);

            for (int i = 0; i < 4; i++)
            {
                users[i] = CreateValidUser(i + 1);
                users[i].Supervisor = Supervisor;
            }

            //Now set individual projects for each user

            users[0].Projects.Add(Repository.OfType<Project>().GetById(1));
            users[0].Projects.Add(Repository.OfType<Project>().GetById(2)); //Inactive
            users[0].Projects.Add(Repository.OfType<Project>().GetById(3));
            users[0].Projects.Add(Repository.OfType<Project>().GetById(4));

            users[1].Projects.Add(Repository.OfType<Project>().GetById(1));
            users[1].Projects.Add(Repository.OfType<Project>().GetById(6));

            users[2].Projects.Add(Repository.OfType<Project>().GetById(2)); //Inactive
            users[2].Projects.Add(Repository.OfType<Project>().GetById(5)); //Inactive

            users[3].Projects = Repository.OfType<Project>().GetAll();

            foreach (var user in users)
            {
                Repository.OfType<User>().EnsurePersistent(user);
            }

        }        

        /// <summary>
        /// Loads the projects.
        /// 5 Active Projects 2 Inactive projects
        /// </summary>
        private void LoadProjects()
        {
            for (int i = 1; i <= 7; i++)
            {
                var proj = CreateValidProject(i);
                Repository.OfType<Project>().EnsurePersistent(proj);
            }
        }

        #endregion Init

        #region GetAllProjectsByUser Tests

        /// <summary>
        /// Gets all projects by user returns all active projects for user with admin role.
        /// 1) Supervisor with Admin role (Linked to 1 active project)
        /// Test that Supervisor (With faked Admin role, returns all active projects
        /// </summary>
        [TestMethod]
        public void GetAllProjectsByUserReturnsAllActiveProjectsForUserWithAdminRole()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(true).Repeat.Once();
            var projects = UserBLL.GetAllProjectsByUser(Repository.OfType<Project>()).ToList();
            Assert.IsNotNull(projects);
            Assert.AreEqual(5, projects.Count());
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(1)));
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(2))); //Inactive
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(3)));
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(4)));
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(5))); //Inactive
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(6)));
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(7)));
        }

        /// <summary>
        /// Gets all projects by user returns all active projects for user1.
        /// 2) Normal user linked to 3 active projects 1 inactive project
        /// Test that only 3 specific projects are returned (1, 3, 4)
        /// </summary>
        [TestMethod]
        public void GetAllProjectsByUserReturnsAllActiveProjectsForUser1()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("UserName1").Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.GetUser("UserName1")).Return(new FakeMembershipUser(UserIds[1]));
                
            var projects = UserBLL.GetAllProjectsByUser(Repository.OfType<Project>()).ToList();
            Assert.IsNotNull(projects);
            Assert.AreEqual(3, projects.Count());
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(1)));
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(2))); //Inactive
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(3)));
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(4)));
            
            //Doesn't Have these
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(5))); //Inactive
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(6)));
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(7)));
        }

        /// <summary>
        /// Gets all projects by user returns all active projects for user2.
        /// 3) Normal user linked to 2 active projects
        /// Test that all 2 projects are returned (1 and 6)
        /// </summary>
        [TestMethod]
        public void GetAllProjectsByUserReturnsAllActiveProjectsForUser2()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("UserName2").Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.GetUser("UserName2")).Return(new FakeMembershipUser(UserIds[2]));

            var projects = UserBLL.GetAllProjectsByUser(Repository.OfType<Project>()).ToList();
            Assert.IsNotNull(projects);
            Assert.AreEqual(2, projects.Count());
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(1)));
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(6)));           

            //Doesn't Have these
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(2))); //Inactive
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(3)));
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(4)));
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(5))); //Inactive
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(7)));
        }

        // 4) Normal user linked to 2 inactive projects
        //Test that this causes a problem
        [TestMethod]
        [ExpectedException(typeof(UCDArch.Core.Utils.PostconditionException))]
        public void GetAllProjectsByUserReturnsAllActiveProjectsForUser3()
        {
            try
            {
                UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
                UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("UserName3").Repeat.Any();
                UserBLL.UserAuth.Expect(a => a.GetUser("UserName3")).Return(new FakeMembershipUser(UserIds[3]));

                UserBLL.GetAllProjectsByUser(Repository.OfType<Project>()).ToList();
            }
            catch (Exception message)
            {
                Assert.AreEqual("User must have at least one active project", message.Message);
                throw;
            }
        }

        
        /// <summary>
        /// Gets all projects by user returns all active projects for user4.
        /// 5) Normal user linked all projects?
        /// Test that all active projects are returned, no inactive projects
        /// </summary>
        [TestMethod]
        public void GetAllProjectsByUserReturnsAllActiveProjectsForUser4()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Once();
            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("UserName4").Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.GetUser("UserName4")).Return(new FakeMembershipUser(UserIds[4]));

            var projects = UserBLL.GetAllProjectsByUser(Repository.OfType<Project>()).ToList();
            Assert.IsNotNull(projects);
            Assert.AreEqual(5, projects.Count());
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(1)));            
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(2))); //Inactive
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(3)));
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(4)));
            Assert.IsFalse(projects.Contains(Repository.OfType<Project>().GetById(5))); //Inactive
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(6)));
            Assert.IsTrue(projects.Contains(Repository.OfType<Project>().GetById(7)));
        }

        #endregion GetAllProjectsByUser Tests

        #region GetAllUsers Tests

        /// <summary>
        /// GetAllUsers For A User With RoleProjectAdmin
        /// This will only return users that have the same project(s) as the project Admin user.
        /// </summary>
        [TestMethod]
        public void GetAllUsersForAUserWithRoleProjectAdmin()
        {
            Peons = new User[2];
            SetupDataForGetAllUsersTests();

            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true).Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(false).Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("ProjAdmin").Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.GetUser("ProjAdmin")).Return(new FakeMembershipUser(ProjAdmin.Id)).Repeat.Any();


            var usersLinkedToMyprojects = UserBLL.GetAllUsers().ToList();
            Assert.IsNotNull(usersLinkedToMyprojects);
            Assert.AreEqual(3, usersLinkedToMyprojects.Count);
            Assert.IsTrue(usersLinkedToMyprojects.Contains(ProjAdmin));
            Assert.IsTrue(usersLinkedToMyprojects.Contains(Peons[0]));
            Assert.IsTrue(usersLinkedToMyprojects.Contains(Peons[1]));

        }

        

        /// <summary>
        /// GetAllUsers For A User With RoleAdmin
        /// This will return all users in the repository.
        /// </summary>
        [TestMethod]
        public void GetAllUsersForAUserWithRoleAdmin()
        {
            Peons = new User[2];
            SetupDataForGetAllUsersTests();

            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(false).Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(true).Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("ProjAdmin").Repeat.Any();
            UserBLL.UserAuth.Expect(a => a.GetUser("ProjAdmin")).Return(new FakeMembershipUser(ProjAdmin.Id)).Repeat.Any();


            var usersLinkedToMyprojects = UserBLL.GetAllUsers().ToList();
            Assert.IsNotNull(usersLinkedToMyprojects);
            Assert.AreEqual(UserIds.Count, usersLinkedToMyprojects.Count, "All users should have been retrieved"); //Should be 8, but if more are added to the load data, this will let it pass without changing a hard coded value.
            Assert.IsTrue(usersLinkedToMyprojects.Contains(ProjAdmin));
            Assert.IsTrue(usersLinkedToMyprojects.Contains(Peons[0]));
            Assert.IsTrue(usersLinkedToMyprojects.Contains(Peons[1]));
            Assert.IsTrue(usersLinkedToMyprojects.Contains(Supervisor)); //It has more, this is just a sample
        }

        #endregion GetAllUsers Tests

        #region Helper Methods
        /// <summary>
        /// Creates the valid user.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private User CreateValidUser(int i)
        {
            var user = new User
            {
                FirstName = "FName" + i,
                LastName = "LName" + i,
                Salary = 50000,
                BenefitRate = 2,
                FTE = 1,
                IsActive = true,
                UserName = "UserName" + i,
                Projects = new List<Project>(),
                FundTypes = Repository.OfType<FundType>().GetAll()
            };

            var userId = Guid.NewGuid();
            user.SetUserID(userId);

            UserIds.Add(userId);

            return user;
        }

        /// <summary>
        /// Creates the valid project.
        /// </summary>
        /// <param name="i">The project number being created.</param>
        /// <returns></returns>
        private static Project CreateValidProject(int i)
        {
            var project = new Project {Name = "Project" + i};
            if (i == 2 || i == 5)
            {
                project.Name = "InactiveProject" + i;
                project.IsActive = false;
            }
            else
            {
                project.IsActive = true;
            }
            project.Accounts = new List<Account>();
            return project;
        }

        /// <summary>
        /// Setup Data For GetAllUsers Tests
        /// Create a new project
        /// Create 3 new users, one of which will have it's role faked.
        /// </summary>
        private void SetupDataForGetAllUsersTests()
        {
            //Create a project that is not linked to any other users in the repository.
            using (var ts = new TransactionScope())
            {
                NewProject = CreateValidProject(10);
                Repository.OfType<Project>().EnsurePersistent(NewProject);
                ts.CommitTransaction();
            }
            //Create 3 users that are only linked to the newly created project
            using (var ts = new TransactionScope())
            {
                //Create the project admin
                ProjAdmin = CreateValidUser(10);
                ProjAdmin.Projects.Add(NewProject);
                ProjAdmin.UserName = "ProjAdmin";
                ProjAdmin.Supervisor = Supervisor;
                Repository.OfType<User>().EnsurePersistent(ProjAdmin);

                //Create the two users working on that project
                Peons[0] = CreateValidUser(11);
                Peons[0].Projects.Add(NewProject);
                Peons[0].Supervisor = Supervisor;
                Repository.OfType<User>().EnsurePersistent(Peons[0]);
                Peons[1] = CreateValidUser(12);
                Peons[1].Projects.Add(NewProject);
                Peons[1].Supervisor = Supervisor;
                Repository.OfType<User>().EnsurePersistent(Peons[1]);

                ts.CommitTransaction();
            }
        }
        #endregion Helper Methods
    }
}
