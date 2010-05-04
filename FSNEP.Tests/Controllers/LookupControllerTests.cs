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
    }
}