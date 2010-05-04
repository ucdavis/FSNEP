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