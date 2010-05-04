using System.Collections.Generic;
using System.Linq;
using CAESArch.Core.DataInterfaces;
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
        private IRepository _repository;
        
        protected override void SetupController()
        {
            _repository = MockRepository.GenerateStub<IRepository>();
            
            CreateController(_repository);
        }

        [TestMethod]
        public void CreateProjectSavesNewProject()
        {
            Project exampleNewProject = null;
            
            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.EnsurePersistent(null)).IgnoreArguments().WhenCalled(
                a => exampleNewProject = (Project) a.Arguments.First());

            _repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            Controller.CreateProject("NewProjectName");

            projectRepository.AssertWasCalled(a => a.EnsurePersistent(null), a => a.IgnoreArguments().Repeat.Once());//make sure we called persist

            Assert.AreEqual(true, exampleNewProject.IsActive, "The created project should be active");
            Assert.AreEqual("NewProjectName", exampleNewProject.Name);
        }

        [TestMethod]
        public void CreateProjectRedirectsToProjects()
        {
            _repository.Expect(a => a.OfType<Project>()).Return(FakeRepository<Project>());

            Controller.CreateProject("ValidProject")
                .AssertActionRedirect()
                .ToAction<LookupController>(a => a.Projects());
        }

        [TestMethod]
        public void InactivateProjectRedirectsOnInvalidProjectId()
        {
            const int invalidProjectId = 42;

            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.GetNullableByID(invalidProjectId)).Return(null);

            _repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

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
            
            _repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();

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
            
            _repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();

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

            _repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            var result = Controller.Projects()
                .AssertViewRendered()
                .WithViewData<List<Project>>();

            Assert.AreEqual(2, result.Count, "Should only get the two active projects");
        }

        [TestMethod]
        public void RoutingProjectListGetsAllProjects()
        {
            "~/Administration/Lookups/Projects"
                .ShouldMapTo<LookupController>(a => a.Projects());
        }
    }
}