using System.Linq;
using System.Web.Mvc;
using CAESArch.Core.Domain;
using FSNEP.Controllers.Helpers;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;

namespace FSNEP.Controllers
{
    [Transaction]
    [Authorize]
    public class LookupController : SuperController
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
            InactivateEntity<ActivityType, int>(id, "Activity Type");

            return this.RedirectToAction(a => a.ActivityTypes());
        }

        [AcceptPost]
        public ActionResult CreateActivityType(ActivityType newActivityType, int activityCategoryId)
        {
            newActivityType.ActivityCategory = Repository.OfType<ActivityCategory>().GetNullableByID(activityCategoryId);

            CreateEntity<ActivityType, int>(newActivityType, "Activity Type");
            
            return this.RedirectToAction(a => a.ActivityTypes());
        }


        public ActionResult ActivityCategories()
        {
            var activeActivityCategories =
                Repository.OfType<ActivityCategory>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
            
            return View(activeActivityCategories);
        }

        [AcceptPost]
        public ActionResult InactivateActivityCategory(int id)
        {
            InactivateEntity<ActivityCategory, int>(id, "Activity Category");
            
            return this.RedirectToAction(a => a.ActivityCategories());
        }

        [AcceptPost]
        public ActionResult CreateActivityCategory(ActivityCategory newActivityCategory)
        {
            CreateEntity<ActivityCategory, int>(newActivityCategory, "Activity Category");

            return this.RedirectToAction(a => a.ActivityCategories());
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
            InactivateEntity<ExpenseType, int>(id, "Expense Type");
            
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
            InactivateEntity<Account, int>(id, "Account");

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
                Repository.OfType<HoursInMonth>().Queryable.OrderBy(a => a.ID.Year).ThenBy(a => a.ID.Month).ToList();

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
            InactivateEntity<Project, int>(id, "Project");

            return this.RedirectToAction(a => a.Projects());
        }

        [AcceptPost]
        public ActionResult CreateProject(Project newProject)
        {
            CreateEntity<Project, int>(newProject, "Project");

            return this.RedirectToAction(a => a.Projects());
        }

        private void CreateEntity<T, IdT>(T entity, string type) where T : DomainObject<T, IdT>
        {
            var lookupEntity = entity as LookupObject<T, IdT>; // If this is a looked entity we want to make sure isActive is true

            if (lookupEntity != null) lookupEntity.IsActive = true;

            ValidationHelper<T>.Validate(entity, ModelState);

            if (!ModelState.IsValid)
            {
                Message = type + " Creation Failed";

                return;
            }

            //Add the new entity
            Repository.OfType<T>().EnsurePersistent(entity);

            Message = type + " Created Successfully";
        }

        private void InactivateEntity<T, IdT>(int id, string type) where T : LookupObject<T, IdT>
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
    }
}