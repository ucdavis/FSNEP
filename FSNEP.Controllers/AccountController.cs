using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using FSNEP.Core.Abstractions;
using FSNEP.Core.Domain;
using MvcContrib.Attributes;
using MvcContrib;
using UCDArch.Core.Utils;
using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;
using UCDArch.Web.Validator;
using FSNEP.Controllers.Helpers;
using UCDArch.Web.Attributes;

namespace FSNEP.Controllers
{
    public class AccountController : ApplicationController
    {
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service, IMessageGateway messageGateway)
        {
            Check.Require(formsAuth != null, "Forms Authentication Service required");
            Check.Require(service != null, "Membership Service required");
            Check.Require(messageGateway != null, "Message Gateway required");
            
            FormsAuth = formsAuth;
            MembershipService = service;
            MessageService = messageGateway;
        }

        private IFormsAuthentication FormsAuth
        {
            get; set;
        }

        private IMembershipService MembershipService
        {
            get; set;
        }

        public IMessageGateway MessageService
        {
            get;
            set;
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [BypassAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {
            userName = userName.ToLower();

            if (!ValidateLogOn(userName, password))
            {
                return View();
            }

            FormsAuth.SignIn(userName, rememberMe);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult LogOff()
        {

            FormsAuth.SignOut();

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Allows a new user to change their password and question/answer given a valid token
        /// </summary>
        public ActionResult NewUser(Guid? id)
        {
            var user = Repository.OfType<User>().Queryable.Where(u => u.Token == id).SingleOrDefault();

            if (user == null) return RedirectToAction("Error", "Home");

            var viewModel = NewUserViewModel.Create();
            viewModel.User = user;

            return View(viewModel);
        }

        /// <summary>
        /// Allows a new user to change their password and question/answer given a valid token
        /// </summary>
        [AcceptPost]
        [Transaction]
        [BypassAntiForgeryToken]
        public ActionResult NewUser(Guid? id, NewUserViewModel viewModel)
        {
            var user = Repository.OfType<User>().Queryable.Where(u => u.Token == id).SingleOrDefault();

            if (user == null) return RedirectToAction("Error", "Home");

            var validationResults = Validation.GetValidationResultsFor(viewModel);            

            MvcValidationAdapter.TransferValidationMessagesTo(ModelState, validationResults);

            if (viewModel.Password != viewModel.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "ConfirmPassword does not match Password");
            }

            if (!ModelState.IsValid)
            {
                viewModel.User = user;

                return View(viewModel);
            }
            else
            {
                //Set the user's attributes
                bool success;

                try
                {
                    success = MembershipService.ChangePassword(user.UserName, Constants.STR_Default_Pass, viewModel.Password);
                    success |= MembershipService.ChangePasswordQuestionAndAnswer(user.UserName, viewModel.Password, viewModel.Question, viewModel.Answer);
                
                }
                catch (Exception exception)
                {
                    success = false;

                    ModelState.AddModelError("__FORM", exception.Message);
                }
                
                if (!success){
                   ModelState.AddModelError("__FORM", "Account Information Could Not Be Saved.  Please check your password");

                    viewModel.User = user;

                    return View(viewModel);
                }

                //Remove the token
                user.Token = null;

                Repository.OfType<User>().EnsurePersistent(user);  //remove the token
            }

            return RedirectToAction("LogOn", "Account");
        }

        /// <summary>
        /// Forgot password will ask the user for their username 
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AcceptPost]
        [BypassAntiForgeryToken]
        public ActionResult ForgotPassword(string id)
        {
            var member = MembershipService.GetUser(id);

            if (member == null)
            {
                ViewData["Message"] = "User information not found.";
                return View();
            }

            return this.RedirectToAction(a => a.ResetPassword(id));
        }

        public ActionResult ResetPassword(string id)
        {
            var member = MembershipService.GetUser(id);

            if (member == null)
            {
                return this.RedirectToAction(a => a.ForgotPassword());
            }

            ViewData["PasswordQuestion"] = member.PasswordQuestion;
            ViewData["UserName"] = member.UserName;

            return View();
        }

        [AcceptPost]
        [BypassAntiForgeryToken]
        public ActionResult ResetPassword(string id, string passwordAnswer)
        {
            var member = MembershipService.GetUser(id);

            if (member == null)
            {
                return this.RedirectToAction(a => a.ForgotPassword(id));
            }

            ViewData["PasswordQuestion"] = member.PasswordQuestion;
            ViewData["UserName"] = member.UserName;

            if (string.IsNullOrEmpty(passwordAnswer))
            {
                ViewData["Message"] = "Your answer can not be blank.  Please try again.";

                return View();
            }

            string newPassword;

            //We have a member, check if password answer
            try
            {
                newPassword = member.ResetPassword(passwordAnswer);
            }
            catch (MembershipPasswordException)
            {
                ViewData["Message"] = "Your answer could not be verified. Please try again.";

                return View();
            }

            //We have a valid password, email the user, then redirect
            MessageService.SendResetPasswordMessage(member.Email, member.UserName, newPassword);

            return this.RedirectToAction(a => a.ResetPasswordSuccess());
        }

        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [BypassAntiForgeryToken]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                if (MembershipService.ChangePassword(User.Identity.Name, currentPassword, newPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }

        public ActionResult ChangePasswordSuccess()
        {

            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }

        #endregion
    }

    public class NewUserViewModel
    {
        public static NewUserViewModel Create()
        {
            return new NewUserViewModel();
        }

        [Required]
        [Length(128)]
        public string Password { get; set; }

        [Valid]  
        public string ConfirmPassword { get; set; }

        [Required]
        [Length(256)]
        public string Question { get; set; }

        [Required]
        [Length(128)]
        public string Answer { get; set; }

        public User User { get; set; }
    }
}
