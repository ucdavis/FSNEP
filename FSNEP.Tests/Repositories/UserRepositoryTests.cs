using System.Linq;
using CAESArch.BLL;
using FSNEP.Tests.Core;
using FSNEP.Tests.Core.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Core.Domain;
using FSNEP.BLL.Impl;
using Rhino.Mocks;
using FSNEP.BLL.Interfaces;
using FSNEP.Core.Abstractions;

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
        public void GetAllUsersReturnsOnlyUsersWithIntersectingProjectsForProjectAdmin()
        {
            var projectBLL = new GenericBLL<Project, int>();

            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true);

            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("currentuser");
            UserBLL.UserAuth.Expect(a => a.GetUser("currentuser")).Return(new FakeMembershipUser(UserIds[0])); //Current user is the first user

            var project1 = projectBLL.Repository.GetByID(1);
            var project2 = projectBLL.Repository.GetByID(2);
            var project3 = projectBLL.Repository.GetByID(3);
            var project4 = projectBLL.Repository.GetByID(4);

            var currentUser = UserBLL.Repository.GetByID(UserIds[0]);

            //The current user gets projects 1 and 3
            currentUser.Projects.Add(project1);
            currentUser.Projects.Add(project3);
            
            //Give projects to the other users
            using (var ts = new TransactionScope())
            {
                //first save the current user
                UserBLL.Repository.EnsurePersistent(currentUser);

                for (int i = 1; i < UserIds.Count; i++)
                {
                    var user = UserBLL.Repository.GetByID(UserIds[i]);

                    if (i == 1) user.Projects.Add(project1); //selected
                    if (i == 2) user.Projects.Add(project2); //not selected
                    
                    if (i == 3) //not selected
                    {
                        user.Projects.Add(project2);
                        user.Projects.Add(project4);
                    }

                    if (i == 4) //selected
                    {
                        user.Projects.Add(project1);
                        user.Projects.Add(project2);
                    }

                    if (i == 5) //selected
                    {
                        user.Projects.Add(project2);
                        user.Projects.Add(project3);
                    }

                    UserBLL.Repository.EnsurePersistent(user);
                }

                ts.CommitTransaction();
            }
            
            var users = UserBLL.GetAllUsers();

            Assert.AreEqual(4, users.Count(), "Should get back the four users associated with either project 1 or 3");
        }

        [TestMethod]
        public void GetAllUsersReturnsOnlyUsersWithSameProjectForProjectAdmin()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true);

            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("currentuser");
            UserBLL.UserAuth.Expect(a => a.GetUser("currentuser")).Return(new FakeMembershipUser(UserIds[0])); //Current user is the first user

            //Give all users project1
            var project1 = UserBLL.Repository.EntitySet<Project>().Where(p => p.ID == 1).Single();
            var project2 = UserBLL.Repository.EntitySet<Project>().Where(p => p.ID == 2).Single();

            var currentUser = UserBLL.Repository.GetByID(UserIds[0]);
            currentUser.Projects.Add(project1);

            using (var ts = new TransactionScope())
            {
                UserBLL.Repository.EnsurePersistent(currentUser);

                for (int i = 1; i < UserIds.Count; i++)
                {
                    var user = UserBLL.Repository.GetByID(UserIds[i]);

                    if (i%2 == 0)
                    {
                        user.Projects.Add(project1);
                    }
                    else
                    {
                        user.Projects.Add(project2);
                    }

                    UserBLL.Repository.EnsurePersistent(user);
                }

                ts.CommitTransaction();
            }

            var users = UserBLL.GetAllUsers();

            Assert.AreEqual(UserIds.Count/2, users.Count(), "Should get back half of all users since only half share the same project");
        }

        [TestMethod]
        public void GetAllUsersReturnsAllUsersWithSameProjectForProjectAdmin()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true);

            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("currentuser");
            UserBLL.UserAuth.Expect(a => a.GetUser("currentuser")).Return(new FakeMembershipUser(UserIds[0])); //Current user is the first user

            //Give all users project1
            var project1 = UserBLL.Repository.EntitySet<Project>().First();

            using (var ts = new TransactionScope())
            {
                foreach (var userID in UserIds)
                {
                    var currentUser = UserBLL.Repository.GetByID(userID);

                    currentUser.Projects.Add(project1);

                    UserBLL.Repository.EnsurePersistent(currentUser);
                }

                ts.CommitTransaction();
            }

            var users = UserBLL.GetAllUsers();

            Assert.AreEqual(UserIds.Count, users.Count(), "Should get back all users since we are all in the same project");
        }

        [TestMethod]
        public void GetUserReturnsCurrentUser()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true);

            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("currentuser");
            UserBLL.UserAuth.Expect(a => a.GetUser("currentuser")).Return(new FakeMembershipUser(UserIds[0])); //Current user is the first user

            var user = UserBLL.GetUser();

            Assert.AreEqual(UserIds[0], user.ID);
        }

        [TestMethod]
        public void GetAllUsersReturnsAllForAdmin()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(true);

            var users = UserBLL.GetAllUsers();

            Assert.AreEqual(UserIds.Count, users.Count(), "All of the users should be retrieved");
        }

        [TestMethod]
        public void GetAllUsersReturnsActiveUsersForAdmin()
        {
            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleAdmin)).Return(true);

            //Make one user inactive
            using (var ts = new TransactionScope())
            {
                var user = UserBLL.Repository.GetByID(UserIds[0]);
                user.IsActive = false;

                UserBLL.Repository.EnsurePersistent(user);
                
                ts.CommitTransaction();
            }

            var usersWithoutInactiveUser = UserBLL.GetAllUsers();

            Assert.AreEqual(UserIds.Count - 1, usersWithoutInactiveUser.Count(), "Should return all users except for the inactivated user");
        }
    }
}