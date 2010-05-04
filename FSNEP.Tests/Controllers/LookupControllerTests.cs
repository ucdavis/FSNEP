using System.Collections.Generic;
using System.Linq;
using FSNEP.Controllers;
using FSNEP.Tests.Core;
using MvcContrib.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class LookupControllerTests : ControllerTestBase<LookupController>
    {
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
                .AssertWasNotCalled(a => a.EnsurePersistent(newProject));//make sure we called persist
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
            //4 projects, 2 are active
            var projects =
                new[] {new Project {IsActive = true}, new Project {IsActive = true}, new Project(), new Project()}.
                    AsQueryable();

            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.Queryable).Return(projects);

            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            var result = Controller.Projects()
                .AssertViewRendered()
                .WithViewData<List<Project>>();

            Assert.AreEqual(2, result.Count, "Should only get the two active projects");
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

            //CreateActivityType calls GetNullableById for ActivityCategory
            var validActivityCategory = new ActivityCategory();
            var validActivityCategoryRepository = FakeRepository<ActivityCategory>();
            validActivityCategoryRepository.Expect(a => a.GetNullableByID(validCategoryId)).Return(validActivityCategory);
            Controller.Repository.Expect(a => a.OfType<ActivityCategory>()).Return(validActivityCategoryRepository).Repeat.Any();

            var newActivityType = new ActivityType() { Name = "ValidActivityType", Indicator = validIndicator};

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
        /// Inactivate ActivityType Redirects On Valid ActivityTypeId
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
    }
}