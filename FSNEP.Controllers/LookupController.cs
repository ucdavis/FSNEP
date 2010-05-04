using System.Linq;
using System.Web.Mvc;
using FSNEP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;
using UCDArch.Core.DomainModel;
using UCDArch.Web.Helpers;
using FSNEP.Controllers.Helpers.Attributes;

namespace FSNEP.Controllers
{
    [AdminOnly]
    public class LookupController : ApplicationController
    {
        public ActionResult ActivityTypes()
        {
            var activeActivityTypes =
                Repository.OfType<ActivityType>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();

            ViewData["ActivityCategories"] = Repository.OfType<ActivityCategory>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();

            return View(activeActivityTypes);
        }

        [AcceptPost]
        public ActionResult InactivateActivityType(int id)
        {
            InactivateEntity<ActivityType>(id, "Activity Type");

            return this.RedirectToAction(a => a.ActivityTypes());
        }

        [AcceptPost]
        public ActionResult CreateActivityType(ActivityType newActivityType, int activityCategoryId)
        {
            newActivityType.ActivityCategory = Repository.OfType<ActivityCategory>().GetNullableByID(activityCategoryId);

            CreateEntity<ActivityType, int>(newActivityType, "Activity Type");
            
            return this.RedirectToAction(a => a.ActivityTypes());
        }


        public ActionResult ActivityCategories(string returnTo)
        {
            var activeActivityCategories =
                Repository.OfType<ActivityCategory>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
            
            return View(activeActivityCategories);
        }

        [AcceptPost]
        public ActionResult InactivateActivityCategory(int id)
        {
            InactivateEntity<ActivityCategory>(id, "Activity Category");
            
            return this.RedirectToAction(a => a.ActivityCategories(null));
        }

        [AcceptPost]
        public ActionResult CreateActivityCategory(ActivityCategory newActivityCategory, string returnTo)
        {
            CreateEntity<ActivityCategory, int>(newActivityCategory, "Activity Category");

            return returnTo == "ActivityType" ? this.RedirectToAction(a => a.ActivityTypes()) : this.RedirectToAction(a => a.ActivityCategories(null));
        }


        public ActionResult ExpenseTypes()
        {
            var activeExpenseTypes =
                Repository.OfType<ExpenseType>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();

            return View(activeExpenseTypes);
        }

        [AcceptPost]
        public ActionResult InactivateExpenseType(int id)
        {
            InactivateEntity<ExpenseType>(id, "Expense Type");
            
            return this.RedirectToAction(a => a.ExpenseTypes());
        }

        [AcceptPost]
        public ActionResult CreateExpenseType(ExpenseType newExpenseType)
        {
            CreateEntity<ExpenseType, int>(newExpenseType, "Expense Type");

            return this.RedirectToAction(a => a.ExpenseTypes());
        }


        public ActionResult Accounts()
        {
            var activeAccounts =
                Repository.OfType<Account>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
            
            return View(activeAccounts);
        }

        [AcceptPost]
        public ActionResult InactivateAccount(int id)
        {
            InactivateEntity<Account>(id, "Account");

            return this.RedirectToAction(a => a.Accounts());
        }

        [AcceptPost]
        public ActionResult CreateAccount(Account newAccount)
        {
            CreateEntity<Account, int>(newAccount, "Account");
            
            return this.RedirectToAction(a => a.Accounts());
        }

        public ActionResult HoursInMonths()
        {
            var hoursInMonths =
                Repository.OfType<HoursInMonth>().Queryable.OrderBy(a => a.Id.Year).ThenBy(a => a.Id.Month).ToList();

            return View(hoursInMonths);
        }

        [AcceptPost]
        public ActionResult CreateHoursInMonth(HoursInMonth newHoursInMonth, YearMonthComposite hoursInMonthId)
        {
            var hoursInMonth = new HoursInMonth(hoursInMonthId.Year, hoursInMonthId.Month) { Hours = newHoursInMonth.Hours };

            CreateEntity<HoursInMonth, YearMonthComposite>(hoursInMonth, "Hours In Month");

            return this.RedirectToAction(a => a.HoursInMonths());
        }

        /// <summary>
        /// Return a list of all projects
        /// </summary>
        public ActionResult Projects()
        {
            var activeProjects =
                Repository.OfType<Project>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
            
            return View(activeProjects);
        }

        [AcceptPost]
        public ActionResult InactivateProject(int id)
        {
            InactivateEntity<Project>(id, "Project");

            return this.RedirectToAction(a => a.Projects());
        }

        [AcceptPost]
        public ActionResult CreateProject(Project newProject)
        {
            CreateEntity<Project, int>(newProject, "Project");

            return this.RedirectToAction(a => a.Projects());
        }

        private void CreateEntity<T, IdT>(T entity, string type) where T : DomainObjectWithTypedId<IdT>
        {
            var lookupEntity = entity as LookupObject; // If this is a looked entity we want to make sure isActive is true

            if (lookupEntity != null) lookupEntity.IsActive = true;

            entity.TransferValidationMessagesTo(ModelState);
            
            if (!ModelState.IsValid)
            {
                Message = type + " Creation Failed. " + GetErrorMessages<T,IdT>(entity);

                return;
            }

            //Add the new entity
            Repository.OfType<T>().EnsurePersistent(entity);

            Message = type + " Created Successfully";
        }

        private void InactivateEntity<T>(int id, string type) where T : LookupObject
        {
            var entity = Repository.OfType<T>().GetNullableByID(id);

            if (Equals(entity, default(T)))
            {
                Message = type + " Not Found";

                return;
            }

            entity.IsActive = false;

            Repository.OfType<T>().EnsurePersistent(entity);

            Message = type + " Removed Successfully";
        }

        private string GetErrorMessages<T, IdT>(T objToValidate) where T : DomainObjectWithTypedId<IdT>
        {
            var sb = new System.Text.StringBuilder();
            
            foreach (var valResult in objToValidate.ValidationResults())
            {
                sb.Append(valResult.PropertyName + ": " + valResult.Message);
            }

            return sb.ToString();

        }
    }
}