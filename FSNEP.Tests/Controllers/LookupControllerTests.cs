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

        #region Project Tests
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
        }

        [TestMethod]
        public void CreateProjectDoesNotSavesProjectWithLongName()
        {
            var newProject = new Project { Name = "InvalidProjectName-TooLongLongLongLongLongLongLongLongLongLongLong" };

            var projectRepository = FakeRepository<Project>();
            
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            Controller.CreateProject(newProject);

            projectRepository
                .AssertWasNotCalled(a => a.EnsurePersistent(newProject));//make sure we didn't call persist
        }

        [TestMethod]
        public void CreateProjectRedirectsToProjects()
        {
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(FakeRepository<Project>());

            Controller.CreateProject(new Project())
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Projects());
        }

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
        }

        [TestMethod]
        public void InactivateProjectPersistsChangesOnValidProjectId()
        {
            var activeProject = new Project {IsActive = true};
            
            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.GetNullableByID(1)).IgnoreArguments().Return(activeProject);
            
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();

            Controller.InactivateProject(activeProject.ID);
            
            Assert.AreEqual(false, activeProject.IsActive, "Project should have been inactivated");
            projectRepository.AssertWasCalled(a => a.EnsurePersistent(activeProject), a => a.Repeat.Once()); //Make sure we saved the change
        }

        //TODO: Review, do we need this test and is it done correctly? If Yes, add to the other lookup control tests.
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
        }

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
        }

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

        [TestMethod]
        public void RoutingProjectsGetsAllProjects()
        {
            "~/Administration/Lookups/Projects"
                .ShouldMapTo<LookupController>(a => a.Projects());
        }

        [TestMethod]
        public void RoutingInactivateProjectCallsInactivateProjectWithParameter()
        {
            "~/Administration/Lookups/InactivateProject/10"
                .ShouldMapTo<LookupController>(a => a.InactivateProject(10));
        }

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

        }

        [TestMethod]
        public void CreateActivityTypeRedirectsActivityType()
        {
            const int validCategoryId = 1;

            FakeActivityCategory(validCategoryId);

            Controller.Repository.Expect(a => a.OfType<ActivityType>()).Return(FakeRepository<ActivityType>());

            Controller.CreateActivityType(new ActivityType(), validCategoryId)
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.ActivityTypes());
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

            Controller.InactivateActivityType(activeActivityType.ID);

            Assert.AreEqual(false, activeActivityType.IsActive, "ActivityType should have been inactivated");
            activityTypeRepository.AssertWasCalled(a => a.EnsurePersistent(activeActivityType), a => a.Repeat.Once()); //Make sure we saved the change
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

            Controller.InactivateAccount(activeAccount.ID);

            Assert.AreEqual(false, activeAccount.IsActive, "Account should have been inactivated");
            accountRepository.AssertWasCalled(a => a.EnsurePersistent(activeAccount), a => a.Repeat.Once()); //Make sure we saved the change
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

            Controller.InactivateActivityCategory(activeActivityCategory.ID);

            Assert.AreEqual(false, activeActivityCategory.IsActive, "ActivityCategory should have been inactivated");
            activityCategoryRepository.AssertWasCalled(a => a.EnsurePersistent(activeActivityCategory), a => a.Repeat.Once()); //Make sure we saved the change
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