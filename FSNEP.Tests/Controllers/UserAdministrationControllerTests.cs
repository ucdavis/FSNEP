using System;
using System.Collections.Generic;
using System.Linq;
using FSNEP.BLL.Impl;
using FSNEP.Controllers;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using FSNEP.Core.Domain;
using FSNEP.Core.Abstractions;

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
    }
}
