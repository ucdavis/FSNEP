using System;
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
        [TestMethod]
        public void CreateUserSavesNewUser()
        {            
            const string validValueName = "ValidName";
            const int validValueSalary = 1;
            const int validValueFte = 1;

            FakeProjects();
            FakeFundTypes();

            #region Supervisor User

            var supervisor = new User
             {
                 FirstName = validValueName,
                 LastName = validValueName,
                 Salary = validValueSalary,
                 FTE = validValueFte,
                 IsActive = true,                
             };
            var supervisorId = Guid.NewGuid();
            supervisor.Supervisor = supervisor; //Supervior is own Supervisor.
            supervisor.SetUserID(supervisorId);            
            UserBLL.Expect(a => a.GetByID(supervisorId)).Return(supervisor).Repeat.Any();            

            #endregion Supervisor User


            #region newUser
            

            var newUser = new User
            {
                FirstName = validValueName,
                LastName = validValueName,
                Salary = validValueSalary,
                FTE = validValueFte,
                IsActive = true,
            };
            newUser.Supervisor = supervisor;

            var userId = Guid.NewGuid();
            newUser.SetUserID(userId);
            #endregion newUser

            #region Parameters needed for the Create Method
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

            

           
            //UserBLL.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            //UserBLL.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();


            //var mockUserId = Guid.NewGuid();
            //var mockUser = MockRepository.GenerateMock<MembershipUser>();
            ////var mockMemember = MockRepository.GenerateStub<MembershipUser>();
            ////mockMemember.Expect(a => a.ProviderUserKey).Return(mockUserId);            
            //mockUser.Expect(m => m.ProviderUserKey).Return(mockUserId);
  
            
            

            //Controller.UserBLL.UserAuth.MembershipService.Expect(
            //    a =>
            //    a.CreateUser(userModel.UserName, "1212dfgsdf", userModel.Email, userModel.Question, userModel.Answer, true,
            //                 null, out status)).OutRef(status = MembershipCreateStatus.Success).Return(mockUser);

            //MembershipCreateStatus createStatus;
            //var testMem = UserBLL.UserAuth.MembershipService.CreateUser(userModel.UserName, "dfgsdf", userModel.Email,
            //                                                            userModel.Question, userModel.Answer, true,
            //                                                            null, out createStatus);

            //var tet = testMem.ProviderUserKey;
            //Assert.IsNotNull(tet);

            MembershipCreateStatus createStatus;
            var mockGuid = Guid.NewGuid();
            //UserBLL = MockRepository.GenerateStub<IUserBLL>();
            UserBLL.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBLL.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();

            //If Repeat.any() isn't used, it will return the Guid only once, which means if you debug and inspect the value, it will be null the next time it is looked at.
            memberShipUser.Expect(a => a.ProviderUserKey).IgnoreArguments().Return(mockGuid).Repeat.Any();
            memberShipUser.Email = "test@test.edu";
            UserBLL.UserAuth.MembershipService.Expect(a => a.GetUser(supervisorId)).IgnoreArguments().Return(memberShipUser).Repeat.Any();

            //If IgnoreArguments is not used, the params don't match and it isn't mocked.
            UserBLL.UserAuth.MembershipService.Expect(a => a.CreateUser(userModel.UserName, "jaskidjflkajsdlf$#12", userModel.Email, userModel.Question, userModel.Answer, true,
                                                                null, out createStatus)).OutRef(createStatus = MembershipCreateStatus.Success).Return(memberShipUser);

            //Controller.Url.RequestContext.HttpContext.Request.Url.Expect()           
            Controller.MessageGateway.Expect(a => a.SendMessageToNewUser(newUser, "ignore", "ignore", "ignore", "ignore")).IgnoreArguments().Repeat.Any();

            Controller.Create(userModel, supervisorId, projectList, fundTypeList, roleList);

        }

        /// <summary>
        /// This demonstrates the mock of the CreateUser.
        /// </summary>
        [TestMethod]
        public void MockTest()
        {
            var status = MembershipCreateStatus.Success;

            var mockGuid = Guid.NewGuid();
            UserBLL = MockRepository.GenerateStub<IUserBLL>();
            UserBLL.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBLL.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();

            //If Repeat.any() isn't used, it will return the Guid only once, which means if you debug and inspect the value, it will be null the next time it is looked at.
            memberShipUser.Expect(a => a.ProviderUserKey).IgnoreArguments().Return(mockGuid).Repeat.Any();

            //If IgnoreArguments is not used, the params don't match and it isn't mocked.
            UserBLL.UserAuth.MembershipService.Expect(a => a.CreateUser("Test", "dfgsdf345234", "Test@test.edu", "Q", "A", true,
                                                                null, out status)).IgnoreArguments().OutRef(status = MembershipCreateStatus.Success).Return(memberShipUser);
  

            var testStatus = MembershipCreateStatus.UserRejected; //Prime to a different value to make sure the OutRef works as expected.
            var testMemebershipUser = UserBLL.UserAuth.MembershipService.CreateUser("1Test", "1dfgsdf345234",
                                                                                    "1Test@test.edu", "1Q", "A", true,
                                                                                    null, out testStatus);
            Assert.AreEqual(MembershipCreateStatus.Success, testStatus);
            Assert.IsNotNull(testMemebershipUser.ProviderUserKey);            
        }


        [TestMethod]
        public void MockTest2()
        {
            var status = MembershipCreateStatus.Success;

            var mockGuid = Guid.NewGuid();
            UserBLL = MockRepository.GenerateStub<IUserBLL>();
            UserBLL.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBLL.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();

            //If Repeat.any() isn't used, it will return the Guid only once, which means if you debug and inspect the value, it will be null the next time it is looked at.
            memberShipUser.Expect(a => a.ProviderUserKey).IgnoreArguments().Return(mockGuid).Repeat.Any();

            //If IgnoreArguments is not used, the params don't match and it isn't mocked.
            UserBLL.UserAuth.MembershipService.Expect(a => a.CreateUser("Test", "dfgsdf345234", "Test@test.edu", "Q", "A", true,
                                                                null, out status)).IgnoreArguments().OutRef(status = MembershipCreateStatus.Success).Return(memberShipUser);


            var testStatus = MembershipCreateStatus.UserRejected; //Prime to a different value to make sure the OutRef works as expected.
            var testMemebershipUser = UserBLL.UserAuth.MembershipService.CreateUser("1Test", "1dfgsdf345234",
                                                                                    "1Test@test.edu", "1Q", "A", true,
                                                                                    null, out testStatus);
            Assert.AreEqual(MembershipCreateStatus.Success, testStatus);
            Assert.IsNotNull(testMemebershipUser.ProviderUserKey);

            memberShipUser.Email = "test@test.edu";

            UserBLL.UserAuth.MembershipService.Expect(a => a.GetUser("test")).Return(
                memberShipUser).Repeat.Any();

            var supervisorEmail = UserBLL.UserAuth.MembershipService.GetUser("test").Email;
            Assert.AreEqual("test@test.edu", supervisorEmail);

        }

        /// <summary>
        /// Generate 2 fake projects.
        /// </summary>
        private void FakeProjects()
        {                  
            //TODO: Assign specific ID's using EntityIdSetter.    
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

        public class MockMessageGateway : IMessageGateway
        {
            public void SendMessage(string to, string subject, string body)
            {
            }

            public void SendMessageToNewUser(User user, string username, string userEmail, string supervisorEmail, string newUserTokenPath)
            {
            }
        }
    }
}
