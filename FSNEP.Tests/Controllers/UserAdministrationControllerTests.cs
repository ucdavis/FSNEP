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

        #region Redirect Tests
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
        #endregion Redirect Tests

        #region Routing Tests
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
        #endregion Routing Tests

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

            MockMethods(userModel, MembershipCreateStatus.Success);
            
            //Call the method that the UI would use to create the new user.
            Controller.Create(userModel, supervisor.ID, projectList, fundTypeList, roleList)
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Creates a user with a valid first name of spaces.
        /// User is saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserSavesWithSpacesOnlyInFirstName()
        {
            const string invalidValueName = "  ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FirstName = invalidValueName;

            Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Creates a user with a invalid first name of 51 characters.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithTooLongFirstName()
        {
            const string invalidValueName = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FirstName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("FirstName: The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).", newUserModel.ViewData.ModelState["User.FirstName"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }        

        /// <summary>
        /// Creates a user with a invalid first name of null.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullFirstName()
        {
            const string invalidValueName = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FirstName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());            
            Assert.AreEqual("FirstName: The value cannot be null.", newUserModel.ViewData.ModelState["User.FirstName"].Errors[0].ErrorMessage);
            Assert.AreEqual("FirstName: The length of the value must fall within the range \"0\" (Ignore) - \"50\" (Inclusive).", newUserModel.ViewData.ModelState["User.FirstName"].Errors[1].ErrorMessage);
            Assert.AreEqual(2, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }        
        #endregion First Name Validation Tests

        #region Last Name Validation Tests
        /// <summary>
        /// Creates a user with a invalid last name of 51 characters.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithTooLongLastName()
        {
            const string invalidValueName = "123456789 123456789 123456789 123456789 12345678901";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.LastName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());            
            Assert.AreEqual("LastName: The length of the value must fall within the range \"1\" (Inclusive) - \"50\" (Inclusive).", newUserModel.ViewData.ModelState["User.LastName"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a user with a invalid last name of null.
        /// User is not saved.
        /// Ensures the corerct message is displayed. The required validation is hit before the length validation.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullLastName()
        {
            const string invalidValueName = null;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.LastName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("LastName: Required", newUserModel.ViewData.ModelState["User.LastName"].Errors[0].ErrorMessage);
            Assert.AreEqual("LastName: The length of the value must fall within the range \"1\" (Inclusive) - \"50\" (Inclusive).", newUserModel.ViewData.ModelState["User.LastName"].Errors[1].ErrorMessage);
            Assert.AreEqual(2, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a user with a invalid last name spaces only.
        /// User is not saved.
        /// Ensures the corerct message is displayed. The Required Validator checks for null and spaces only.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSpacesOnlyInLastName()
        {
            const string invalidValueName = "  ";
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.LastName = invalidValueName;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("LastName: Required", newUserModel.ViewData.ModelState["User.LastName"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        #endregion Last Name Validation Tests

        #region Other Validation Tests
        /// <summary>
        /// Creates a user with a null supervisorId.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullSupervisorId()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            var newUserModel = (ViewResult)Controller.Create(userModel, null, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("You must select a supervisor", newUserModel.ViewData.ModelState["SupervisorID"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a user with a null Project List (The User didn't select one from the list).
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullProjectList()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, null,
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("You must select at least one project", newUserModel.ViewData.ModelState["ProjectList"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a user with a null Fund Type List (The User didn't select one from the list).
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullFundTypeList()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      null, CreateListOfRoles());
            Assert.AreEqual("You must select at least one fund type", newUserModel.ViewData.ModelState["FundTypeList"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a user with a null Role List (The User didn't select one from the list).
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithNullRoleList()
        {
            CreateUserViewModel userModel = CreateValidUserModel();

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), null);
            Assert.AreEqual("User must have at least one role", newUserModel.ViewData.ModelState["RoleList"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a duplicate user name.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithDuplicateUser()
        {          
            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.DuplicateUserName);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Username already exists", newUserModel.ViewData.ModelState["Username"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        #endregion Other Validation Tests

        #region MembershipCreateStatus tests that still need to be done. These will fail when code is added
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorDuplicateEmail()
        {
            
                CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.DuplicateEmail);

                var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                          CreateListOfFundTypes(), CreateListOfRoles());
                Assert.AreEqual("Create Failed Duplicate Email", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
                Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");                        
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorDuplicateProviderUserKey()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.DuplicateProviderUserKey);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Duplicate Provider User Key", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidAnswer()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidAnswer);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Invalid Answer", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidEmail()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidEmail);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Invalid Email", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidPassword()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidPassword);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Invalid Password", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidProviderUserKey()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidProviderUserKey);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Invalid Provider User Key", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidQuestion()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidQuestion);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Invalid Question", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorInvalidUserName()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.InvalidUserName);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Invalid User Name", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorProviderError()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.ProviderError);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed Provider Error", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a valid user, but mock the create to fail because of a Create Status Error.
        /// User is not saved.
        /// Ensures the corerct message is displayed. (Note, we would expect this to fail when the code is updated)
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithCreateStatusErrorUserRejected()
        {

            CreateUserViewModel userModel = CreateValidUserModel(MembershipCreateStatus.UserRejected);

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(),
                                      CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Create Failed User Rejected", newUserModel.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        #endregion MembershipCreateStatus tests that still need to be done. These will fail when code is added

        #region Salary Validation Tests
        /// <summary>
        /// Creates a user with a invalid salary .
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSalaryLessThanZero()
        {
            const double invalidValueSalary = -1.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.Salary = invalidValueSalary;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Salary: Must be greater than zero", newUserModel.ViewData.ModelState["User.Salary"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a user with a invalid salary .
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithSalaryOfZero()
        {
            const double invalidValueSalary = 0.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.Salary = invalidValueSalary;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("Salary: Must be greater than zero", newUserModel.ViewData.ModelState["User.Salary"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        #endregion Salary Validation Tests

        #region Benefit Rate Valadation Tests
        /// <summary>
        /// Creates a user with a invalid Benefit Rate .
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithBenefitRateLessThanZero()
        {
            const double invalidValueBenefitRate = -1.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = invalidValueBenefitRate;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("BenefitRate: The value must fall within the range \"0\" (Inclusive) - \"2\" (Inclusive).", newUserModel.ViewData.ModelState["User.BenefitRate"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a user with a invalid Benefit Rate .
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithBenefitRateGreaterThanTwo()
        {
            const double invalidValueBenefitRate = 2.000001;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = invalidValueBenefitRate;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("BenefitRate: The value must fall within the range \"0\" (Inclusive) - \"2\" (Inclusive).", newUserModel.ViewData.ModelState["User.BenefitRate"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }

        /// <summary>
        /// Creates a user with a valid Benefit Rate of zero.
        /// User is saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserSavesWithBenefitRateOfZero()
        {
            const double validValueBenefitRate = 0;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = validValueBenefitRate;

            Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
        }

        /// <summary>
        /// Creates a user with a valid Benefit Rate of two.
        /// User is saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserSavesWithBenefitRateOfTwo()
        {
            const double validValueBenefitRate = 2.000;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.BenefitRate = validValueBenefitRate;

            Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles())
                .AssertActionRedirect()
                .ToAction<HomeController>(a => a.Index());
        }
        #endregion Benefit Rate Valadation Tests

        #region FTE Validation Tests
        /// <summary>
        /// Creates a user with a invalid FTE.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithFteLessThanZero()
        {
            const double invalidValueFte = -1.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FTE = invalidValueFte;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("FTE: The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).", newUserModel.ViewData.ModelState["User.FTE"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a user with a invalid FTE.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithFteOfZero()
        {
            const double invalidValueFte = 0.00;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FTE = invalidValueFte;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("FTE: The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).", newUserModel.ViewData.ModelState["User.FTE"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        /// <summary>
        /// Creates a user with a invalid FTE.
        /// User is not saved.
        /// Ensures the corerct message is displayed.
        /// </summary>
        [TestMethod]
        public void CreateUserDoesNotSaveWithFteGreaterThanOne()
        {
            const double invalidValueFte = 1.0001;
            CreateUserViewModel userModel = CreateValidUserModel();
            userModel.User.FTE = invalidValueFte;

            var newUserModel = (ViewResult)Controller.Create(userModel, userModel.User.Supervisor.ID, CreateListOfProjects(), CreateListOfFundTypes(), CreateListOfRoles());
            Assert.AreEqual("FTE: The value must fall within the range \"0\" (Exclusive) - \"1\" (Inclusive).", newUserModel.ViewData.ModelState["User.FTE"].Errors[0].ErrorMessage);
            Assert.AreEqual(1, CountErrorMessages(newUserModel.ViewData.ModelState), "Wrong number of error messages.");
        }
        #endregion FTE Validation Tests

        #region Tests to ensure Mocking is working as expected. These could be removed.
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
        #endregion Tests to ensure Mocking is working as expected. These could be removed.

        #region Helper Methods
        private static List<int> CreateListOfProjects()
        {
            var projectList = new List<int> {2, 3};

            return projectList;
        }
        private static List<int> CreateListOfFundTypes()
        {
            var fundTypeList = new List<int> {4, 5};


            return fundTypeList;
        }
        private static List<string> CreateListOfRoles()
        {
            var roleList = new List<string> {"Supervisor", "Timesheet User"};

            return roleList;
        }

        private CreateUserViewModel CreateValidUserModel()
        {
            return CreateValidUserModel(MembershipCreateStatus.Success);
        }

        private CreateUserViewModel CreateValidUserModel(MembershipCreateStatus wantedCreateStatus)
        {
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

            var userModel = new CreateUserViewModel
            {
                Question = "Q",
                Answer = "A",
                User = newUser,
                UserName = "ValidUserName",
                Email = "test@test.edu"
            };

            MockMethods(userModel, wantedCreateStatus);
            MocksForFailure(userModel);

            return userModel;
        }


        private void MocksForFailure(CreateUserViewModel userModel)
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

            UserBLL.Expect(a => a.GetSupervisors()).Return(supervisors.AsQueryable());
            UserBLL.Expect(a => a.GetAllProjectsByUser()).Return(projects.AsQueryable());
            UserBLL.Expect(a => a.GetAvailableFundTypes()).Return(fundTypes.AsQueryable());
        }
       
        /// <summary>
        /// Mocks for the Create method.
        /// This needs the User and Supervisor to be populated in the userModel
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="wantedCreateStatus">The status wanted for the Create mock</param>
        private void MockMethods(CreateUserViewModel userModel, MembershipCreateStatus wantedCreateStatus)
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
                                                                null, out createStatus)).OutRef(createStatus = wantedCreateStatus).Return(memberShipUser);
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
        #endregion Helper Methods
    }
}
