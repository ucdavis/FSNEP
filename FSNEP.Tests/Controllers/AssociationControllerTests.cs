using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FSNEP.Controllers;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using UCDArch.Testing;


namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class AssociationControllerTests : FSNEP.Tests.Core.ControllerTestBase<AssociationController>
    {
        [TestMethod]
        public void RoutingProjectIdMapsToProjectWithIdParam()
        {
            int? id = 5;

            "~/Administration/Association/Projects/5"
                .ShouldMapTo<AssociationController>(a => a.Projects(id));
        }

        /// <summary>
        /// Projects gets the correct project and accounts.
        /// </summary>
        [TestMethod]
        public void ProjectsGetsCorrectProjectAndAccounts()
        {
            var project = new Project {IsActive = true, Name = "Project2"};
            FakeProjects(project);
            FakeAccounts();

            var result = (ViewResult) Controller.Projects(1);
            Assert.IsNotNull(result);
            var viewModel = (ProjectsAccountsViewModel)result.ViewData.Model;

            Assert.AreEqual(3, viewModel.Projects.Count);
            Assert.AreEqual(3, viewModel.Accounts.Count);
            Assert.AreEqual(project.Name, viewModel.Project.Name);
            foreach (var list in viewModel.Projects)
            {
                Assert.IsTrue(list.IsActive);
            }
            foreach (var list in viewModel.Accounts)
            {
                Assert.IsTrue(list.IsActive);
            }            
        }

        /// <summary>
        /// Associate saves project and account associations.
        /// </summary>
        [TestMethod]
        public void AssociateSavesProjectAndAccountAssociations()
        {
            var project = new Project { IsActive = true, Name = "Project2" };
            project.SetIdTo(1);
            FakeProjects(project);
            FakeAccounts();
            var result = (ViewResult)Controller.Projects(project.Id);
            var viewModel = (ProjectsAccountsViewModel)result.ViewData.Model;

            var accountIds = new int[3];
            for (var i = 0; i < 3; i++)
            {
                accountIds[i] = viewModel.Accounts[i].Id;
            }
            //TODO: Figure out why commented out doesn't work.
            //Controller.Associate(project.Id, accountIds)
            //    .AssertActionRedirect()
            //    .ToAction<AssociationController>(a => a.Projects());
            Controller.Associate(project.Id, accountIds);
            Assert.AreEqual("Accounts successfully associated", Controller.Message);

        }




        #region Helper Methods
        /// <summary>
        /// Fakes the projects.
        /// </summary>
        /// <param name="project">A specific project to test against in the calling method.</param>
        private void FakeProjects(Project project)
        {
            var projects =
                new[]
                    {
                        new Project { IsActive = true, Name = "Project1"}, 
                        project,                          
                        new Project { IsActive = false, Name = "Project3"}, 
                        new Project { IsActive = true, Name = "Project4" },
                        new Project { IsActive = false, Name = "Project5"}
                    }.
                    AsQueryable();

            var projectRepository = FakeRepository<Project>();
            projectRepository.Expect(a => a.Queryable).Return(projects).Repeat.Any();
            projectRepository.Expect(a => a.GetNullableByID(1)).Return(project).Repeat.Any();
            projectRepository.Expect(a => a.GetById(1)).Return(project).Repeat.Any();
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository).Repeat.Any();
        }
        /// <summary>
        /// Fakes the accounts.
        /// </summary>
        private void FakeAccounts()
        {
            
            var accountList = new List<Account>
                                  {
                                      new Account {IsActive = false, Name = "Account1"},
                                      new Account {IsActive = true, Name = "Account2"},
                                      new Account {IsActive = true, Name = "Account3"},
                                      new Account {IsActive = true, Name = "Account4"},
                                      new Account {IsActive = false, Name = "Account5"}
                                  };           
            accountList[0].SetIdTo(1);
            accountList[1].SetIdTo(2);
            accountList[2].SetIdTo(3);
            accountList[3].SetIdTo(4);
            accountList[4].SetIdTo(5);
            var accounts = accountList.AsQueryable();


            var accountRepository = FakeRepository<Account>();
            accountRepository.Expect(a => a.Queryable).Return(accounts).Repeat.Any();
            for (int i = 0; i < 5; i++)
            {
                accountRepository.Expect(a => a.GetById(i+1)).Return(accountList[i]).Repeat.Any();
            }
            

            Controller.Repository.Expect(a => a.OfType<Account>()).Return(accountRepository).Repeat.Any();
        }
        #endregion Helper Methods

    }
}