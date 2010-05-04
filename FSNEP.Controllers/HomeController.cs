using System;
using System.Linq;
using System.Web.Mvc;
using FSNEP.Core.Domain;
using FSNEP.Core.Abstractions;
using UCDArch.Web.Attributes;
using System.Security.Principal;

namespace FSNEP.Controllers
{
    [HandleTransactionsManually]
    public class HomeController : ApplicationController
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            var viewModel = UserPermissionsViewModel.Create(CurrentUser);

            return View(viewModel);
        }


        public ActionResult Error(string errorMessage)
        {
            return View("Error", errorMessage);
        }

        [BypassAntiForgeryToken]
        public ActionResult Menu()
        {
            var viewModel = UserPermissionsViewModel.Create(CurrentUser);

            return View("Menu", viewModel); //Menu partial
        }

        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// This will show the semi annual certification form if these requirements are met
        /// 1) The current user is 1.0FTE & a time record user
        /// 2) It is April 1-15 or October 1-15
        /// </summary>
        [BypassAntiForgeryToken] //We don't need to look at the anti forgery token
        public ActionResult SemiAnnualCertification()
        {
            if (!CurrentUser.Identity.IsAuthenticated) //return nothing if the user isn't authenticated
            {
                return Content(string.Empty);
            }

            var viewModel = SemiAnnualCertificationViewModel.Create();
            viewModel.ShouldShowCertificationLink = false;

            var now = SystemTime.Now();

            //Is it between april 1-15 or october 1-15
            var aprilFirst = new DateTime(now.Year, 4, 1);
            var octoberFirst = new DateTime(now.Year, 10, 1);

            if (
                    (aprilFirst <= now && now < aprilFirst.AddDays(15)) ||
                    (octoberFirst <= now && now < octoberFirst.AddDays(15))
                )
            {
                //Now we are in one of the correct date ranges

                var userRepository = Repository.OfType<User>();

                userRepository.DbContext.BeginTransaction(); //Handle the transaction manually to save db strain on each load

                //Does the current user have 1.0FTE
                var currentUserHasOneFte =
                    userRepository.Queryable.Where(x => x.UserName == CurrentUser.Identity.Name && x.FTE == 1).Any();

                userRepository.DbContext.CommitTransaction();

                if (currentUserHasOneFte && CurrentUser.IsInRole(RoleNames.RoleTimeSheet)) viewModel.ShouldShowCertificationLink = true;
            }
            
            return View(viewModel);
        }
    }

    public class UserPermissionsViewModel
    {
        /// <summary>
        /// Determines a user's roles through the user context
        /// </summary>
        /// <remarks>
        /// Could possibly be improved by getting all roles at once and enumerating this collection
        /// </remarks>
        /// <param name="userContext">IPrincipal current user context</param>
        public static UserPermissionsViewModel Create(IPrincipal userContext)
        {
            var viewModel = new UserPermissionsViewModel
                                {
                                    IsAdmin = userContext.IsInRole(RoleNames.RoleAdmin),
                                    IsProjectAdmin = userContext.IsInRole(RoleNames.RoleProjectAdmin),
                                    IsTimeRecordUser = userContext.IsInRole(RoleNames.RoleTimeSheet),
                                    IsCostShareUser = userContext.IsInRole(RoleNames.RoleNonSalary),
                                    IsSupervisor = userContext.IsInRole(RoleNames.RoleSupervisor) ||
                                                   userContext.IsInRole(RoleNames.RoleDelegateSupervisor)
                                };


            return viewModel;
        }

        public bool IsSupervisor { get; set; }

        public bool IsCostShareUser { get; set; }

        public bool IsTimeRecordUser { get; set; }

        public bool IsProjectAdmin { get; set; }

        public bool IsAdmin { get; set; }
    }

    public class SemiAnnualCertificationViewModel
    {
        public static SemiAnnualCertificationViewModel Create()
        {
            var viewModel = new SemiAnnualCertificationViewModel
                                {
                                    ShouldShowCertificationLink = false
                                };

            return viewModel;
        }

        public bool ShouldShowCertificationLink { get; set; }
    }
}
