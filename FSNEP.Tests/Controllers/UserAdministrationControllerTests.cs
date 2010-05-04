using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
        /// Create User Saves New user
        /// </summary>
        [TestMethod]
        public void CreateUserSavesNewUser()
        {            
            //TODO: Clean up code.
            const string validValueName = "ValidName";
            const int validValueSalary = 1;
            const int validValueFte = 1;

            FakeProjects();
            FakeFundTypes();
            
            var supervisor = FakeSupervisor();

            #region newUser            
            var newUser = new User
              {
                  FirstName = validValueName,
                  LastName = validValueName,
                  Salary = validValueSalary,
                  FTE = validValueFte,
                  IsActive = true,
                  Supervisor = supervisor,
              };

            var userId = Guid.NewGuid();
            newUser.SetUserID(userId);
            #endregion newUser

            #region Parameters needed for the Create Method
            var projectList = new List<int>();
            var fundTypeList = new List<int>();
            var roleList = new List<string>();            

            projectList.Add(2); //Need to match at least 1 value from FakeProjects
            projectList.Add(3);
            fundTypeList.Add(4);
            fundTypeList.Add(5);
            roleList.Add("Supervisor");
            roleList.Add("Timesheet User");
            #endregion Parameters needed for the Create Method

            var userModel = new CreateUserViewModel
            {
                Question = "Q",
                Answer = "A",
                User = newUser,
                UserName = "ValidUserName",
                Email = "test@test.edu"
            };

            MockMethods(userModel);
            
            //Call the method that the UI would use to create the new user.
            Controller.Create(userModel, supervisor.ID, projectList, fundTypeList, roleList);

        }
       
        /// <summary>
        /// Mocks for the Create method.
        /// This needs the User and Supervisor to be populated in the userModel
        /// </summary>
        /// <param name="userModel"></param>
        private void MockMethods(CreateUserViewModel userModel)
        {
            UserBLL.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBLL.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();

            MembershipCreateStatus createStatus;
            var mockGuid = Guid.NewGuid();                                    
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();

            #region Mocks for the supervisor            
            //We don't need to ignore arguments for this one because we are only doing it for a specific Supervior ID.
            UserBLL.UserAuth.MembershipService.Expect(a => a.GetUser(userModel.User.Supervisor.ID)).Return(memberShipUser).Repeat.Any();
            memberShipUser.Email = "test@test.edu"; //Email for the Supervisor. If we need a different email for a user, this would need to be changed.

            UserBLL.Expect(a => a.GetByID(userModel.User.Supervisor.ID)).Return(userModel.User.Supervisor).Repeat.Any();
            #endregion Mocks for the supervisor

            #region Mocks for the Create method
            //If IgnoreArguments is not used, the params don't match and it isn't mocked.
            UserBLL.UserAuth.MembershipService.Expect(a => a.CreateUser(userModel.UserName, "jaskidjflkajsdlf$#12", userModel.Email, userModel.Question, userModel.Answer, true,
                                                                null, out createStatus)).OutRef(createStatus = MembershipCreateStatus.Success).Return(memberShipUser);
            //If Repeat.any() isn't used, it will return the Guid only once, which means if you debug and inspect the value, it will be null the next time it is looked at.
            memberShipUser.Expect(a => a.ProviderUserKey).IgnoreArguments().Return(mockGuid).Repeat.Any();

            Controller.MessageGateway.Expect(a => a.SendMessageToNewUser(userModel.User, "ignore", "ignore", "ignore", "ignore")).IgnoreArguments().Repeat.Any();
            #endregion Mocks for the Create method
            
            #region Mocks for the URL methods (In Create)
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            Controller.Url.RequestContext.HttpContext.Request.Expect(a => a.Url).Return(new Uri("http://sample.com")).
                            Repeat.Any();
            #endregion Mocks for the URL methods (In Create)

        }

        /// <summary>
        /// Fake a Supervisor to use for a new user.
        /// </summary>
        /// <returns></returns>
        private static User FakeSupervisor()
        {
            var supervisor = new User
            {
                FirstName = "FName",
                LastName = "LName",
                Salary = 1,
                FTE = 1,
                IsActive = true,
            };
            var supervisorId = Guid.NewGuid();
            supervisor.Supervisor = supervisor; //Supervior is own Supervisor.
            supervisor.SetUserID(supervisorId);

            return supervisor;
        }

        /// <summary>
        /// This demonstrates the mock of the CreateUser.
        /// </summary>
        [TestMethod]
        public void MockTest()
        {
            MembershipCreateStatus status;

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
  

            MembershipCreateStatus testStatus; 

            var testMemebershipUser = UserBLL.UserAuth.MembershipService.CreateUser("1Test", "1dfgsdf345234",
                                                                                    "1Test@test.edu", "1Q", "A", true,
                                                                                    null, out testStatus);
            Assert.AreEqual(MembershipCreateStatus.Success, testStatus);
            Assert.IsNotNull(testMemebershipUser.ProviderUserKey);            
        }


        [TestMethod]
        public void MockTest2()
        {
            MembershipCreateStatus status;

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

            
// ReSharper disable RedundantAssignment
            var testStatus = MembershipCreateStatus.UserRejected;  //Prime to a different value to make sure the OutRef works as expected.
// ReSharper restore RedundantAssignment
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
            var projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project{Name = "Name2", IsActive = true}
                               };
            projects[0].SetIdTo(2);
            projects[1].SetIdTo(3);  
            
            var projectRepository = FakeRepository<Project>();

            //This ties the "().Queryable" to return "projects"
            projectRepository.Expect(a => a.Queryable).Return(projects.AsQueryable());

            //This ties the call "Repository.OfType<Project>()" to my repository here "projectRepository"
            Controller.Repository.Expect(a => a.OfType<Project>()).Return(projectRepository);
 
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
                                };
            fundTypes[0].SetIdTo(4);
            fundTypes[1].SetIdTo(5);
            fundTypes[2].SetIdTo(6);
            var fundRepository = FakeRepository<FundType>();
            fundRepository.Expect(a => a.Queryable).Return(fundTypes.AsQueryable());
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
