using System;
using System.Linq;
using CAESArch.BLL;
using CAESArch.BLL.Repositories;
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
            var projectRepository = new Repository<Project>();

            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true);

            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("currentuser");
            UserBLL.UserAuth.Expect(a => a.GetUser("currentuser")).Return(new FakeMembershipUser(UserIds[0])); //Current user is the first user

            var project1 = projectRepository.GetByID(1);
            var project2 = projectRepository.GetByID(2);
            var project3 = projectRepository.GetByID(3);
            var project4 = projectRepository.GetByID(4);

            var currentUser = UserBLL.GetByID(UserIds[0]);

            //The current user gets projects 1 and 3
            currentUser.Projects.Add(project1);
            currentUser.Projects.Add(project3);
            
            //Give projects to the other users
            using (var ts = new TransactionScope())
            {
                //first save the current user
                UserBLL.EnsurePersistent(currentUser);

                for (int i = 1; i < UserIds.Count; i++)
                {
                    var user = UserBLL.GetByID(UserIds[i]);

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

                    UserBLL.EnsurePersistent(user);
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
            var project1 = UserBLL.EntitySet<Project>().Where(p => p.ID == 1).Single();
            var project2 = UserBLL.EntitySet<Project>().Where(p => p.ID == 2).Single();

            var currentUser = UserBLL.GetByID(UserIds[0]);
            currentUser.Projects.Add(project1);

            using (var ts = new TransactionScope())
            {
                UserBLL.EnsurePersistent(currentUser);

                for (int i = 1; i < UserIds.Count; i++)
                {
                    var user = UserBLL.GetByID(UserIds[i]);

                    if (i%2 == 0)
                    {
                        user.Projects.Add(project1);
                    }
                    else
                    {
                        user.Projects.Add(project2);
                    }

                    UserBLL.EnsurePersistent(user);
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
            var project1 = UserBLL.EntitySet<Project>().First();

            using (var ts = new TransactionScope())
            {
                foreach (var userID in UserIds)
                {
                    var currentUser = UserBLL.GetByID(userID);

                    currentUser.Projects.Add(project1);

                    UserBLL.EnsurePersistent(currentUser);
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
                var user = UserBLL.GetByID(UserIds[0]);
                user.IsActive = false;

                UserBLL.EnsurePersistent(user);
                
                ts.CommitTransaction();
            }

            var usersWithoutInactiveUser = UserBLL.GetAllUsers();

            Assert.AreEqual(UserIds.Count - 1, usersWithoutInactiveUser.Count(), "Should return all users except for the inactivated user");
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithOnlySpacesInLastName()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = " ",
                Salary = 1,
                FTE = 1,
                IsActive = true,                
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: LastName, Required\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithNullLastName()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = null,
                Salary = 1,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: LastName, The length of the value must fall within the range \"1\" (Inclusive) - \"50\" (Inclusive).\r\nLastName, Required\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithLastNameLongerThan50Characters()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "123456789 123456789 123456789 123456789 123456789 1",
                Salary = 1,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: LastName, The length of the value must fall within the range \"1\" (Inclusive) - \"50\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        public void UserSavesWithLastName1CharacterLong()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "1",
                Salary = 1,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);            
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithNullSupervisor()
        {
            var userBLL = new UserBLL(null);
            var user = new User
               {
                   FirstName = "FName",
                   LastName = "LName",
                   Salary = 1,
                   FTE = 1,
                   IsActive = true,
                   Supervisor = null,
               };

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: Supervisor, The value cannot be null.\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithSalaryZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 0,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: Salary, Must be greater than zero\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithSalaryLessThanZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = -1,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: Salary, Must be greater than zero\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithBenifitRateLessThanZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName", 
                Salary = 1,
                BenefitRate = -1,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: BenefitRate, The value must fall within the range \"0\" (Inclusive) - \"2\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithBenifitRateGreaterThan2()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 2.00001,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: BenefitRate, The value must fall within the range \"0\" (Inclusive) - \"2\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        public void UserSavesWithBenifitRateOfZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 0,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);            
        }

        [TestMethod] //TODO:Review
        public void UserSavesWithBenifitRateOf2()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 2,
                FTE = 1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithFteLessThanZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 1,
                FTE = -1,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: FTE, The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithFteGreaterThan1()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 1,
                FTE = 1.0009,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: FTE, The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod] //TODO:Review
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithFteOfZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 1,
                FTE = 0,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception message)
            {
                Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: FTE, The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }          
        }

        [TestMethod] //TODO:Review
        public void UserSavesWithFteBetweenzeroAnd1()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                BenefitRate = 1,
                FTE = 0.00001,
                IsActive = true,
            };
            user.Supervisor = user; //I'm my own supervisor

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);            
        }
    }
}