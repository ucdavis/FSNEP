﻿using System;
using System.Collections.Generic;
using System.Linq;
using FSNEP.BLL.Impl;
using FSNEP.BLL.Interfaces;
using FSNEP.Controllers;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using FSNEP.Core.Domain;
using FSNEP.Core.Abstractions;
using System.Web.Security;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class UserAdministrationControllerTests : ControllerTestBase<UserAdministrationController>
    {
        public IUserBLL UserBLL { get; set; }

        protected override void SetupController()
        {
            UserBLL = MockRepository.GenerateStub<IUserBLL>();
            var messageGateway = MockRepository.GenerateStub<IMessageGateway>();

            CreateController(UserBLL, messageGateway);
        }

        [TestMethod]
        public void ListUsersCallsGetAllUsers()
        {
            var fourUsers = new List<User> {new User(), new User(), new User(), new User()};

            UserBLL.Expect(u=>u.GetAllUsers()).Return(fourUsers.AsQueryable());

            var result = Controller.List()
                            .AssertViewRendered()
                            .WithViewData<List<User>>();

            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public void CreateUserReturnsCreateUserViewModel()
        {
            UserBLL.Expect(a => a.GetSupervisors()).Return(new List<User>().AsQueryable());
            UserBLL.Expect(a => a.GetAllProjectsByUser()).Return(new List<Project>().AsQueryable());
            UserBLL.Expect(a => a.GetAvailableFundTypes()).Return(new List<FundType>().AsQueryable());
            
            Controller.Create()
                .AssertViewRendered()
                .WithViewData<CreateUserViewModel>();
        }

        [TestMethod]
        public void ModifyUserRedirectsToCreateUserWithInvalidUsername()
        {
            UserBLL.Expect(a => a.GetUser("BADUSER")).Return(null);

            Controller.Modify("BADUSER")
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.Create());
        }

        [TestMethod]
        public void ModifyUserRedirectsToCreateUserWithEmptyId()
        {
            Controller.Modify(string.Empty)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.Create());
        }

        [TestMethod]
        public void ModifyUserRedirectsToCreateUserWithNullId()
        {
            Controller.Modify(null)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.Create());
        }

        [TestMethod]
        public void RoutingListUsersMapsToListUser()
        {
            "~/Administration/Users/List"
                .ShouldMapTo<UserAdministrationController>(a => a.List());
        }

        [TestMethod]
        public void RoutingCreateUserMapsToCreateUser()
        {
            "~/Administration/Users/Create"
                .ShouldMapTo<UserAdministrationController>(a => a.Create());
        }

        [TestMethod]
        public void RoutingModifyUserByIdWithParamMapsToModifyUserByIdMethod()
        {
            Guid? id = Guid.NewGuid();
            
            string url = "~/Administration/Users/ModifyById/" + id;
                
            url.ShouldMapTo<UserAdministrationController>(a => a.ModifyById(id));
        }

        [TestMethod]
        public void RoutingModifyUserWithNoParamMapsToModifyUserMethodWithEmptyUsername()
        {
            string username = string.Empty;

            "~/Administration/Users/Modify"
                .ShouldMapTo<UserAdministrationController>(a => a.Modify(username));
        }

        [TestMethod]
        public void RoutingModifyUserWithUsernameMapsToModifyUserMethodWithThatUsername()
        {
            "~/Administration/Users/Modify/testuser"
                .ShouldMapTo<UserAdministrationController>(a => a.Modify("testuser"));
        }

        /// <summary>
        /// WIP. FundTypes fake needs ID to pass the Unit Test.
        /// </summary>
        [TestMethod, Ignore]
        public void CreateUserSavesNewUser()
        {            
            FakeProjects();
            FakeFundTypes();
            

            #region newUser
            const string validValueName = "ValidName";
            const int validValueSalary = 1;
            const int validValueFte = 1;

            var newUser = new User
            {
                FirstName = validValueName,
                LastName = validValueName,
                Salary = validValueSalary,
                FTE = validValueFte,
                IsActive = true,
            };
            newUser.Supervisor = newUser; //I'm my own supervisor //May need or want to change this

            var userId = Guid.NewGuid();
            newUser.SetUserID(userId);
            #endregion newUser

            #region Parameters needed for the Create Method
            var supervisorGuid = newUser.ID;
            var projectList = new List<int>();
            var fundTypeList = new List<int>();
            var roleList = new List<string>();
            #endregion Parameters needed for the Create Method

            projectList.Add(0); //Need to be zero
            projectList.Add(0);
            fundTypeList.Add(0);
            fundTypeList.Add(0);
            roleList.Add("Supervisor");
            roleList.Add("Timesheet User");


            var userModel = new CreateUserViewModel
            {
                Question = "Q",
                Answer = "A",
                User = newUser,
                UserName = "ValidUserName",
                Email = "test@test.edu"
            };

            MembershipCreateStatus status = MembershipCreateStatus.Success;


            UserBLL.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBLL.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();


            var mockUserId = Guid.NewGuid();
            var mockUser = MockRepository.GenerateMock<MembershipUser>();
            //var mockMemember = MockRepository.GenerateStub<MembershipUser>();
            //mockMemember.Expect(a => a.ProviderUserKey).Return(mockUserId);            
            mockUser.Expect(m => m.ProviderUserKey).Return(mockUserId);
  
            
            

            Controller.UserBLL.UserAuth.MembershipService.Expect(
                a =>
                a.CreateUser(userModel.UserName, "dfgsdf", userModel.Email, userModel.Question, userModel.Answer, true,
                             null, out status)).OutRef(status = MembershipCreateStatus.Success).Return(mockUser);

            MembershipCreateStatus createStatus;
            var testMem = UserBLL.UserAuth.MembershipService.CreateUser(userModel.UserName, "dfgsdf", userModel.Email,
                                                                        userModel.Question, userModel.Answer, true,
                                                                        null, out createStatus);

            var tet = testMem.ProviderUserKey;
            Assert.IsNotNull(tet);

            Controller.Create(userModel, supervisorGuid, projectList, fundTypeList, roleList);

        }

        /// <summary>
        /// WIP, Need to figure out why it isn't working
        /// </summary>
        [TestMethod, Ignore]
        public void MockTest()
        {
            var status = MembershipCreateStatus.Success;

            UserBLL = MockRepository.GenerateStub<IUserBLL>();
            UserBLL.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBLL.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();
            memberShipUser.Expect(a => a.ProviderUserKey).Return(Guid.NewGuid());

            UserBLL.UserAuth.MembershipService.Expect(a => a.CreateUser("Test", "dfgsdf345234", "Test@test.edu", "Q", "A", true,
                                                                null, out status)).Return(memberShipUser);
           


            var testStatus = MembershipCreateStatus.Success;
            var testMemebershipUser = UserBLL.UserAuth.MembershipService.CreateUser("1Test", "1dfgsdf345234",
                                                                                    "1Test@test.edu", "1Q", "A", true,
                                                                                    null, out testStatus);
            Assert.IsNotNull(testMemebershipUser.ProviderUserKey);            
        }


        private void FakeProjects()
        {                                  
            var projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project{Name = "Name2", IsActive = true}
                               }.AsQueryable();            
            var projectRepository = FakeRepository<Project>();

            //This ties the "().Queryable" to return "projects"
            projectRepository.Expect(a => a.Queryable).Return(projects);

            //This ties the call "Repository.OfType<Project>()" to my repository here "projectRepository"
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);

            //Note: I can't set/mock the ID of projects, so the code below will never have any matches.  
            /* This is what is calling the above code.
            var projects = from proj in Repository.OfType<Project>().Queryable
                           where projectList.Contains(proj.ID)
                           select proj;
             */
        }

        private void FakeFundTypes()
        {
            var fundTypes = new List<FundType>
                                {
                                    new FundType {Name = "Name1"},
                                    new FundType {Name = "Name2"},
                                    new FundType {Name = "Name3"}
                                }.AsQueryable();
            var fundRepository = FakeRepository<FundType>();
            fundRepository.Expect(a => a.Queryable).Return(fundTypes);
            Controller.Repository.Expect(a => a.OfType<FundType>()).Return(fundRepository);
        }
    }
}
