using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
using FSNEP.Tests.Core.Extensions;
using FSNEP.Tests.Core.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FSNEP.Controllers;
using MvcContrib.TestHelper;
using Rhino.Mocks;

namespace FSNEP.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest : ControllerTestBase<AccountController>
    {
        protected override void SetupController()
        {
            IFormsAuthentication formsAuth = new MockFormsAuthenticationService();

            IMembershipService membershipService = new AccountMembershipService(new MockMembershipProvider());

            var messageGateway = MockRepository.GenerateStub<IMessageGateway>();

            CreateController(formsAuth, membershipService, messageGateway);
        }

        [TestMethod]
        public void ForgotPasswordGetReturnsView()
        {
            Controller.ForgotPassword()
                .AssertViewRendered();
        }

        [TestMethod]
        public void ForgotPasswordPostRedirectsToResetPasswordActionOnValidUser()
        {
            Controller.ForgotPassword("validUser")
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ResetPassword("validUser"));
        }

        [TestMethod]
        public void ForgotPasswordPostShowsErrorMessageForInvalidUser()
        {
            Controller.ForgotPassword("invalidUser")
                .AssertViewRendered()
                .ViewData["Message"]
                .ShouldBe("User information not found.");
        }

        [TestMethod]
        public void ResetPasswordWithUsernameMapsToProperController()
        {
            "~/Account/ResetPassword/validUser"
                .ShouldMapTo<AccountController>(a => a.ResetPassword("validUser"));
        }

        [TestMethod]
        public void ResetPasswordGetRedirectsToForgotPasswordWhenUsernameInvalid()
        {
            Controller.ResetPassword("invalidUser")
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ForgotPassword());
        }

        [TestMethod]
        public void ResetPasswordGetPopulatesPasswordQuestionWhenUsernameIsValid()
        {
            Controller.ResetPassword("validUser")
                .AssertViewRendered()
                .ViewData["PasswordQuestion"]
                .ShouldBe("Question");
        }

        [TestMethod]
        public void ResetPasswordPostGivesErrorMessageWhenAnswerIsIncorrect()
        {
            Controller.ResetPassword("validUser", "invalidAnswer")
                .AssertViewRendered()
                .ViewData["Message"]
                .ShouldBe("Your answer could not be verified. Please try again.");
        }

        [TestMethod]
        public void ResetPasswordPostGivesErrorMessageWhenAnswerIsBlank()
        {
            Controller.ResetPassword("validUser", "")
                .AssertViewRendered()
                .ViewData["Message"]
                .ShouldBe("Your answer can not be blank.  Please try again.");
        }

        [TestMethod]
        public void ResetPasswordPostRedirectsToPasswordSuccessPageWhenAnswerIsValid()
        {
            var controller = Controller;
            controller.MessageService = new MockMessageGateway();

            controller.ResetPassword("validUser", "validAnswer")
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.ResetPasswordSuccess());
        }

        [TestMethod]
        public void ResetPasswordPostSendsEmailWhenAnswerIsValid()
        {
            //Setup a mock messageGateway
            var messageGateway = MockRepository.GenerateMock<IMessageGateway>();

            //Get the account controller
            var controller = Controller;
            controller.MessageService = messageGateway;

            controller.ResetPassword("validUser", "validAnswer");

            messageGateway.AssertWasCalled(a=>a.SendMessage("","",""), a=>a.IgnoreArguments()); //Verify the message gateway was called
        }

        [TestMethod]
        public void ChangePasswordGetReturnsView()
        {
            Controller.ChangePassword()
                .AssertViewRendered()
                .ViewData["PasswordLength"]
                .ShouldBe(6);
        }

        /*
        [TestMethod]
        public void ChangePasswordPostRedirectsOnSuccess()
        {
            GetAccountController().ChangePassword("oldPass", "newPass", "newPass")
                .AssertActionRedirect()
                .ToAction("ChangePasswordSuccess");
        }
         */

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfCurrentPasswordNotSpecified()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.ChangePassword("", "newPassword", "newPassword");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a current password.", result.ViewData.ModelState["currentPassword"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfNewPasswordDoesNotMatchConfirmPassword()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.ChangePassword("currentPassword", "newPassword", "otherPassword");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("The new password and confirmation password do not match.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfNewPasswordIsNull()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.ChangePassword("currentPassword", null, null);

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a new password of 6 or more characters.", result.ViewData.ModelState["newPassword"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfNewPasswordIsTooShort()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.ChangePassword("currentPassword", "12345", "12345");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("You must specify a new password of 6 or more characters.", result.ViewData.ModelState["newPassword"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordPostReturnsViewIfProviderRejectsPassword()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.ChangePassword("oldPass", "badPass", "badPass");

            // Assert
            Assert.AreEqual(6, result.ViewData["PasswordLength"]);
            Assert.AreEqual("The current password is incorrect or the new password is invalid.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ChangePasswordSuccess()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.ChangePasswordSuccess();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginGet()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.LogOn();

            // Assert
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Routing LogOn Calls LogOn
        /// </summary>
        [TestMethod]
        public void RoutingLogOnCallsLogOn()
        {
            "~/Account/LogOn"
                .ShouldMapTo<AccountController>(a => a.LogOn());
        }

        /// <summary>
        /// Routing LogOff Calls LogOff
        /// </summary>
        [TestMethod]
        public void RoutingLogOffCallsLogOff()
        {
            "~/Account/LogOff"
                .ShouldMapTo<AccountController>(a => a.LogOff());
        }

        /// <summary>
        /// Routing ForgotPassoword Calls ForgotPassword
        /// </summary>
        [TestMethod]
        public void RoutingForgotPassowordCallsForgotPassword()
        {
            "~/Account/ForgotPassword"
                .ShouldMapTo<AccountController>(a => a.ForgotPassword());
        }

        /// <summary>
        /// Routing ChangePassword Calls ChangePassword
        /// </summary>
        [TestMethod]
        public void RoutingChangePasswordCallsChangePassword()
        {
            "~/Account/ChangePassword"
                .ShouldMapTo<AccountController>(a => a.ChangePassword());
        }

        /// <summary>
        /// Routing ChangePasswordSuccess Calls ChangePasswordSuccess
        /// </summary>
        [TestMethod]
        public void RoutingChangePasswordSuccessCallsChangePasswordSuccess()
        {
            "~/Account/ChangePasswordSuccess"
                .ShouldMapTo<AccountController>(a => a.ChangePasswordSuccess());
        }

        [TestMethod]
        public void LoginPostRedirectsHomeIfLoginSuccessfulButNoReturnUrlGiven()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (RedirectToRouteResult)controller.LogOn("someUser", "goodPass", true, null);

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void LoginPostRedirectsToReturnUrlIfLoginSuccessfulAndReturnUrlGiven()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (RedirectResult)controller.LogOn("someUser", "goodPass", false, "someUrl");

            // Assert
            Assert.AreEqual("someUrl", result.Url);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfPasswordNotSpecified()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.LogOn("username", "", true, null);

            // Assert
            Assert.AreEqual("You must specify a password.", result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfUsernameNotSpecified()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.LogOn("", "somePass", false, null);

            // Assert
            Assert.AreEqual("You must specify a username.", result.ViewData.ModelState["username"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfUsernameOrPasswordIsIncorrect()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (ViewResult)controller.LogOn("someUser", "badPass", true, null);

            // Assert
            Assert.AreEqual("The username or password provided is incorrect.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LogOff()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            var result = (RedirectToRouteResult)controller.LogOff();

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        #region New User Tests

        /// <summary>
        /// A new user returns view.
        /// </summary>
        [TestMethod]
        public void NewUserGetReturnsView()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);           

            Controller.NewUser(token).AssertViewRendered();
        }

        [TestMethod]
        public void NewUserGetReturnsViewAndNullsOutToken()
        {
            #region create a valid user
            var token = Guid.NewGuid();
            var user = new User
                {
                    FirstName = "FName",
                    LastName = "LName",
                    Salary = 1,
                    BenefitRate = 2,
                    FTE = 1,
                    IsActive = true,
                    UserName = "UserName"
                };
            user.Supervisor = user; //I'm my own supervisor
            user.Projects = new List<Project>
                           {
                               new Project {Name = "Name", IsActive = true},
                               new Project{Name = "Name2", IsActive = true}
                           };
            user.FundTypes = new List<FundType>
                            {
                                new FundType {Name = "Name1"},
                                new FundType {Name = "Name2"}
                            };

            var userId = Guid.NewGuid();

            user.Token = Guid.NewGuid();
            user.SetUserID(userId);
      
            user.Token = token; //Set 1 to the passed value.
            #endregion create a valid user

            Assert.IsNotNull(user.Token);

            IQueryable<User> userList = new[]{user}.AsQueryable();
            var fakeUserRepository = FakeRepository<User>();
            fakeUserRepository.Expect(a => a.Queryable).Return(userList);
            Controller.Repository.Expect(a => a.OfType<User>()).Return(fakeUserRepository).Repeat.Any();

            Controller.NewUser(token, CreateValidUserViewModel())
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.LogOn());

            Assert.AreEqual(userId, user.Id);
            Assert.IsNull(user.Token);
        }

        /// <summary>
        /// A New user with invalid GUID redirects to Home.
        /// </summary>
        [TestMethod]
        public void NewUserWithInvalidGuidRedirectsToHome()
        {
            var token = Guid.NewGuid();
            var notFoundtoken = Guid.NewGuid();
            FakeUserForNewUserTests(token); 
            // Act
            var result = (RedirectToRouteResult)Controller.NewUser(notFoundtoken);

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Error", result.RouteValues["action"]);
        }

        /// <summary>
        /// New User saves valid new user.
        /// </summary>
        [TestMethod]
        public void NewUserSavesValidNewUser()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();

            

            Controller.NewUser(token, newUserViewModel)
                .AssertActionRedirect()
                .ToAction<AccountController>(a => a.LogOn());
            
            //TODO: In the repository test base, ensure that this nulls out the token.
        }

        #region Password Validation Tests
        /// <summary>
        /// New user does not save with null password.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithNullPassword()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Password = null;

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Password: may not be null or empty",
                "ConfirmPassword does not match Password");            
        }

        /// <summary>
        /// New user does not save with Empty password.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithEmptyPassword()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Password = "";
            newUserViewModel.ConfirmPassword = "";

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Password: may not be null or empty");
        }

        /// <summary>
        /// New user does not save with only spaces in password.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithOnlySpacesInPassword()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Password = " ";
            newUserViewModel.ConfirmPassword = " ";

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Password: may not be null or empty");
        }

        /// <summary>
        /// New user does not save with password more than 128 characters long.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithOnlyTooLongPassword()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Password = "123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789"; //129 characters long
            newUserViewModel.ConfirmPassword = newUserViewModel.Password;

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Password: length must be between 0 and 128");
        }

        /// <summary>
        /// New user does not save with confirm password different from password.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithConfirmPasswordDifferentFromPassword()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Password = "ABC";
            newUserViewModel.ConfirmPassword = "abc";

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("ConfirmPassword does not match Password");
        }
        #endregion Password Validation Tests

        #region Question Validation Tests
        /// <summary>
        /// New user does not save with null question.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithNullQuestion()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Question = null;

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Question: may not be null or empty");
        }

        /// <summary>
        /// New user does not save with empty question.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithEmptyQuestion()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Question = "";

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Question: may not be null or empty");
        }

        /// <summary>
        /// New user does not save with spaces only in question.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithSpacesOnlyInQuestion()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Question = " ";

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Question: may not be null or empty");
        }
        #endregion Question Validation Tests

        #region Answer Validation Tests
        /// <summary>
        /// New user does not save with null Answer.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithNullAnswer()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Answer = null;

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Answer: may not be null or empty");
        }

        /// <summary>
        /// New user does not save with empty Answer.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithEmptyAnswer()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Answer = string.Empty;

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Answer: may not be null or empty");
        }

        /// <summary>
        /// New user does not save with spaces only in question.
        /// </summary>
        [TestMethod]
        public void NewUserDoesNotSaveWithSpacesOnlyInAnswer()
        {
            var token = Guid.NewGuid();
            FakeUserForNewUserTests(token);

            NewUserViewModel newUserViewModel = CreateValidUserViewModel();
            newUserViewModel.Answer = " ";

            var result = (ViewResult)Controller.NewUser(token, newUserViewModel);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            result.ViewData.ModelState.AssertErrorsAre("Answer: may not be null or empty");
        }
        #endregion Answer Validation Tests


        #region Helper Methods
        /// <summary>
        /// Creates the valid user view model.
        /// </summary>
        /// <returns></returns>
        private static NewUserViewModel CreateValidUserViewModel()
        {
            return new NewUserViewModel
            {
                Password = "123",
                ConfirmPassword = "123",
                Question = "Q",
                Answer = "A"
            };
        }

        /// <summary>
        /// Fakes the user for new user tests.
        /// </summary>
        /// <param name="token">The token.</param>
        private void FakeUserForNewUserTests(Guid token)
        {
            var users = new User[3];
            for (int i = 0; i < 3; i++)
            {
                users[i] = new User
                {
                    FirstName = "FName" + i + 1,
                    LastName = "LName" + i + 1,
                    Salary = 1,
                    BenefitRate = 2,
                    FTE = 1,
                    IsActive = true,
                    UserName = "UserName" + i + 1
                };
                users[i].Supervisor = users[i]; //I'm my own supervisor
                users[i].Projects = new List<Project>
                               {
                                   new Project {Name = "Name", IsActive = true},
                                   new Project{Name = "Name2", IsActive = true}
                               };
                users[i].FundTypes = new List<FundType>
                                {
                                    new FundType {Name = "Name1"},
                                    new FundType {Name = "Name2"}
                                };

                var userId = Guid.NewGuid();

                users[i].Token = Guid.NewGuid();
                users[i].SetUserID(userId);
            }

            users[1].Token = token; //Set 1 to the passed value.

            IQueryable<User> userList = new[]
                                            {
                                                users[0],
                                                users[1],
                                                users[2]
                                            }.AsQueryable();


            var fakeUserRepository = FakeRepository<User>();
            fakeUserRepository.Expect(a => a.Queryable).Return(userList);
            Controller.Repository.Expect(a => a.OfType<User>()).Return(fakeUserRepository).Repeat.Any();

        }
        #endregion Helper Methods

        #endregion NewUser Tests

        public class MockMessageGateway : IMessageGateway
        {
            public void SendMessage(string to, string subject, string body)
            {
            }

            public void SendMessageToNewUser(User user, string username, string userEmail, string supervisorEmail, string newUserTokenPath)
            {
            }

            public void SendReviewMessage(Record record, bool approved)
            {
                
            }
        }

        public class MockFormsAuthenticationService : IFormsAuthentication
        {
            public void SignIn(string userName, bool createPersistentCookie)
            {
            }

            public void SignOut()
            {
            }
        }

        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "someUser";
                }
            }
        }

        public class MockPrincipal : IPrincipal
        {
            IIdentity identity;

            public IIdentity Identity
            {
                get
                {
                    if (identity == null)
                    {
                        identity = new MockIdentity();
                    }
                    return identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal user;

            public override IPrincipal User
            {
                get
                {
                    if (user == null)
                    {
                        user = new MockPrincipal();
                    }
                    return user;
                }
                set
                {
                    user = value;
                }
            }
        }

        public class MockMembershipProvider : MembershipProvider
        {
            public override string ApplicationName { get; set; }

            public override bool EnablePasswordReset
            {
                get
                {
                    return false;
                }
            }

            public override bool EnablePasswordRetrieval
            {
                get
                {
                    return false;
                }
            }

            public override int MaxInvalidPasswordAttempts
            {
                get
                {
                    return 0;
                }
            }

            public override int MinRequiredNonAlphanumericCharacters
            {
                get
                {
                    return 0;
                }
            }

            public override int MinRequiredPasswordLength
            {
                get
                {
                    return 6;
                }
            }

            public override string Name
            {
                get
                {
                    return null;
                }
            }

            public override int PasswordAttemptWindow
            {
                get
                {
                    return 3;
                }
            }

            public override MembershipPasswordFormat PasswordFormat
            {
                get
                {
                    return MembershipPasswordFormat.Clear;
                }
            }

            public override string PasswordStrengthRegularExpression
            {
                get
                {
                    return null;
                }
            }

            public override bool RequiresQuestionAndAnswer
            {
                get
                {
                    return false;
                }
            }

            public override bool RequiresUniqueEmail
            {
                get
                {
                    return false;
                }
            }

            public override bool ChangePassword(string username, string oldPassword, string newPassword)
            {
                return true; //Need for NewUser
            }

            public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
            {
                return true; //Need for NewUser
            }

            public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, Object providerUserKey, out MembershipCreateStatus status)
            {
                var user = new FakeMembershipUser();

                if (username.Equals("someUser") && password.Equals("goodPass") && email.Equals("email"))
                {
                    status = MembershipCreateStatus.Success;
                }
                else
                {
                    // the 'email' parameter contains the status we want to return to the user
                    status = (MembershipCreateStatus)Enum.Parse(typeof(MembershipCreateStatus), email);
                }

                return user;
            }

            public override bool DeleteUser(string username, bool deleteAllRelatedData)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override int GetNumberOfUsersOnline()
            {
                throw new NotImplementedException();
            }

            public override string GetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override string GetUserNameByEmail(string email)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(Object providerUserKey, bool userIsOnline)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(string username, bool userIsOnline)
            {
                return username == "invalidUser" ? null : new FakeMembershipUser();
            }

            public override string ResetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override bool UnlockUser(string userName)
            {
                throw new NotImplementedException();
            }

            public override void UpdateUser(MembershipUser user)
            {
                throw new NotImplementedException();
            }

            public override bool ValidateUser(string username, string password)
            {
                return password.Equals("goodPass");
            }

        }
    }
}
