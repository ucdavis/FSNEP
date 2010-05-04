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
using FSNEP.Tests.Core.Extensions;
using UCDArch.Testing;
using Rhino.Mocks.Interfaces;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class UserAdministrationControllerTests : Core.ControllerTestBase<UserAdministrationController>
    {
        public IUserBLL UserBll { get; set; }

        protected override void SetupController()
        {
            UserBll = MockRepository.GenerateStub<IUserBLL>();
            var messageGateway = MockRepository.GenerateStub<IMessageGateway>();

            CreateController(UserBll, messageGateway);
            var fakeContext =
                MockRepository.GenerateStub<UCDArch.Core.PersistanceSupport.IDbContext>();
            UserBll.Expect(a => a.DbContext).Return(fakeContext).Repeat.Any();
        }

        [TestMethod]
        public void ListUsersCallsGetAllUsers()
        {
            var fourUsers = new List<User> {new User(), new User(), new User(), new User()};

            UserBll.Expect(u=>u.GetAllUsers()).Return(fourUsers.AsQueryable());
            UserBll.Expect(u => u.GetAllProjectsByUser(null)).IgnoreArguments().Return(new List<Project>().AsQueryable());

            var result = Controller.List(null)
                            .AssertViewRendered()
                            .WithViewData<UserListViewModel>();

            Assert.AreEqual(4, result.Users.Count());
        }

        [TestMethod]
        public void CreateUserReturnsCreateUserViewModel()
        {
            UserBll.Expect(a => a.GetSupervisors()).Return(new List<User>().AsQueryable());
            UserBll.Expect(a => a.GetAllProjectsByUser(null)).IgnoreArguments().Return(new List<Project>().AsQueryable());
            UserBll.Expect(a => a.GetAvailableFundTypes(null)).IgnoreArguments().Return(new List<FundType>().AsQueryable());
            
            Controller.Create()
                .AssertViewRendered()
                .WithViewData<CreateUserViewModel>();
        }

        #region Redirect Tests
        [TestMethod]
        public void ModifyUserRedirectsToCreateUserWithInvalidUsername()
        {
            UserBll.Expect(a => a.GetUser("BADUSER")).Return(null);

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
        #endregion Redirect Tests

        #region Routing Tests
        [TestMethod]
        public void RoutingListUsersMapsToListUser()
        {
            "~/Administration/Users/List"
                .ShouldMapTo<UserAdministrationController>(a => a.List(null));
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
        #endregion Routing Tests

        #region Create user tests

        #region First Name Validation Tests
        /// <summary>
        /// Create User Saves New user
        /// Redirects to Home Controller Index
        /// </summary>
        [TestMethod]
        public void CreateUserSavesNewUser()
        {            
            const string validValueName = "ValidName";
            const int validValueSalary = 1;
            const int validValueFte = 1;

            //FakeProjects(); //Don't need anymore.
            //FakeFundTypes(); //Don't need anymore.
            
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
            var roleList = new List<string> {RoleNames.RoleSupervisor, RoleNames.RoleTimeSheet};

            #endregion Parameters needed for the Create Method

            var userModel = new CreateUserViewModel
            {
                Question = "Q",
                Answer = "A",
                User = newUser,
                UserName = "ValidUserName",
                Email = "test@test.edu"
            };

            CreateAndAttachProjectsToUser(userModel);
            CreateAndAttachFundTypesToUser(userModel, false);
            

            MockMethods(userModel, MembershipCreateStatus.Success);

            
            
            //Call the method that the UI would use to create the new user.
            //Ok, it has been changed so that a successful create will redirect back to the list of users.
            Controller.Create(userModel, roleList)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Created Successfully", Controller.Message);
        }


  

        /// <summary>
        /// Previously, spaces only were allowed. A change to the user.cs not makes this invalid. (2009/10/16)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSpacesOnlyInFirstName()
        {
            const string validValueName = "  ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FirstName = validValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("FirstName: may not be null or empty");
        }
        

        /// <summary>
        /// Creates a user with a invalid first name of 51 characters.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithTooLongFirstName()
        {
            const string invalidValueName = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FirstName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("FirstName: length must be between 0 and 50");            
        }        

        /// <summary>
        /// Creates a user with a invalid first name of null.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullFirstName()
        {
            const string invalidValueName = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FirstName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());            
            newUserModel.ViewData.ModelState.AssertErrorsAre("FirstName: may not be null or empty");
        }        
        #endregion First Name Validation Tests

        #region Last Name Validation Tests
        /// <summary>
        /// Creates a user with a invalid last name of 51 characters.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithTooLongLastName()
        {
            const string invalidValueName = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.LastName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("LastName: length must be between 0 and 50");
        }

        /// <summary>
        /// Creates a user with a invalid last name of null.
        /// User is not saved.
        /// Ensures the correct message is displayed. The required validation is hit before the length validation.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullLastName()
        {
            const string invalidValueName = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.LastName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("LastName: may not be null or empty");            
        }

        /// <summary>
        /// Creates a user with a invalid last name spaces only.
        /// User is not saved.
        /// Ensures the correct message is displayed. The Required Validator checks for null and spaces only.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSpacesOnlyInLastName()
        {
            const string invalidValueName = "  ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.LastName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("LastName: may not be null or empty"); 
        }

        #endregion Last Name Validation Tests

        #region Other Validation Tests
        /// <summary>
        /// Creates a user with a null supervisorId.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullSupervisorId()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            userModel.User.Supervisor = null;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Supervisor: may not be empty"); 
        } 
        
        /// <summary>
        /// Creates a user with an empty Project List (The User didn't select one from the list).
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithEmptyProjectList()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            userModel.User.Projects = new List<Project>();

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Projects: may not be null or empty"); 
        }

        /// <summary>
        /// Creates a user with a null Fund Type List (The User didn't select one from the list).
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithEmptyFundTypeList()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            userModel.User.FundTypes = new List<FundType>();

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("FundTypes: may not be null or empty"); 
        }

        /// <summary>
        /// Creates a user with a null Role List (The User didn't select one from the list).
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullRoleList()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            var newUserModel = (ViewResult)Controller.Create(userModel, null);
            newUserModel.ViewData.ModelState.AssertErrorsAre("User must have at least one role"); 
        }

        /// <summary>
        /// Creates the user does not save with empty role list.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithEmptyRoleList()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            var newUserModel = (ViewResult)Controller.Create(userModel, new List<string>());
            newUserModel.ViewData.ModelState.AssertErrorsAre("User must have at least one role");
        }

        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a duplicate user name.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithDuplicateUser()
        {          
            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.DuplicateUserName);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Username already exists"); 
        }
        #endregion Other Validation Tests

        #region MembershipCreateStatus tests that still need to be done. These will fail when code is added
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// Note: this is turned off in the Web.Config
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorDuplicateEmail()
        {
            
            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.DuplicateEmail);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Duplicate Email");                      
        }
                
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorDuplicateProviderUserKey()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.DuplicateProviderUserKey);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Duplicate Provider User Key"); 
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidAnswer()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidAnswer);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Invalid Answer");             
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidEmail()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidEmail);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Invalid Email"); 
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidPassword()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidPassword);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Invalid Password"); 
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidProviderUserKey()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidProviderUserKey);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Invalid Provider User Key"); 
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidQuestion()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidQuestion);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Invalid Question"); 
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidUserName()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidUserName);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Invalid User Name"); 
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorProviderError()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.ProviderError);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed Provider Error"); 
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the correct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorUserRejected()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.UserRejected);

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Create Failed User Rejected"); 
        }
        #endregion MembershipCreateStatus tests that still need to be done. These will fail when code is added
        
        #region Salary Validation Tests
        /// <summary>
        /// Creates a user with a invalid salary .
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSalaryLessThanZero()
        {
            const double invalidValueSalary = -1.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.Salary = invalidValueSalary;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Salary: Must be greater than zero"); 
        }

        /// <summary>
        /// Creates a user with a invalid salary .
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSalaryOfZero()
        {
            const double invalidValueSalary = 0.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.Salary = invalidValueSalary;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Salary: Must be greater than zero"); 
        }
        #endregion Salary Validation Tests

        #region Benefit Rate Valadation Tests
        /// <summary>
        /// Creates a user with a invalid Benefit Rate .
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithBenefitRateLessThanZero()
        {
            const double invalidValueBenefitRate = -1.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = invalidValueBenefitRate;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("BenefitRate: must be between 0 and 2"); 
        }
        /// <summary>
        /// Creates a user with a invalid Benefit Rate .
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithBenefitRateGreaterThanTwo()
        {
            const double invalidValueBenefitRate = 2.000001;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = invalidValueBenefitRate;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("BenefitRate: must be between 0 and 2"); 
        }

        /// <summary>
        /// Creates a user with a valid Benefit Rate of zero.
        /// User is saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserSavesWithBenefitRateOfZero()
        {
            const double validValueBenefitRate = 0;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = validValueBenefitRate;

            Controller.Create(userModel, CreateListOfRoles())
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Created Successfully", Controller.Message);
        }

        /// <summary>
        /// Creates a user with a valid Benefit Rate of two.
        /// User is saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserSavesWithBenefitRateOfTwo()
        {
            const double validValueBenefitRate = 2.000;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = validValueBenefitRate;

            Controller.Create(userModel, CreateListOfRoles())
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Created Successfully", Controller.Message);
            Assert.AreEqual(validValueBenefitRate, userModel.User.BenefitRate); //Make sure it wasn't changed?
        }
        #endregion Benefit Rate Valadation Tests

        #region FTE Validation Tests
        /// <summary>
        /// Creates a user with a invalid FTE.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithFteLessThanZero()
        {
            const double invalidValueFte = -1.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FTE = invalidValueFte;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("FTE: must be between 0.01 and 1"); 
        }
        /// <summary>
        /// Creates a user with a invalid FTE.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithFteOfZero()
        {
            const double invalidValueFte = 0.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FTE = invalidValueFte;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("FTE: must be between 0.01 and 1"); 
        }
        /// <summary>
        /// Creates a user with a invalid FTE.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithFteGreaterThanOne()
        {
            const double invalidValueFte = 1.0001;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FTE = invalidValueFte;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("FTE: must be between 0.01 and 1"); 
        }
        #endregion FTE Validation Tests

        #region userName Tests
        /// <summary>
        /// Creates a user with a invalid user name of 51 characters.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithUserNameMoreThan50Characters()
        {
            const string invalidValueUserName = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.UserName = invalidValueUserName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("UserName: Must be between 1 and 50 characters long"); 
        }
        /// <summary>
        /// Creates a user with a invalid user name of null.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullUserName()
        {
            const string invalidValueUserName = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.UserName = invalidValueUserName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("UserName: may not be null or empty",
                "UserName: Username should be set upon creation"); 
        }
        /// <summary>
        /// Creates a user with a invalid user name of Spaces Only.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSpacesOnlyInUserName()
        {
            const string invalidValueUserName = "   ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.UserName = invalidValueUserName;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("UserName: may not be null or empty",
                "UserName: Username should be set upon creation"); 
        } 
        #endregion userName Tests

        #region Question Tests
        /// <summary>
        /// Creates a user with a invalid Question of 51 characters.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithQuestionMoreThan50Characters()
        {
            const string invalidValueQuestion = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Question = invalidValueQuestion;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Question: Must be between 1 and 50 characters long");
        }
        /// <summary>
        /// Creates a user with a invalid Question of null.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullQuestion()
        {
            const string invalidValueQuestion = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Question = invalidValueQuestion;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Question: may not be null or empty"); 
        }
        /// <summary>
        /// Creates a user with a invalid Question of Spaces Only.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSpacesOnlyInQuestion()
        {
            const string invalidValueQuestion = "   ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Question = invalidValueQuestion;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Question: may not be null or empty");
        }
        #endregion Question Tests

        #region Answer Tests
        /// <summary>
        /// Creates a user with a invalid Question of 51 characters.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithAnswerMoreThan50Characters()
        {
            const string invalidValueAnswer = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Answer = invalidValueAnswer;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Answer: Must be between 1 and 50 characters long");
        }
        /// <summary>
        /// Creates a user with a invalid Answer of null.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullAnswer()
        {
            const string invalidValueAnswer = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Answer = invalidValueAnswer;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Answer: may not be null or empty");            
        }
        /// <summary>
        /// Creates a user with a invalid Answer of Spaces Only.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSpacesOnlyInAnswer()
        {
            const string invalidValueAnswer = "   ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Answer = invalidValueAnswer;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Answer: may not be null or empty");
        }
        #endregion Answer Tests

        #region Email Tests
        /// <summary>
        /// Creates a user with a invalid Email of null.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullEmail()
        {
            const string invalidValueEmail = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Email = invalidValueEmail;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Email: may not be null or empty",
                "Email: Email should be set upon creation");
        }
        /// <summary>
        /// Creates a user with a invalid Email of Spaces Only.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSpacesOnlyEmail()
        {
            const string invalidValueEmail = " ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Email = invalidValueEmail;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre(
                "Email: may not be null or empty",
                "Email: may not be null or empty",
                "Email: Must be a valid email address");
        }
        /// <summary>
        /// Creates a user with a invalid Email that is too long and invalid.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithTooLongEmailAndInvalid()
        {
            const string invalidValueEmail = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Email = invalidValueEmail;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Email: Must be between 1 and 50 characters long",
                "Email: Must be a valid email address");            
        }

        /// <summary>
        /// Creates a user with a invalid Email that is too long.
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithTooLongEmail()
        {
            const string invalidValueEmail = "123456789@12345678901234567890123456789.12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.Email = invalidValueEmail;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Email: Must be between 1 and 50 characters long",
                "Email: Must be a valid email address"); 
        }
        /// <summary>
        /// Creates a user with a invalid Email Regex check.
        /// RegexValidator(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithInvalidEmail1()
        {
            const string invalidEmail = " 123@123.123";

            CreateUserViewModel userModel = CreateValidUserModel();

            userModel.Email = invalidEmail;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Email: Must be a valid email address");          
        }
        /// <summary>
        /// Creates a user with a invalid Email Regex check.
        /// RegexValidator(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase
        /// User is not saved.
        /// Ensures the correct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithInvalidEmail2()
        {
            const string invalidEmail = "123.1*23@123.COM";

            CreateUserViewModel userModel = CreateValidUserModel();

            userModel.Email = invalidEmail;

            var newUserModel = (ViewResult)Controller.Create(userModel, CreateListOfRoles());
            newUserModel.ViewData.ModelState.AssertErrorsAre("Email: Must be a valid email address");        
        }
        #endregion Email Tests
        #endregion Create user tests

        #region Modify user tests
        #region Modify valid changes saves for each type of value that can be modified.

        /// <summary>
        /// Modifies the first name of the existing valid user saves valid changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesFirstName()
        {
            const string newValidValue = "NewFirstName";

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FirstName = newValidValue; //Change it

            Assert.AreNotEqual(newValidValue, userModelOriginal.User.FirstName, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(newValidValue, userModelOriginal.User.FirstName, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies the last name of the existing valid user saves valid changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesLastName()
        {
            const string newValidValue = "NewLastName";

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.LastName = newValidValue; //Change it

            Assert.AreNotEqual(newValidValue, userModelOriginal.User.LastName, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(newValidValue, userModelOriginal.User.LastName, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies the salary of the existing valid user saves valid changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesSalary()
        {
            const int newValidValue = 200000;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.Salary = newValidValue; //Change it

            Assert.AreNotEqual(newValidValue, userModelOriginal.User.Salary, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(newValidValue, userModelOriginal.User.Salary, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies FTE of the existing valid user saves valid changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesFte()
        {
            const double newValidValue = 0.5;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FTE = newValidValue; //Change it

            Assert.AreNotEqual(newValidValue, userModelOriginal.User.FTE, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(newValidValue, userModelOriginal.User.FTE, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies benefit rate of the existing valid user saves valid changes .
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesBenefitRate()
        {
            const double newValidValue = 1.5;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.BenefitRate = newValidValue; //Change it

            Assert.AreNotEqual(newValidValue, userModelOriginal.User.BenefitRate, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(newValidValue, userModelOriginal.User.BenefitRate, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies "is active" of the existing valid user saves valid changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesIsActive()
        {
            const bool newValidValue = false;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.IsActive = newValidValue; //Change it

            Assert.AreNotEqual(newValidValue, userModelOriginal.User.IsActive, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(newValidValue, userModelOriginal.User.IsActive, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies the supervisor of the existing valid user saves valid changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesSupervisor()
        {
            var newFakeSupervisor = FakeSupervisor();

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.Supervisor = newFakeSupervisor; //Change it

            Assert.AreNotEqual(newFakeSupervisor, userModelOriginal.User.Supervisor, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(newFakeSupervisor, userModelOriginal.User.Supervisor, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies the existing valid user saves valid changes to fund types.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesFundTypes()
        {
            //Based on CreateValidUserModel, this will deselect 2 fund types.
            var fundTypes = new List<FundType>
                                {
                                    new FundType {Name = "Name2"}
                                };
            fundTypes[0].SetIdTo(5);

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FundTypes = fundTypes; //Change it

            Assert.AreNotEqual(fundTypes, userModelOriginal.User.IsActive, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(fundTypes, userModelOriginal.User.FundTypes, "Value was not changed in the modify method.");
        }

        /// <summary>
        /// Modifies the existing valid user saves valid changes to projects.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserSavesValidChangesProjects()
        {
            var projects = new List<Project>
                               {
                                   new Project{Name = "Name2", IsActive = true}
                               };
            projects[0].SetIdTo(3);

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.Projects = projects; //Change it

            Assert.AreNotEqual(projects, userModelOriginal.User.Projects, "Value was changed before we expected it to be.");

            Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName)
                .AssertActionRedirect()
                .ToAction<UserAdministrationController>(a => a.List(null));
            Assert.AreEqual("ValidUserName Modified Successfully", Controller.Message);
            Assert.IsTrue(Controller.ViewData.ModelState.IsValid);
            Assert.AreEqual(projects, userModelOriginal.User.Projects, "Value was not changed in the modify method.");
        }

        


        #endregion Modify valid changes saves for each type of value that can be modified.

        #region Modify to invalid does not save and redirects back to the view


        /// <summary>
        /// Modifies the first name of the existing valid user does not save with too long changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithTooLongChangesFirstName()
        {
            const string newInvalidValue = "123456789 123456789 123456789 123456789 12345678901";

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FirstName = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.FirstName, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("FirstName: length must be between 0 and 50");
        }

        /// <summary>
        /// Modifies the first name of the existing valid user does not save with null changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithNullChangesFirstName()
        {
            const string newInvalidValue = null;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FirstName = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.FirstName, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("FirstName: may not be null or empty");
        }

        /// <summary>
        /// Modifies the last name of the existing valid user does not save withtoo long changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithtooLongChangesLastName()
        {
            const string newInvalidValue = "123456789 123456789 123456789 123456789 12345678901";

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.LastName = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.LastName, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("LastName: length must be between 0 and 50");
        }

        /// <summary>
        /// Modifies the last name of the existing valid user does not save with null changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithNullChangesLastName()
        {
            const string newInvalidValue = null;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.LastName = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.LastName, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("LastName: may not be null or empty");            
        
        }

        /// <summary>
        /// Modifies the last name of the existing valid user does not save with spaces only changes.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithSpacesOnlyChangesLastName()
        {
            const string newInvalidValue = "   ";

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.LastName = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.LastName, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("LastName: may not be null or empty"); 
        }

        /// <summary>
        /// Modifies the existing valid user does not save with less than zero changes salary.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithLessThanZeroChangesSalary()
        {
            const int newInvalidValue = -1;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.Salary = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.Salary, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("Salary: Must be greater than zero"); 
        }

        /// <summary>
        /// Modifies the existing valid user does not save with zero changes salary.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithZeroChangesSalary()
        {
            const int newInvalidValue = 0;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.Salary = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.Salary, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("Salary: Must be greater than zero");
        }

        /// <summary>
        /// Modifies the existing valid user does not save with less than zero changes fte.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithLessThanZeroChangesFte()
        {
            const double newInvalidValue = -0.0001;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FTE = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.FTE, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("FTE: must be between 0.01 and 1"); 
        }

        /// <summary>
        /// Modifies the existing valid user does not save with zero changes fte.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithZeroChangesFte()
        {
            const double newInvalidValue = 0;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FTE = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.FTE, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("FTE: must be between 0.01 and 1");

        }

        /// <summary>
        /// Modifies the existing valid user does not save with greater than one changes fte.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithGreaterThanOneChangesFte()
        {
            const double newInvalidValue = 1.001;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FTE = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.FTE, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("FTE: must be between 0.01 and 1");

        }

        /// <summary>
        /// Modifies the existing valid user does not save with less than zero changes benefit rate.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithLessThanZeroChangesBenefitRate()
        {
            const double newInvalidValue = -1;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.BenefitRate = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.BenefitRate, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("BenefitRate: must be between 0 and 2");
        }

        /// <summary>
        /// Modifies the existing valid user does not save with greater than two changes benefit rate.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithGreaterThanTwoChangesBenefitRate()
        {
            const double newInvalidValue = 3;

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.BenefitRate = newInvalidValue; //Change it

            Assert.AreNotEqual(newInvalidValue, userModelOriginal.User.BenefitRate, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("BenefitRate: must be between 0 and 2");
        }

        /// <summary>
        /// Modifies the existing valid user does not save with null changes supervisor.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithNullChangesSupervisor()
        {

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.Supervisor = null; //Change it

            Assert.AreNotEqual(null, userModelOriginal.User.Supervisor, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("Supervisor: may not be empty");
        }


        /// <summary>
        /// Modifies the existing valid user does not save with empty changes fund types.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithEmptyChangesFundTypes()
        {
            var emptyFundType = new List<FundType>();

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.FundTypes = emptyFundType; //Change it

            Assert.AreNotEqual(emptyFundType, userModelOriginal.User.FundTypes, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("FundTypes: may not be null or empty");
        }

        /// <summary>
        /// Modifies the existing valid user does not save with empty changes projects.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithEmptyChangesProjects()
        {
            var emptyProject = new List<Project>();

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            newUser.Projects = emptyProject; //Change it

            Assert.AreNotEqual(emptyProject, userModelOriginal.User.Projects, "Value was changed before we expected it to be.");

            var newUserModel = (ViewResult)Controller.Modify(newUser, CreateListOfRoles(), userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("Projects: may not be null or empty");
        }

        /// <summary>
        /// Modifies the existing valid user does not save with empty changes role list.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithEmptyChangesRoleList()
        {
            var emptyRoles = new List<string>();

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            var newUserModel = (ViewResult)Controller.Modify(newUser, emptyRoles, userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("User must have at least one role");
        }

        /// <summary>
        /// Modifies the existing valid user does not save with null changes role list.
        /// </summary>
        [TestMethod]
        public void ModifyExistingValidUserDoesNotSaveWithNullChangesRoleList()
        {
            //var emptyRoles = new List<string>();

            CreateUserViewModel userModelOriginal = CreateValidUserModel();
            CreateAndAttachProjectsToUser(userModelOriginal);
            CreateAndAttachFundTypesToUser(userModelOriginal, false);

            MockModifySpecificMethods(userModelOriginal);

            var newUser = CopySpecificUserFields(userModelOriginal);

            var newUserModel = (ViewResult)Controller.Modify(newUser, null, userModelOriginal.UserName);
            newUserModel.ViewData.ModelState.AssertErrorsAre("User must have at least one role");
        }

        #endregion Modify to invalid does not save and redirects back to the view

        #endregion Modify user tests

        #region Tests to ensure Mocking is working as expected. These could be removed.
        /// <summary>
        /// This demonstrates the mock of the CreateUser.
        /// </summary>
        [TestMethod]
        public void MockTest()
        {
            MembershipCreateStatus status;

            var mockGuid = Guid.NewGuid();
            UserBll = MockRepository.GenerateStub<IUserBLL>();
            UserBll.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBll.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();

            //If Repeat.any() isn't used, it will return the Guid only once, which means if you debug and inspect the value, it will be null the next time it is looked at.
            memberShipUser.Expect(a => a.ProviderUserKey).IgnoreArguments().Return(mockGuid).Repeat.Any();

            //If IgnoreArguments is not used, the params don't match and it isn't mocked.
            UserBll.UserAuth.MembershipService.Expect(a => a.CreateUser("Test", "dfgsdf345234", "Test@test.edu", "Q", "A", true,
                                                                null, out status)).IgnoreArguments().OutRef(status = MembershipCreateStatus.Success).Return(memberShipUser);


            MembershipCreateStatus testStatus;

            var testMemebershipUser = UserBll.UserAuth.MembershipService.CreateUser("1Test", "1dfgsdf345234",
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
            UserBll = MockRepository.GenerateStub<IUserBLL>();
            UserBll.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBll.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();

            //If Repeat.any() isn't used, it will return the Guid only once, which means if you debug and inspect the value, it will be null the next time it is looked at.
            memberShipUser.Expect(a => a.ProviderUserKey).IgnoreArguments().Return(mockGuid).Repeat.Any();

            //If IgnoreArguments is not used, the params don't match and it isn't mocked.
            UserBll.UserAuth.MembershipService.Expect(a => a.CreateUser("Test", "dfgsdf345234", "Test@test.edu", "Q", "A", true,
                                                                null, out status)).IgnoreArguments().OutRef(status = MembershipCreateStatus.Success).Return(memberShipUser);


            // ReSharper disable RedundantAssignment
            var testStatus = MembershipCreateStatus.UserRejected;  //Prime to a different value to make sure the OutRef works as expected.
            // ReSharper restore RedundantAssignment
            var testMemebershipUser = UserBll.UserAuth.MembershipService.CreateUser("1Test", "1dfgsdf345234",
                                                                                    "1Test@test.edu", "1Q", "A", true,
                                                                                    null, out testStatus);
            Assert.AreEqual(MembershipCreateStatus.Success, testStatus);
            Assert.IsNotNull(testMemebershipUser.ProviderUserKey);

            memberShipUser.Email = "test@test.edu";

            UserBll.UserAuth.MembershipService.Expect(a => a.GetUser("test")).Return(
                memberShipUser).Repeat.Any();

            var supervisorEmail = UserBll.UserAuth.MembershipService.GetUser("test").Email;
            Assert.AreEqual("test@test.edu", supervisorEmail);

        }
        #endregion Tests to ensure Mocking is working as expected. These could be removed.

        
        #region Helper Methods


        /// <summary>
        /// Creates the list of roles.
        /// </summary>
        /// <returns></returns>
        private static List<string> CreateListOfRoles()
        {
            var roleList = new List<string> {RoleNames.RoleSupervisor, RoleNames.RoleTimeSheet};

            return roleList;
        }

        /// <summary>
        /// Create a valid UserModel
        /// Default the wanted status to success.
        /// </summary>
        /// <returns></returns>
        private CreateUserViewModel CreateValidUserModel()
        {
            return CreateValidUserModel(MembershipCreateStatus.Success);
        }

        /// <summary>
        /// Create a valid UserModel
        /// </summary>
        /// <param name="wantedCreateStatus">Specify the status that should be returned for the out param</param>
        /// <returns></returns>
        private CreateUserViewModel CreateValidUserModel(MembershipCreateStatus wantedCreateStatus)
        {
            const string validValueName = "ValidName";
            const int validValueSalary = 1;
            const int validValueFte = 1;

            //FakeProjects();
            //FakeFundTypes();

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
                UserName = "UserName",
                Email = "test@test.edu"
            };

            var userId = Guid.NewGuid();
            newUser.SetUserID(userId);
            #endregion newUser            

            var userModel = new CreateUserViewModel
            {
                Question = "Q",
                Answer = "A",
                User = newUser,
                UserName = "ValidUserName",
                Email = "test@test.edu"
            };

            CreateAndAttachProjectsToUser(userModel);
            CreateAndAttachFundTypesToUser(userModel, false);

            MockMethods(userModel, wantedCreateStatus);
            MocksForUserLists(userModel);

            return userModel;
        }

        /// <summary>
        /// Create Projects, FundTypes, and a supervisor.
        /// Mock calls to these.
        /// </summary>
        /// <param name="userModel"></param>
        private void MocksForUserLists(UserViewModel userModel)
        {
            var supervisors = new List<User> {userModel.User.Supervisor};
            var projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project{Name = "Name2", IsActive = true}
                               };
            projects[0].SetIdTo(2);
            projects[1].SetIdTo(3);

            var fundTypes = new List<FundType>
                                {
                                    new FundType {Name = "Name1"},
                                    new FundType {Name = "Name2"},
                                    new FundType {Name = "Name3"}
                                };
            fundTypes[0].SetIdTo(4);
            fundTypes[1].SetIdTo(5);
            fundTypes[2].SetIdTo(6);

            UserBll.Expect(a => a.GetSupervisors()).Return(supervisors.AsQueryable());
            UserBll.Expect(a => a.GetAllProjectsByUser(null)).IgnoreArguments().Return(projects.AsQueryable());
            UserBll.Expect(a => a.GetAvailableFundTypes(null)).IgnoreArguments().Return(fundTypes.AsQueryable());
        }
       
        /// <summary>
        /// Mocks for the Create method.
        /// This needs the User and Supervisor to be populated in the userModel
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="wantedCreateStatus">The status wanted for the Create mock</param>
        private void MockMethods(CreateUserViewModel userModel, MembershipCreateStatus wantedCreateStatus)
        {
            UserBll.UserAuth = MockRepository.GenerateStub<IUserAuth>();
            UserBll.UserAuth.MembershipService = MockRepository.GenerateStub<IMembershipService>();

            MembershipCreateStatus createStatus;
            var mockGuid = Guid.NewGuid();                                    
            var memberShipUser = MockRepository.GenerateStub<MembershipUser>();

            #region Mocks for the supervisor            
            //We don't need to ignore arguments for this one because we are only doing it for a specific Supervior ID.
            UserBll.UserAuth.MembershipService.Expect(a => a.GetUser(userModel.User.Supervisor.Id)).Return(memberShipUser).Repeat.Any();
            memberShipUser.Email = "test@test.edu"; //Email for the Supervisor. If we need a different email for a user, this would need to be changed.

            UserBll.Expect(a => a.GetByID(userModel.User.Supervisor.Id)).Return(userModel.User.Supervisor).Repeat.Any();
            #endregion Mocks for the supervisor

            #region Mocks for the Create method
            //If IgnoreArguments is not used, the params don't match and it isn't mocked.
            UserBll.UserAuth.MembershipService.Expect(a => a.CreateUser(userModel.UserName, "jaskidjflkajsdlf$#12", userModel.Email, userModel.Question, userModel.Answer, true,
                                                                null, out createStatus)).OutRef(createStatus = wantedCreateStatus).Return(memberShipUser);
            //If Repeat.any() isn't used, it will return the Guid only once, which means if you debug and inspect the value, it will be null the next time it is looked at.
            memberShipUser.Expect(a => a.ProviderUserKey).IgnoreArguments().Return(mockGuid).Repeat.Any();

            #endregion Mocks for the Create method
            
            #region Mocks for the URL methods (In Create)
            Controller.Url = MockRepository.GenerateStub<UrlHelper>(Controller.ControllerContext.RequestContext);

            Controller.Url.RequestContext.HttpContext.Request.Expect(a => a.Url).Return(new Uri("http://sample.com")).
                            Repeat.Any();
            
            //Mock the GetSubordinates so it doesn't require this user to gave the supervisor role.
            //TODO: Change this with a parameter to test having subordinates
            var emptyList = new List<User>().AsQueryable();            
            UserBll.Expect(a => a.GetSubordinates(userModel.User)).Return(emptyList).Repeat.Any();            
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
        /// Creates the fund types and attaches them to user.
        /// </summary>
        /// <param name="userModel">The user model.</param>
        /// <param name="createStateFundTypes">if set to <c>true</c> [create state fund types].</param>
        private static void CreateAndAttachFundTypesToUser(UserViewModel userModel, bool createStateFundTypes)
        {
            var fundTypes = new List<FundType>();
            if (createStateFundTypes)
            {
                fundTypes.Add(new FundType { Name = "Name1" });
                fundTypes.Add(new FundType { Name = "State Share" });

                fundTypes[0].SetIdTo(4);
                fundTypes[1].SetIdTo(5);
            }
            else
            {
                fundTypes.Add(new FundType {Name = "Name1"});
                fundTypes.Add(new FundType {Name = "Name2"});
                fundTypes.Add(new FundType {Name = "Name3"});

                fundTypes[0].SetIdTo(4);
                fundTypes[1].SetIdTo(5);
                fundTypes[2].SetIdTo(6);
            }

            userModel.FundTypes = fundTypes;
            userModel.User.FundTypes = fundTypes;
        }

        /// <summary>
        /// Creates the projects and attachs them to user.
        /// </summary>
        /// <param name="userModel">The user model.</param>
        private static void CreateAndAttachProjectsToUser(UserViewModel userModel)
        {
            var projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project{Name = "Name2", IsActive = true}
                               };
            projects[0].SetIdTo(2);
            projects[1].SetIdTo(3);
            userModel.Projects = projects; //Is this what the view uses to display projects?
            userModel.User.Projects = projects; //These would be the ones selected?
        }

        /// <summary>
        /// Mocks the specific methods modify unit tests need.
        /// </summary>
        /// <param name="userModelOriginal">The user model original.</param>
        private void MockModifySpecificMethods(CreateUserViewModel userModelOriginal)
        {
            UserBll.Expect(a => a.GetUser(userModelOriginal.UserName)).Return(userModelOriginal.User);
        }


        /// <summary>
        /// Copies the specific user fields.
        /// </summary>
        /// <param name="userModelOriginal">The user model original.</param>
        /// <returns></returns>
        private static User CopySpecificUserFields(UserViewModel userModelOriginal)
        {
            return new User
            {
                FirstName = userModelOriginal.User.FirstName,
                LastName = userModelOriginal.User.LastName,
                Salary = userModelOriginal.User.Salary,
                FTE = userModelOriginal.User.FTE,
                BenefitRate = userModelOriginal.User.BenefitRate,
                IsActive = userModelOriginal.User.IsActive,
                Supervisor = userModelOriginal.User.Supervisor,
                FundTypes = userModelOriginal.User.FundTypes,
                Projects = userModelOriginal.User.Projects
            };

        }
        #endregion Helper Methods
    }
}
