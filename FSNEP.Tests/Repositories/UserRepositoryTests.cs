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
using UCDArch.Data.NHibernate;
using UCDArch.Testing.Extensions;


namespace FSNEP.Tests.Repositories
{
    [TestClass]
    public class UserRepositoryTests : RepositoryTestBase
    {
        public IUserBLL UserBLL { get; set; }

        /// <summary>
        /// Valid name, 50 characters long
        /// </summary>
        public const string ValidValueName = "123456789 123456789 123456789 123456789 1234567890";
        /// <summary>
        /// Invalid name, 51 characters long
        /// </summary>
        public const string InvalidValueName = "123456789 123456789 123456789 123456789 123456789 1";
        /// <summary>
        /// Valid Salary
        /// </summary>
        public const int ValidValueSalary = 1;
        /// <summary>
        /// Valid FTE
        /// </summary>
        public const int ValidValueFte = 1;

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

            var project1 = projectRepository.GetById(1);
            var project2 = projectRepository.GetById(2);
            var project3 = projectRepository.GetById(3);
            var project4 = projectRepository.GetById(4);

            var currentUser = UserBLL.GetById(UserIds[0]);

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
                    var user = UserBLL.GetById(UserIds[i]);

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
            var projectRepository = new Repository<Project>();

            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true);

            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("currentuser");
            UserBLL.UserAuth.Expect(a => a.GetUser("currentuser")).Return(new FakeMembershipUser(UserIds[0])); //Current user is the first user

            //Give all users project1
            var project1 = projectRepository.Queryable.Where(p => p.Id == 1).Single();
            var project2 = projectRepository.Queryable.Where(p => p.Id == 2).Single();

            var currentUser = UserBLL.GetById(UserIds[0]);
            currentUser.Projects.Add(project1);

