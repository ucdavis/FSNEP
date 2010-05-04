using System;
using System.Collections.Generic;
using System.Linq;
using FSNEP.Controllers;
using FSNEP.Tests.Core;
using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core.Extensions;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class LookupControllerTests : ControllerTestBase<LookupController>
    {
        /// <summary>
        /// 51 characters for Name
        /// </summary>
        public const string InvalidValueName = "123456789 123456789 123456789 123456789 123456789 1";
        /// <summary>
        /// The string that is returned by the validator for a name which exceeds 50 characters.
        /// </summary>
        public const string InvalidNameMessageTooLong = "Name: length must be between 0 and 50";

        #region Project Tests
        /// <summary>
        /// Creates the project saves new project.
        /// </summary>
        [TestMethod]
        public void CreateProjectSavesNewProject()
        {
            var newProject = new Project {Name = "ValidProject"};
            
            var projectRepository = FakeRepository<Project>();
            projectRepository
                .Expect(a => a.EnsurePersistent(Arg<Project>.Is.Anything))
                .WhenCalled(a => newProject = (Project) a.Arguments.First()); //set newproject to the project that was saved

            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            Controller.CreateProject(newProject);

            projectRepository
                .AssertWasCalled(a => a.EnsurePersistent(newProject), a => a.Repeat.Once());//make sure we called persist

            Assert.AreEqual(true, newProject.IsActive, "The created project should be active");
            Assert.AreEqual("ValidProject", newProject.Name);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Project Created Successfully", Controller.Message);
        }

        /// <summary>
        /// Does not save project with the long name.
        /// </summary>
        [TestMethod]
        public void CreateProjectDoesNotSavesProjectWithLongName()
        {
            var newProject = new Project { Name = "InvalidProjectName-TooLongLongLongLongLongLongLongLongLongLongLong" };

            var projectRepository = FakeRepository<Project>();
            
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            Controller.CreateProject(newProject);

            projectRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newProject));//make sure we didn't call persist
            Controller.ModelState.AssertErrorsAre(InvalidNameMessageTooLong);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Project Creation Failed."));

        }

        /// <summary>
        /// Create the project redirects to projects.
        /// </summary>
        [TestMethod]
        public void CreateProjectRedirectsToProjects()
        {
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(FakeRepository<Project>());

            Controller.CreateProject(new Project())
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Projects());
        }

        /// <summary>
        /// Inactivate the project redirects on invalid project id.
        /// </summary>
        [TestMethod]
        public void InactivateProjectRedirectsOnInvalidProjectId()
        {
            const int invalidProjectId = 42;

            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.GetNullableByID(invalidProjectId)).Return(null);

            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            Controller.InactivateProject(invalidProjectId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Projects());
            Assert.AreEqual("Project Not Found", Controller.Message);
        }

        /// <summary>
        /// Inactivate the project persists changes on valid project id.
        /// </summary>
        [TestMethod]
        public void InactivateProjectPersistsChangesOnValidProjectId()
        {
            var activeProject = new Project {IsActive = true};
            
            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.GetNullableByID(1)).IgnoreArguments().Return(activeProject);
            
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();

            Controller.InactivateProject(activeProject.Id);
            
            Assert.AreEqual(false, activeProject.IsActive, "Project should have been inactivated");
            projectRepository.AssertWasCalled(a => a.EnsurePersistent(activeProject), a => a.Repeat.Once()); //Make sure we saved the change
            Assert.AreEqual("Project Removed Successfully", Controller.Message);
        }
        
        /// <summary>
        /// Inactivate Project Does Not Persists Changes On Invalid ProjectId
        /// </summary>
        [TestMethod]
        public void InactivateProjectDoesNotPersistsChangesOnInvalidProjectId()
        {
            var activeProject = new Project { IsActive = true };

            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.GetNullableByID(1)).IgnoreArguments().Return(null);

            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();

            Controller.InactivateProject(42);

            Assert.AreEqual(true, activeProject.IsActive, "Project should have not been inactivated");
            projectRepository.AssertWasNotCalled(a => a.EnsurePersistent(activeProject), a => a.Repeat.Once()); //Make sure we saved the change
            Assert.AreEqual("Project Not Found", Controller.Message);
        }

        /// <summary>
        /// Inactivate the project redirects on valid project id.
        /// </summary>
        [TestMethod]
        public void InactivateProjectRedirectsOnValidProjectId()
        {
            const int validProjectId = 1;
            var validProject = new Project();

            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.GetNullableByID(validProjectId)).Return(validProject);
            
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();

            Controller.InactivateProject(validProjectId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Projects());
            Assert.AreEqual("Project Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// Projects gets only active projects.
        /// </summary>
        [TestMethod]
        public void ProjectsGetsOnlyActiveProjects()
        {
            //5 projects, 3 are active
            var projects =
                new[] { new Project { IsActive = true }, new Project { IsActive = true }, new Project { IsActive = true }, new Project(), new Project() }.
                    AsQueryable();

            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.Queryable).Return(projects);

            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            var result = Controller.Projects()
                .AssertViewRendered()
                .WithViewData<List<Project>>();

            Assert.AreEqual(3, result.Count, "Should only get the three active projects");
        }

        /// <summary>
        /// Routings project gets all projects.
        /// </summary>
        [TestMethod]
        public void RoutingProjectsGetsAllProjects()
        {
            "~/Administration/Lookups/Projects"
                .ShouldMapTo<LookupController>(a => a.Projects());
        }

        /// <summary>
        /// Routing inactivate project calls inactivate project with parameter.
        /// </summary>
        [TestMethod]
        public void RoutingInactivateProjectCallsInactivateProjectWithParameter()
        {
            "~/Administration/Lookups/InactivateProject/10"
                .ShouldMapTo<LookupController>(a => a.InactivateProject(10));
        }

        /// <summary>
        /// Routing create project calls create project.
        /// </summary>
        [TestMethod]
        public void RoutingCreateProjectCallsCreateProject()
        {
            "~/Administration/Lookups/CreateProject"
                .ShouldMapTo<LookupController>(a => a.CreateProject(null));
        }
        #endregion Project Tests

        #region ActivityType Tests
        /// <summary>
        /// Create ActivityType Saves New ActivityType
        /// </summary>
        [TestMethod]
        public void CreateActivityTypeSavesNewActivityType()
        {
            const string validIndicator = "12";
            const int validCategoryId = 1;

            FakeActivityCategory(validCategoryId);            

            var newActivityType = new ActivityType { Name = "ValidActivityType", Indicator = validIndicator};

            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository
                .Expect(a => a.EnsurePersistent(Arg<ActivityType>.Is.Anything))
                .WhenCalled(a => newActivityType = (ActivityType)a.Arguments.First()); //set newActivityType to the activityType that was saved

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository);

            Controller.CreateActivityType(newActivityType, validCategoryId);

            activityTypeRepository
                .AssertWasCalled(a => a.EnsurePersistent(newActivityType), a => a.Repeat.Once());//make sure we called persist

            Assert.AreEqual(true, newActivityType.IsActive, "The created activityType should be active");
            Assert.AreEqual("ValidActivityType", newActivityType.Name);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Activity Type Created Successfully", Controller.Message);
        }

        /// <summary>
        /// Create ActivityType Does Not Save ActivityType With Long Name
        /// </summary>
        [TestMethod]
        public void CreateActivityTypeDoesNotSaveActivityTypeWithLongName()
        {
            const string validIndicator = "12";
            const int validCategoryId = 1;
            
            FakeActivityCategory(validCategoryId);            

            var newActivityType = new ActivityType { 
                Name = InvalidValueName, 
                Indicator = validIndicator };
            
            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository
                .Expect(a => a.EnsurePersistent(Arg<ActivityType>.Is.Anything))
                .WhenCalled(a => newActivityType = (ActivityType)a.Arguments.First()); //set newActivityType to the activityType that was saved

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository);

            Controller.CreateActivityType(newActivityType, validCategoryId);

            activityTypeRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newActivityType));//make sure we didn't call persist

            Controller.ModelState.AssertErrorsAre(InvalidNameMessageTooLong);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Activity Type Creation Failed."));
        }

        /// <summary>
        /// Creates the activity type does not save activity type with invalid category id.
        /// </summary>
        [TestMethod]
        public void CreateActivityTypeDoesNotSaveActivityTypeWithInvalidCategoryId()
        {
            const string validIndicator = "12";
            const int validCategoryId = 1;
            const int invalidCategoryId = 2; 

            FakeActivityCategory(validCategoryId); //Prime the Mock so it returns not null for the valid one only.

            var newActivityType = new ActivityType
            {
                Name = "Valid",
                Indicator = validIndicator
            };

            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository
                .Expect(a => a.EnsurePersistent(Arg<ActivityType>.Is.Anything))
                .WhenCalled(a => newActivityType = (ActivityType)a.Arguments.First()); //set newActivityType to the activityType that was saved

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository);

            Controller.CreateActivityType(newActivityType, invalidCategoryId);

            activityTypeRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newActivityType));//make sure we didn't call persist

            Controller.ModelState.AssertErrorsAre("ActivityCategory: may not be empty");
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Activity Type Creation Failed."));
        }

        /// <summary>
        /// Create activity type redirects to activityType.
        /// </summary>
        [TestMethod]
        public void CreateActivityTypeRedirectsActivityType()
        {
            const int validCategoryId = 1;

            FakeActivityCategory(validCategoryId);

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(FakeRepository<ActivityType>());

            Controller.CreateActivityType(new ActivityType(), validCategoryId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityTypes());            
            //Note: If we examine the ModelState for errors, we see them, but I think this is because this is a mock.
        }

        /// <summary>
        /// Inactivate ActivityType Redirects On Invalid ActivityType Id
        /// </summary>
        [TestMethod]
        public void InactivateActivityTypeRedirectsOnInvalidActivityTypeId()
        {
            const int invalidActivityTypeId = 42;

            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository.Expect(a => a.GetNullableByID(invalidActivityTypeId)).Return(null);

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository);

            Controller.InactivateActivityType(invalidActivityTypeId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityTypes());
            Assert.AreEqual("Activity Type Not Found", Controller.Message);

        }

        /// <summary>
        /// Inactivate ActivityType Persists Changes On Valid ActivityType Id
        /// </summary>
        [TestMethod]
        public void InactivateActivityTypePersistsChangesOnValidActivityTypeId()
        {
            var activeActivityType = new ActivityType { IsActive = true };

            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository.Expect(a => a.GetNullableByID(1)).IgnoreArguments().Return(activeActivityType);

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository).Repeat.Any();

            Controller.InactivateActivityType(activeActivityType.Id);

            Assert.AreEqual(false, activeActivityType.IsActive, "ActivityType should have been inactivated");
            activityTypeRepository.AssertWasCalled(a => a.EnsurePersistent(activeActivityType), a => a.Repeat.Once()); //Make sure we saved the change
            Assert.AreEqual("Activity Type Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// Inactivate the activity type does not persists changes on invalid activity type id.
        /// </summary>
        [TestMethod]
        public void InactivateActivityTypeDoesNotPersistsChangesOnInvalidActivityTypeId()
        {
            var activeActivityType = new ActivityType { IsActive = true };

            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository.Expect(a => a.GetNullableByID(42)).IgnoreArguments().Return(null);

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository).Repeat.Any();

            Controller.InactivateActivityType(42);

            Assert.AreEqual(true, activeActivityType.IsActive, "ActivityType should have not been inactivated");
            activityTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(activeActivityType), a => a.Repeat.Once()); //Make sure we saved the change
            Assert.AreEqual("Activity Type Not Found", Controller.Message);
        }
        
        /// <summary>
        /// Inactivate ActivityType Redirects On Valid ActivityType Id
        /// </summary>
        [TestMethod]
        public void InactivateActivityTypeRedirectsOnValidActivityTypeId()
        {
            const int validActivityTypeId = 1;
            var validActivityType = new ActivityType();

            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository.Expect(a => a.GetNullableByID(validActivityTypeId)).Return(validActivityType);

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository).Repeat.Any();

            Controller.InactivateActivityType(validActivityTypeId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityTypes());
            Assert.AreEqual("Activity Type Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// ActivityType Gets Only Active ActivityTypes
        /// Internally, this needs the CategoryRepository Mocked for Queryable
        /// </summary>
        [TestMethod]
        public void ActivityTypeGetsOnlyActiveActivityTypes()
        {           
            //5 ActivityTypes, 3 are active
            var activityTypes =
                new[]
                    {
                        new ActivityType { Name = "Valid", IsActive = true}, 
                        new ActivityType { Name = "Valid", IsActive = true}, 
                        new ActivityType { Name = "Valid", IsActive = true},
                        new ActivityType { Name = "Valid"}, 
                        new ActivityType { Name = "Valid"}
                    }.AsQueryable();

            var activityTypeRepository = FakeRepository<ActivityType>();
            activityTypeRepository.Expect(a => a.Queryable).Return(activityTypes);

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(activityTypeRepository);

            FakeQueryableActivityCategory();            

            var result = Controller.ActivityTypes()
                .AssertViewRendered()
                .WithViewData<List<ActivityType>>();

            Assert.AreEqual(3, result.Count, "Should only get the three active ActivityTypes");
        }

        /// <summary>
        /// Routing ActivityTypes Gets All ActivityTypes
        /// </summary>
        [TestMethod]
        public void RoutingActivityTypesGetsAllActivityTypes()
        {
            "~/Administration/Lookups/ActivityTypes"
                .ShouldMapTo<LookupController>(a => a.ActivityTypes());
        }

        /// <summary>
        /// Routing InactivateActivityType Calls InactivateActivityType With Parameter
        /// </summary>
        [TestMethod]
        public void RoutingInactivateActivityTypeCallsInactivateActivityTypeWithParameter()
        {
            "~/Administration/Lookups/InactivateActivityType/10"
                .ShouldMapTo<LookupController>(a => a.InactivateActivityType(10));
        }

        /// <summary>
        /// Routing Create ActivityType Calls Create ActivityType
        /// </summary>
        [TestMethod]
        public void RoutingCreateActivityTypeCallsCreateActivityType()
        {
            //TODO: When we get the next version of MvcContrib, check if .ShouldMapTo works for this test
            "~/Administration/Lookups/CreateActivityType".ShouldMapToIgnoringParams("Lookup", "CreateActivityType");

            /*
            var routeData = "~/Administration/Lookups/CreateActivityType".Route();

            Assert.AreEqual("CreateActivityType", routeData.Values["action"]);
            Assert.AreEqual("Lookup", routeData.Values["controller"]);
            */

            /*
            const int activityCategory = 1;

            "~/Administration/Lookups/CreateActivityType"
                .ShouldMapTo<LookupController>(a => a.CreateActivityType(null,activityCategory));
            */
        }
        #endregion ActivityType Tests

        #region Account Tests
        /// <summary>
        /// Create Account Saves New Account
        /// </summary>
        [TestMethod]
        public void CreateAccountSavesNewAccount()
        {
            var newAccount = new Account { Name = "ValidAccount" };

            var accountRepository = FakeRepository<Account>();
            accountRepository
                .Expect(a => a.EnsurePersistent(Arg<Account>.Is.Anything))
                .WhenCalled(a => newAccount = (Account)a.Arguments.First()); //set newAccount to the Account that was saved

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository);

            Controller.CreateAccount(newAccount);

            accountRepository
                .AssertWasCalled(a => a.EnsurePersistent(newAccount), a => a.Repeat.Once());//make sure we called persist

            Assert.AreEqual(true, newAccount.IsActive, "The created Account should be active");
            Assert.AreEqual("ValidAccount", newAccount.Name);
            //TODO: Do we want to add the following two asserts to other tests which complete successfully?
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Account Created Successfully", Controller.Message, "View message incorrect");
        }

        /// <summary>
        /// Create Account Does Not Save Account With Long Name
        /// </summary>
        [TestMethod]
        public void CreateAccountDoesNotSaveAccountWithLongName()
        {
            var newAccount = new Account { Name = InvalidValueName };

            var accountRepository = FakeRepository<Account>();

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository);

            Controller.CreateAccount(newAccount);

            accountRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newAccount));//make sure we didn't call persist

            Controller.ModelState.AssertErrorsAre(InvalidNameMessageTooLong);
            Assert.IsFalse(Controller.ModelState.IsValid);
        }

        /// <summary>
        /// Create Account Redirects To Accounts
        /// </summary>
        [TestMethod]
        public void CreateAccountRedirectsToAccounts()
        {
            Controller.Repository.Expect(a => a.OfType<Account>()).Return(FakeRepository<Account>());

            Controller.CreateAccount(new Account())
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Accounts());
            //Note: This has validation Errors, but I think this is because it is a Mock.
        }

        /// <summary>
        /// Inactivate Account Redirects On Invalid Account Id
        /// </summary>
        [TestMethod]
        public void InactivateAccountRedirectsOnInvalidAccountId()
        {
            const int invalidAccountId = 42;

            var accountRepository = FakeRepository<Account>();
            accountRepository.Expect(a => a.GetNullableByID(invalidAccountId)).Return(null);

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository);

            Controller.InactivateAccount(invalidAccountId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Accounts());
            Assert.AreEqual("Account Not Found", Controller.Message);

        }

        /// <summary>
        /// Inactivate Account Persists Changes On Valid Account Id
        /// </summary>
        [TestMethod]
        public void InactivateAccountPersistsChangesOnValidAccountId()
        {
            var activeAccount = new Account { IsActive = true };

            var accountRepository = FakeRepository<Account>();
            accountRepository.Expect(a => a.GetNullableByID(1)).IgnoreArguments().Return(activeAccount);

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository).Repeat.Any();

            Controller.InactivateAccount(activeAccount.Id);

            Assert.AreEqual(false, activeAccount.IsActive, "Account should have been inactivated");
            accountRepository.AssertWasCalled(a => a.EnsurePersistent(activeAccount), a => a.Repeat.Once()); //Make sure we saved the change
            Assert.AreEqual("Account Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// Inactivate the account does not persists changes on invalid account id.
        /// </summary>
        [TestMethod]
        public void InactivateAccountDoesNotPersistsChangesOnInvalidAccountId()
        {
            var activeAccount = new Account { IsActive = true };

            var accountRepository = FakeRepository<Account>();
            accountRepository.Expect(a => a.GetNullableByID(42)).IgnoreArguments().Return(null);

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository).Repeat.Any();

            Controller.InactivateAccount(42);

            Assert.AreEqual(true, activeAccount.IsActive, "Account should have not been inactivated");
            accountRepository.AssertWasNotCalled(a => a.EnsurePersistent(activeAccount), a => a.Repeat.Once()); //Make sure we didn't save the change
            Assert.AreEqual("Account Not Found", Controller.Message);
        }

        /// <summary>
        /// Inactivate Account Redirects On Valid Account Id
        /// </summary>
        [TestMethod]
        public void InactivateAccountRedirectsOnValidAccountId()
        {
            const int validAccountId = 1;
            var validAccount = new Account();

            var accountRepository = FakeRepository<Account>();
            accountRepository.Expect(a => a.GetNullableByID(validAccountId)).Return(validAccount);

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository).Repeat.Any();

            Controller.InactivateAccount(validAccountId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Accounts());
            Assert.AreEqual("Account Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// Accounts Gets Only Active Accounts
        /// </summary>
        [TestMethod]
        public void AccountsGetsOnlyActiveAccounts()
        {
            //5 Accounts, 3 are active
            var accounts =
                new[] { new Account { IsActive = true }, new Account { IsActive = true }, new Account { IsActive = true }, new Account(), new Account() }.
                    AsQueryable();

            var accountRepository = FakeRepository<Account>();
            accountRepository.Expect(a => a.Queryable).Return(accounts);

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository);

            var result = Controller.Accounts()
                .AssertViewRendered()
                .WithViewData<List<Account>>();

            Assert.AreEqual(3, result.Count, "Should only get the three active accounts");
        }

        /// <summary>
        /// Routing Accounts Gets All Accounts
        /// </summary>
        [TestMethod]
        public void RoutingAccountsGetsAllAccounts()
        {
            "~/Administration/Lookups/Accounts"
                .ShouldMapTo<LookupController>(a => a.Accounts());
        }

        /// <summary>
        /// Routing InactivateAccount Calls Inactivate Account With Parameter
        /// </summary>
        [TestMethod]
        public void RoutingInactivateAccountCallsInactivateAccountWithParameter()
        {
            "~/Administration/Lookups/InactivateAccount/10"
                .ShouldMapTo<LookupController>(a => a.InactivateAccount(10));
        }

        /// <summary>
        /// Routing CreateAccount Calls CreateAccount
        /// </summary>
        [TestMethod]
        public void RoutingCreateAccountCallsCreateAccount()
        {
            "~/Administration/Lookups/CreateAccount"
                .ShouldMapTo<LookupController>(a => a.CreateAccount(null));
        }
        #endregion Account Tests

        #region ActivityCategory Tests       
        /// <summary>
        /// Create ActivityCategory Saves New ActivityCategory
        /// </summary>
        [TestMethod]
        public void CreateActivityCategorySavesNewActivityCategory()
        {
            var newActivityCategory = new ActivityCategory { Name = "ValidActivityCategory" };

            var activityCategoryRepository = FakeRepository<ActivityCategory>();
            activityCategoryRepository
                .Expect(a => a.EnsurePersistent(Arg<ActivityCategory>.Is.Anything))
                .WhenCalled(a => newActivityCategory = (ActivityCategory)a.Arguments.First()); //set newActivityCategory to the activityCategory that was saved

            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository);

            Controller.CreateActivityCategory(newActivityCategory, null);

            activityCategoryRepository
                .AssertWasCalled(a => a.EnsurePersistent(newActivityCategory), a => a.Repeat.Once());//make sure we called persist

            Assert.AreEqual(true, newActivityCategory.IsActive, "The created ActivityCategory should be active");
            Assert.AreEqual("ValidActivityCategory", newActivityCategory.Name);
            Assert.AreEqual("Activity Category Created Successfully", Controller.Message);
        }

        /// <summary>
        /// Create ActivityCategory Does Not Save ActivityCategory With Long Name
        /// </summary>
        [TestMethod]
        public void CreateActivityCategoryDoesNotSaveActivityCategoryWithLongName()
        {
            var newActivityCategory = new ActivityCategory { Name = InvalidValueName };

            var activityCategoryRepository = FakeRepository<ActivityCategory>();

            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository);

            Controller.CreateActivityCategory(newActivityCategory, null);

            activityCategoryRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newActivityCategory));//make sure we didn't call persist
            Controller.ModelState.AssertErrorsAre(InvalidNameMessageTooLong);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Activity Category Creation Failed."));
        }

        /// <summary>
        /// Create ActivityCategory Redirects To ActivityCategories When Called From ActivityCategory
        /// </summary>
        [TestMethod]
        public void CreateActivityCategoryRedirectsToActivityCategoriesWhenCalledFromActivityCategory()
        {
            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(FakeRepository<ActivityCategory>());

            Controller.CreateActivityCategory(new ActivityCategory(), null)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityCategories(null));
        }

        /// <summary>
        /// Create ActivityCategory Redirects To ActivityTypes When Called From ActivityType
        /// </summary>
        [TestMethod]
        public void CreateActivityCategoryRedirectsToActivityTypesWhenCalledFromActivityType()
        {
            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(FakeRepository<ActivityCategory>());

            Controller.CreateActivityCategory(new ActivityCategory(), "ActivityType")
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityTypes());
        }

        /// <summary>
        /// Inactivate ActivityCategory Redirects On Invalid ActivityCategory Id
        /// </summary>
        [TestMethod]
        public void InactivateActivityCategoryRedirectsOnInvalidActivityCategoryId()
        {
            const int invalidActivityCategoryId = 42;

            var activityCategoryRepository = FakeRepository<ActivityCategory>();
            activityCategoryRepository.Expect(a => a.GetNullableByID(invalidActivityCategoryId)).Return(null);

            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository);

            Controller.InactivateActivityCategory(invalidActivityCategoryId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityCategories(null));
            Assert.AreEqual("Activity Category Not Found", Controller.Message);
        }

        /// <summary>
        /// Inactivate ActivityCategory Persists Changes On Valid ActivityCategory Id
        /// </summary>
        [TestMethod]
        public void InactivateActivityCategoryPersistsChangesOnValidActivityCategoryId()
        {
            var activeActivityCategory = new ActivityCategory { IsActive = true };

            var activityCategoryRepository = FakeRepository<ActivityCategory>();
            activityCategoryRepository.Expect(a => a.GetNullableByID(1)).IgnoreArguments().Return(activeActivityCategory);

            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository).Repeat.Any();

            Controller.InactivateActivityCategory(activeActivityCategory.Id);

            Assert.AreEqual(false, activeActivityCategory.IsActive, "ActivityCategory should have been inactivated");
            activityCategoryRepository.AssertWasCalled(a => a.EnsurePersistent(activeActivityCategory), a => a.Repeat.Once()); //Make sure we saved the change
            Assert.AreEqual("Activity Category Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// Inactivate the activity category does not persists changes on invalid activity category id.
        /// </summary>
        [TestMethod]
        public void InactivateActivityCategoryDoesNotPersistsChangesOnInvalidActivityCategoryId()
        {
            var activeActivityCategory = new ActivityCategory { IsActive = true };

            var activityCategoryRepository = FakeRepository<ActivityCategory>();
            activityCategoryRepository.Expect(a => a.GetNullableByID(42)).IgnoreArguments().Return(null);

            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository).Repeat.Any();

            Controller.InactivateActivityCategory(42);

            Assert.AreEqual(true, activeActivityCategory.IsActive, "ActivityCategory should have not been inactivated");
            activityCategoryRepository.AssertWasNotCalled(a => a.EnsurePersistent(activeActivityCategory), a => a.Repeat.Once()); //Make sure we didn't save the change
            Assert.AreEqual("Activity Category Not Found", Controller.Message);
        }

        /// <summary>
        /// Inactivate ActivityCategory Redirects On Valid ActivityCategory Id
        /// </summary>
        [TestMethod]
        public void InactivateActivityCategoryRedirectsOnValidActivityCategoryId()
        {
            const int validActivityCategoryId = 1;
            var validActivityCategory = new ActivityCategory();

            var activityCategoryRepository = FakeRepository<ActivityCategory>();
            activityCategoryRepository.Expect(a => a.GetNullableByID(validActivityCategoryId)).Return(validActivityCategory);

            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository).Repeat.Any();

            Controller.InactivateActivityCategory(validActivityCategoryId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityCategories(null));
            Assert.AreEqual("Activity Category Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// ActivityCategory Gets Only Active ActivityCategories
        /// </summary>
        [TestMethod]
        public void ActivityCategoryGetsOnlyActiveActivityCategories()
        {
            //5 projects, 3 are active
            var activityCategories =
                new[]
                    {
                        new ActivityCategory { IsActive = true }, 
                        new ActivityCategory { IsActive = true }, 
                        new ActivityCategory { IsActive = true }, 
                        new ActivityCategory(), 
                        new ActivityCategory()
                    }.
                    AsQueryable();

            var activityCategoryRepository = FakeRepository<ActivityCategory>();
            activityCategoryRepository.Expect(a => a.Queryable).Return(activityCategories);

            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(activityCategoryRepository);

            var result = Controller.ActivityCategories(null)
                .AssertViewRendered()
                .WithViewData<List<ActivityCategory>>();

            Assert.AreEqual(3, result.Count, "Should only get the three active ActivityCategories");
        }

        /// <summary>
        /// Routing ActivityCategory Gets All ActivityCategories
        /// </summary>
        [TestMethod]
        public void RoutingActivityCategoryGetsAllActivityCategories()
        {
            "~/Administration/Lookups/ActivityCategories"
                .ShouldMapTo<LookupController>(a => a.ActivityCategories(null));
        }

        /// <summary>
        /// Routing Inactivate ActivityCategory Calls InactivateActivityCategory With Parameter
        /// </summary>
        [TestMethod]
        public void RoutingInactivateActivityCategoryCallsInactivateActivityCategoryWithParameter()
        {
            "~/Administration/Lookups/InactivateActivityCategory/10"
                .ShouldMapTo<LookupController>(a => a.InactivateActivityCategory(10));
        }        

        /// <summary>
        /// Routing CreateActivityCategory Calls CreateActivityCategory When Called From ActivityType
        /// </summary>
        [TestMethod]
        public void RoutingCreateActivityCategoryCallsCreateActivityCategoryWhenCalledFromActivityCategory()
        {
            "~/Administration/Lookups/CreateActivityCategory"
                .ShouldMapTo<LookupController>(a => a.CreateActivityCategory(null, null));
        } 
        #endregion ActivityCategory Tests

        #region ExpenseType Tests
        /// <summary>
        /// Create ExpenseType Saves New ExpenseType
        /// </summary>
        [TestMethod]
        public void CreateExpenseTypeSavesNewExpenseType()
        {
            var newExpenseType = new ExpenseType { Name = "ValidExpenseType" };

            var expenseTypeRepository = FakeRepository<ExpenseType>();
            expenseTypeRepository
                .Expect(a => a.EnsurePersistent(Arg<ExpenseType>.Is.Anything))
                .WhenCalled(a => newExpenseType = (ExpenseType)a.Arguments.First()); //set newExpenseType to the ExpenseType that was saved

            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(expenseTypeRepository);

            Controller.CreateExpenseType(newExpenseType);

            expenseTypeRepository
                .AssertWasCalled(a => a.EnsurePersistent(newExpenseType), a => a.Repeat.Once());//make sure we called persist

            Assert.AreEqual(true, newExpenseType.IsActive, "The created ExpenseType should be active");
            Assert.AreEqual("ValidExpenseType", newExpenseType.Name);
            Assert.IsTrue(Controller.ModelState.IsValid);
            Assert.AreEqual("Expense Type Created Successfully", Controller.Message);
        }

        /// <summary>
        /// Create ExpenseType Does Not Save ExpenseType With Long Name
        /// </summary>
        [TestMethod]
        public void CreateExpenseTypeDoesNotSaveExpenseTypeWithLongName()
        {
            var newExpenseType = new ExpenseType { Name = InvalidValueName };

            var expenseTypeRepository = FakeRepository<ExpenseType>();

            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(expenseTypeRepository);

            Controller.CreateExpenseType(newExpenseType);

            expenseTypeRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newExpenseType));//make sure we didn't call persist
            Controller.ModelState.AssertErrorsAre(InvalidNameMessageTooLong);
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Expense Type Creation Failed."));
        }

        /// <summary>
        /// Create ExpenseType Redirects To ExpenseTypes
        /// </summary>
        [TestMethod]
        public void CreateExpenseTypeRedirectsToExpenseTypes()
        {
            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(FakeRepository<ExpenseType>());

            Controller.CreateExpenseType(new ExpenseType())
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ExpenseTypes());
        }

        /// <summary>
        /// Inactivate ExpenseType Redirects On Invalid ExpenseType Id
        /// </summary>
        [TestMethod]
        public void InactivateExpenseTypeRedirectsOnInvalidExpenseTypeId()
        {
            const int invalidExpenseTypeId = 42;

            var expenseTypeRepository = FakeRepository<ExpenseType>();
            expenseTypeRepository.Expect(a => a.GetNullableByID(invalidExpenseTypeId)).Return(null);

            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(expenseTypeRepository);

            Controller.InactivateExpenseType(invalidExpenseTypeId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ExpenseTypes());
            Assert.AreEqual("Expense Type Not Found", Controller.Message);
        }

        /// <summary>
        /// Inactivate ExpenseType Persists Changes On Valid ExpenseType Id
        /// </summary>
        [TestMethod]
        public void InactivateExpenseTypePersistsChangesOnValidExpenseTypeId()
        {
            var activeExpenseType = new ExpenseType { IsActive = true };

            var expenseTypeRepository = FakeRepository<ExpenseType>();
            expenseTypeRepository.Expect(a => a.GetNullableByID(1)).IgnoreArguments().Return(activeExpenseType);

            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(expenseTypeRepository).Repeat.Any();

            Controller.InactivateExpenseType(activeExpenseType.Id);

            Assert.AreEqual(false, activeExpenseType.IsActive, "ExpenseType should have been inactivated");
            expenseTypeRepository.AssertWasCalled(a => a.EnsurePersistent(activeExpenseType), a => a.Repeat.Once()); //Make sure we saved the change
            Assert.AreEqual("Expense Type Removed Successfully", Controller.Message);
        }


        /// <summary>
        /// Inactivate the expense type does not persists changes on invalid expense type id.
        /// </summary>
        [TestMethod]
        public void InactivateExpenseTypeDoesNotPersistsChangesOnInvalidExpenseTypeId()
        {
            var activeExpenseType = new ExpenseType { IsActive = true };

            var expenseTypeRepository = FakeRepository<ExpenseType>();
            expenseTypeRepository.Expect(a => a.GetNullableByID(42)).IgnoreArguments().Return(null);

            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(expenseTypeRepository).Repeat.Any();

            Controller.InactivateExpenseType(42);

            Assert.AreEqual(true, activeExpenseType.IsActive, "ExpenseType should have not been inactivated");
            expenseTypeRepository.AssertWasNotCalled(a => a.EnsurePersistent(activeExpenseType), a => a.Repeat.Once()); //Make sure we didn't save the change
            Assert.AreEqual("Expense Type Not Found", Controller.Message);
        }

        /// <summary>
        /// Inactivate ExpenseType Redirects On Valid ExpenseType Id
        /// </summary>
        [TestMethod]
        public void InactivateExpenseTypeRedirectsOnValidExpenseTypeId()
        {
            const int validExpenseTypeId = 1;
            var validExpenseType = new ExpenseType();

            var expenseTypeRepository = FakeRepository<ExpenseType>();
            expenseTypeRepository.Expect(a => a.GetNullableByID(validExpenseTypeId)).Return(validExpenseType);

            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(expenseTypeRepository).Repeat.Any();

            Controller.InactivateExpenseType(validExpenseTypeId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ExpenseTypes());
            Assert.AreEqual("Expense Type Removed Successfully", Controller.Message);
        }

        /// <summary>
        /// ExpenseTypes Gets Only Active ExpenseTypes
        /// </summary>
        [TestMethod]
        public void ExpenseTypesGetsOnlyActiveExpenseTypes()
        {
            //5 ExpenseType, 3 are active
            var expenseTypes =
                new[]
                    {
                        new ExpenseType { IsActive = true }, 
                        new ExpenseType { IsActive = true }, 
                        new ExpenseType { IsActive = true }, 
                        new ExpenseType(),
                        new ExpenseType()
                    }.AsQueryable();

            var expenseTypeRepository = FakeRepository<ExpenseType>();
            expenseTypeRepository.Expect(a => a.Queryable).Return(expenseTypes);

            Controller.Repository.Expect(a => a.OfType<ExpenseType>()).Return(expenseTypeRepository);

            var result = Controller.ExpenseTypes()
                .AssertViewRendered()
                .WithViewData<List<ExpenseType>>();

            Assert.AreEqual(3, result.Count, "Should only get the three active ExpenseTypes");
        }

        /// <summary>
        /// Routing ExpenseTypes Gets All ExpenseTypes
        /// </summary>
        [TestMethod]
        public void RoutingExpenseTypesGetsAllExpenseTypes()
        {
            "~/Administration/Lookups/ExpenseTypes"
                .ShouldMapTo<LookupController>(a => a.ExpenseTypes());
        }

        /// <summary>
        /// Routing Inactivate ExpenseType Calls Inactivate ExpenseType With Parameter
        /// </summary>
        [TestMethod]
        public void RoutingInactivateExpenseTypeCallsInactivateExpenseTypeWithParameter()
        {
            "~/Administration/Lookups/InactivateExpenseType/10"
                .ShouldMapTo<LookupController>(a => a.InactivateExpenseType(10));
        }

        /// <summary>
        /// Routing CreateExpenseType Calls CreateExpenseType
        /// </summary>
        [TestMethod]
        public void RoutingCreateExpenseTypeCallsCreateExpenseType()
        {
            "~/Administration/Lookups/CreateExpenseType"
                .ShouldMapTo<LookupController>(a => a.CreateExpenseType(null));
        }
        #endregion ExpenseType Tests

        #region HoursInMonth Tests
        /// <summary>
        /// Create HoursInMonth Saves New HoursInMonth
        /// </summary>
        [TestMethod]
        public void CreateHoursInMonthSavesNewHoursInMonth()
        {
            var newYearMonthComposite = new YearMonthComposite(2009, 01);
            var newHoursInMonth = new HoursInMonth(newYearMonthComposite.Year, newYearMonthComposite.Month) { Hours = 999 };


            var hoursInMonthRepository = FakeRepository<HoursInMonth>();
            hoursInMonthRepository
                .Expect(a => a.EnsurePersistent(Arg<HoursInMonth>.Is.Anything))
                .WhenCalled(a => newHoursInMonth = (HoursInMonth)a.Arguments.First()); //set newHoursInMonth to the HoursInMonth that was saved

            Controller.Repository.Expect(a => a.OfType<HoursInMonth>()).Return(hoursInMonthRepository);

            Controller.CreateHoursInMonth(newHoursInMonth, newYearMonthComposite);

            hoursInMonthRepository
                .AssertWasCalled(a => a.EnsurePersistent(newHoursInMonth), a => a.Repeat.Once());//make sure we called persist

            Assert.AreEqual(999, newHoursInMonth.Hours, "The newHoursInMonth should have 999 hours.");
            Assert.AreEqual(newYearMonthComposite, newHoursInMonth.Id, "YearMonthComposite should be the same.");            
            Assert.AreEqual(string.Format("{0:MMMM yyyy}: {1} Hrs", new DateTime(2009, 01, 1), 999), newHoursInMonth.ToString());
            Assert.AreEqual("Hours In Month Created Successfully", Controller.Message);
        }

        /// <summary>
        /// CreateHoursInMonth Does Not Save HoursInMonth With Zero Hours
        /// </summary>
        [TestMethod]
        public void CreateHoursInMonthDoesNotSaveHoursInMonthWithZeroHours()
        {
            var newYearMonthComposite = new YearMonthComposite(2009, 01);
            var newHoursInMonth = new HoursInMonth(newYearMonthComposite.Year, newYearMonthComposite.Month) { Hours = 0 };

            var hoursInMonthRepository = FakeRepository<HoursInMonth>();

            Controller.Repository.Expect(a => a.OfType<HoursInMonth>()).Return(hoursInMonthRepository);

            Controller.CreateHoursInMonth(newHoursInMonth, newYearMonthComposite);

            hoursInMonthRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newHoursInMonth));//make sure we didn't call persist
            Controller.ModelState.AssertErrorsAre("Hours: must be greater than or equal to 1");
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Hours In Month Creation Failed."));
        }

        /// <summary>
        /// CreateHoursInMonth Does Not Save HoursInMonth With Years And Months of Zero
        /// </summary>
        [TestMethod]
        public void CreateHoursInMonthDoesNotSaveHoursInMonthWithYearsAndMonthsOfZero()
        {
            var newYearMonthComposite = new YearMonthComposite(0000, 00);
            var newHoursInMonth = new HoursInMonth(newYearMonthComposite.Year, newYearMonthComposite.Month) { Hours = 100 };

            var hoursInMonthRepository = FakeRepository<HoursInMonth>();

            Controller.Repository.Expect(a => a.OfType<HoursInMonth>()).Return(hoursInMonthRepository);

            Controller.CreateHoursInMonth(newHoursInMonth, newYearMonthComposite);

            hoursInMonthRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newHoursInMonth));//make sure we didn't call persist
            Controller.ModelState.AssertErrorsAre("Year: must be greater than or equal to 2000", "Month: must be a valid month");
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Hours In Month Creation Failed."));
        }

        /// <summary>
        /// CreateHoursInMonth Does Not Save HoursInMonth With Months of 14
        /// </summary>
        [TestMethod]
        public void CreateHoursInMonthDoesNotSaveHoursInMonthWithMonthsOf14()
        {
            var newYearMonthComposite = new YearMonthComposite(2008, 14);
            var newHoursInMonth = new HoursInMonth(newYearMonthComposite.Year, newYearMonthComposite.Month) { Hours = 100 };

            var hoursInMonthRepository = FakeRepository<HoursInMonth>();

            Controller.Repository.Expect(a => a.OfType<HoursInMonth>()).Return(hoursInMonthRepository);

            Controller.CreateHoursInMonth(newHoursInMonth, newYearMonthComposite);

            hoursInMonthRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newHoursInMonth));//make sure we didn't call persist
            Controller.ModelState.AssertErrorsAre("Month: must be a valid month");
            Assert.IsFalse(Controller.ModelState.IsValid);
            Assert.IsTrue(Controller.Message.StartsWith("Hours In Month Creation Failed."));
        }
        #endregion HoursInMonth Tests

        /// <summary>
        /// Fake a Queryable ActivityCategoryRepository.
        /// </summary>
        private void FakeQueryableActivityCategory()
        {
            var validActivityCategories =
                new[]
                    {
                        new ActivityCategory {IsActive = true, Name = "ValidCategory"},
                        new ActivityCategory {IsActive = true, Name = "ValidCategory"}
                    }.AsQueryable();
            var validActivityCategoryRepository = FakeRepository<ActivityCategory>();
            validActivityCategoryRepository.Expect(a => a.Queryable).Return(validActivityCategories);
            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(validActivityCategoryRepository).Repeat.Any();
        }

        /// <summary>
        /// For ActivityType, fake the ActivityCategory
        /// </summary>
        /// <param name="validCategoryId"></param>
        private void FakeActivityCategory(int validCategoryId)
        {
            //CreateActivityType calls GetNullableById for ActivityCategory
            var validActivityCategory = new ActivityCategory{Name = "ValidCategory"};
            var validActivityCategoryRepository = FakeRepository<ActivityCategory>();
            validActivityCategoryRepository.Expect(a => a.GetNullableByID(validCategoryId)).Return(validActivityCategory);
            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(validActivityCategoryRepository).Repeat.Any();
           
        }
    }
}