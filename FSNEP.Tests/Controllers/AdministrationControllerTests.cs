using System.Collections.Generic;
using System.Linq;
using FSNEP.BLL.Impl;
using FSNEP.Controllers;
using FSNEP.Tests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcContrib.TestHelper;
using Rhino.Mocks;
using FSNEP.Core.Domain;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class AdministrationControllerTests : ControllerTestBase<AdministrationController>
    {
        public IUserBLL UserBLL { get; set; }

        protected override void SetupController()
        {
            UserBLL = MockRepository.GenerateStub<IUserBLL>();

            CreateController(UserBLL);
        }

        [TestMethod]
        public void CreateUserReturnsCreateUserViewModel()
        {
            UserBLL.Expect(a => a.GetSupervisors()).Return(new List<User>().AsQueryable());
            UserBLL.Expect(a => a.GetAllProjectsByUser()).Return(new List<Project>().AsQueryable());
            UserBLL.Expect(a => a.GetAvailableFundTypes()).Return(new List<FundType>().AsQueryable());
            
            Controller.CreateUser()
                .AssertViewRendered()
                .WithViewData<CreateUserViewModel>();
        }

        [TestMethod]
        public void ModifyUserRedirectsToCreateUserWithInvalidUsername()
        {
            UserBLL.Expect(a => a.GetUser("BADUSER")).Return(null);

            Controller.ModifyUser("BADUSER")
                .AssertActionRedirect()
                .ToAction<AdministrationController>(a => a.CreateUser());
        }

        [TestMethod]
        public void ModifyUserRedirectsToCreateUserWithEmptyId()
        {
            Controller.ModifyUser(string.Empty)
                .AssertActionRedirect()
                .ToAction<AdministrationController>(a => a.CreateUser());
        }

        [TestMethod]
        public void ModifyUserRedirectsToCreateUserWithNullId()
        {
            Controller.ModifyUser(null)
                .AssertActionRedirect()
                .ToAction<AdministrationController>(a => a.CreateUser());
        }

        [TestMethod]
        public void RoutingCreateUserMapsToCreateUser()
        {
            "~/Administration/CreateUser"
                .ShouldMapTo<AdministrationController>(a => a.CreateUser());
        }

        [TestMethod]
        public void RoutingModifyUserWithNoParamMapsToModifyUserMethodWithEmptyUsername()
        {
            string username = string.Empty;

            "~/Administration/ModifyUser"
                .ShouldMapTo<AdministrationController>(a => a.ModifyUser(username));
        }

        [TestMethod]
        public void RoutingModifyUserWithUsernameMapsToModifyUserMethodWithThatUsername()
        {
            "~/Administration/ModifyUser/testuser"
                .ShouldMapTo<AdministrationController>(a => a.ModifyUser("testuser"));
        }
    }
}
