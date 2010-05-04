using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using FSNEP.Tests.Core;
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
            ViewResult result = (ViewResult)controller.ChangePassword("currentPassword", "newPassword", "otherPassword");

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
            ViewResult result = (ViewResult)controller.ChangePassword("currentPassword", null, null);

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
            ViewResult result = (ViewResult)controller.ChangePassword("currentPassword", "12345", "12345");

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
            ViewResult result = (ViewResult)controller.ChangePassword("oldPass", "badPass", "badPass");

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
            ViewResult result = (ViewResult)controller.ChangePasswordSuccess();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginGet()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            ViewResult result = (ViewResult)controller.LogOn();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginPostRedirectsHomeIfLoginSuccessfulButNoReturnUrlGiven()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            RedirectToRouteResult result = (RedirectToRouteResult)controller.LogOn("someUser", "goodPass", true, null);

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
            RedirectResult result = (RedirectResult)controller.LogOn("someUser", "goodPass", false, "someUrl");

            // Assert
            Assert.AreEqual("someUrl", result.Url);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfPasswordNotSpecified()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            ViewResult result = (ViewResult)controller.LogOn("username", "", true, null);

            // Assert
            Assert.AreEqual("You must specify a password.", result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfUsernameNotSpecified()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            ViewResult result = (ViewResult)controller.LogOn("", "somePass", false, null);

            // Assert
            Assert.AreEqual("You must specify a username.", result.ViewData.ModelState["username"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LoginPostReturnsViewIfUsernameOrPasswordIsIncorrect()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            ViewResult result = (ViewResult)controller.LogOn("someUser", "badPass", true, null);

            // Assert
            Assert.AreEqual("The username or password provided is incorrect.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void LogOff()
        {
            // Arrange
            AccountController controller = Controller;

            // Act
            RedirectToRouteResult result = (RedirectToRouteResult)controller.LogOff();

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
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
                throw new NotImplementedException();
            }

            public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
            {
                throw new NotImplementedException();
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