            using (var ts = new TransactionScope())
            {
                UserBLL.EnsurePersistent(currentUser);

                for (int i = 1; i < UserIds.Count; i++)
                {
                    var user = UserBLL.GetById(UserIds[i]);

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
            var projectRepository = new Repository<Project>();

            UserBLL.UserAuth.Expect(a => a.IsCurrentUserInRole(RoleNames.RoleProjectAdmin)).Return(true);

            UserBLL.UserAuth.Expect(a => a.CurrentUserName).Return("currentuser");
            UserBLL.UserAuth.Expect(a => a.GetUser("currentuser")).Return(new FakeMembershipUser(UserIds[0])); //Current user is the first user

            //Give all users project1
            var project1 = projectRepository.Queryable.First();

            using (var ts = new TransactionScope())
            {
                foreach (var userID in UserIds)
                {
                    var currentUser = UserBLL.GetById(userID);

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

            Assert.AreEqual(UserIds[0], user.Id);
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
                var user = UserBLL.GetById(UserIds[0]);
                user.IsActive = false;

                UserBLL.EnsurePersistent(user);
                
                ts.CommitTransaction();
            }

            var usersWithoutInactiveUser = UserBLL.GetAllUsers();

            Assert.AreEqual(UserIds.Count - 1, usersWithoutInactiveUser.Count(), "Should return all users except for the inactivated user");
        }

        #region FirstName Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithNullFirstName()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = null,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("FirstName: may not be null or empty");
                //results.AssertContains("FirstName: length must be between 0 and 50");
                //Assert.IsTrue(results.Contains("FirstName: The value cannot be null."), "Expected the validation result to have \"FirstName: The value cannot be null.\"");
                //Assert.IsTrue(results.Contains("FirstName: length must be between 0 and 50"), "Expected the validation result to have \"FirstName: length must be between 0 and 50\"");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithTooLongFirstName()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = InvalidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();
            //may not be null or empty

            var userId = Guid.NewGuid();
            user.SetUserID(userId);

            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count); 
                results.AssertContains("FirstName: length must be between 0 and 50");
                throw;
            }
        }
        #endregion FirstName Tests

        #region LastName Tests
        [TestMethod] 
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithOnlySpacesInLastName()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = " ",
                Salary = ValidValueSalary,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor

            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();
            //may not be null or empty

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("LastName: may not be null or empty");
                //Assert.AreEqual(true, results.Contains("LastName: Required"), "Expected the validation result to have \"LastName: Required\"");    
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithNullLastName()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = null,
                Salary = ValidValueSalary,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();
            //may not be null or empty

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            
            try
            {                
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("LastName: may not be null or empty");
                //results.AssertContains("LastName: length must be between 1 and 50");
                //Assert.AreEqual(true, results.Contains("LastName: Required"), "Expected the validation result to have \"LastName: Required\"");
                //Assert.AreEqual(true, results.Contains("LastName: length must be between 1 and 50"), "Expected the validation result to have \"LastName: length must be between 1 and 50\"");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithLastNameLongerThan50Characters()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = InvalidValueName,
                Salary = ValidValueSalary,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();
            //may not be null or empty

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("LastName: length must be between 0 and 50");
                //Assert.AreEqual(true, results.Contains("LastName: length must be between 1 and 50"), "Expected the validation result to have \"LastName: length must be between 1 and 50\"");
                throw;
            }
        }

        [TestMethod]
        public void UserSavesWithLastName1CharacterLong()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = "1",
                Salary = ValidValueSalary,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();


            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);
        }

        #endregion LastName Tests

        #region Supervisor Test
        [TestMethod] 
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithNullSupervisor()
        {
            var userBLL = new UserBLL(null);
            var user = new User
               {
                   FirstName = ValidValueName,
                   LastName = ValidValueName,
                   Salary = ValidValueSalary,
                   FTE = ValidValueFte,
                   IsActive = true,
                   Supervisor = null,
                   UserName = "UserName"
               };

            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Supervisor: may not be empty");
                //Assert.AreEqual(true, results.Contains("Supervisor: The value cannot be null."), "Expected the validation result to have \"Supervisor: The value cannot be null.\"");
                throw;
            }
        }

        
        #endregion Supervisor Test

        #region Salary Tests
        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithSalaryZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = 0,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();
         

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Salary: Must be greater than zero");
                //Assert.AreEqual(true, results.Contains("Salary: Must be greater than zero"), "Expected the validation result to have \"Salary: Must be greater than zero\"");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithSalaryLessThanZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = -1,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("Salary: Must be greater than zero");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: Salary, Must be greater than zero\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }
        #endregion Salary Tests

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithBenifitRateLessThanZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName, 
                Salary = ValidValueSalary,
                BenefitRate = -1,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("BenefitRate: must be between 0 and 2");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: BenefitRate, must be between 0 and 2\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithBenifitRateGreaterThan2()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                BenefitRate = 2.00001,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("BenefitRate: must be between 0 and 2");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: BenefitRate, must be between 0 and 2\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        public void UserSavesWithBenifitRateOfZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                BenefitRate = 0,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);            
        }

        [TestMethod]
        public void UserSavesWithBenifitRateOf2()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                BenefitRate = 2,
                FTE = ValidValueFte,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithFteLessThanZero()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                FTE = -1,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("FTE: must be between 0.01 and 1");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: FTE, The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithFteGreaterThan1()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                FTE = 1.0009,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("FTE: must be between 0.01 and 1");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: FTE, The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void UserDoesNotSaveWithFteOfZero()
        {
            //TODO: For this test and others expecing an application exception, wrap the whole thing in the try catch block.
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                FTE = 0,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            try
            {
                userBLL.EnsurePersistent(user, true);
            }
            catch (Exception)
            {
                var results = user.ValidationResults().AsMessageList();
                Assert.AreEqual(1, results.Count);
                results.AssertContains("FTE: must be between 0.01 and 1");
                //Assert.AreEqual("Object of type FSNEP.Core.Domain.User could not be persisted\n\n\r\nValidation Errors: FTE, The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).\r\n", message.Message, "Expected Exception Not encountered");
                throw;
            }          
        }

        [TestMethod]
        public void UserSavesWithFteBetweenzeroAnd1()
        {
            var userBLL = new UserBLL(null);
            var user = new User
            {
                FirstName = ValidValueName,
                LastName = ValidValueName,
                Salary = ValidValueSalary,
                FTE = 0.01,
                IsActive = true,
                UserName = "UserName"
            };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = FakeProjects();
            user.FundTypes = FakeFundTypes();

            var userId = Guid.NewGuid();
            user.SetUserID(userId);
            userBLL.EnsurePersistent(user, true);            
        }

        private static IList<FundType> FakeFundTypes()
        {
            var fundTypes = new List<FundType>();

            fundTypes.Add(new FundType { Name = "Name1" });
            fundTypes.Add(new FundType { Name = "Name2" });
            fundTypes.Add(new FundType { Name = "Name3" });
            return fundTypes;
        }

        private static IList<Project> FakeProjects()
        {
            var projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project{Name = "Name2", IsActive = true}
                               };
            return projects;
            
        }
    }
}