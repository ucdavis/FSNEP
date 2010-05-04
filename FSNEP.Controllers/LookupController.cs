using System.Linq;
using System.Web.Mvc;
using CAESArch.Core.DataInterfaces;
using CAESArch.Core.Domain;
using FSNEP.Controllers.Helpers;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;

namespace FSNEP.Controllers
{
    public class LookupController : SuperController
    {
        public IRepository Repository;

        public LookupController(IRepository repository)
        {
            Repository = repository;
        }

        [Transaction]
        public ActionResult ActivityTypes()
        {
            var activeActivityTypes =
                Repository.OfType<ActivityType>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();

            ViewData["ActivityCategories"] = Repository.OfType<ActivityCategory>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();

            return View(activeActivityTypes);
        }

        [AcceptPost]
        [Transaction]
        public ActionResult InactivateActivityType(int id)
        {
            InactivateEntity<ActivityType, int>(id, "Activity Type");

            return this.RedirectToAction(a => a.ActivityTypes());
        }

        [AcceptPost]
        [Transaction]
        public ActionResult CreateActivityType(ActivityType newActivityType, int activityCategoryId)
        {
            newActivityType.ActivityCategory = Repository.OfType<ActivityCategory>().GetNullableByID(activityCategoryId);

            CreateEntity<ActivityType, int>(newActivityType, "Activity Type");
            
            return this.RedirectToAction(a => a.ActivityTypes());
        }


        [Transaction]
        public ActionResult ActivityCategories()
        {
            var activeActivityCategories =
                Repository.OfType<ActivityCategory>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
            
            return View(activeActivityCategories);
        }

        [AcceptPost]
        [Transaction]
        public ActionResult InactivateActivityCategory(int id)
        {
            InactivateEntity<ActivityCategory, int>(id, "Activity Category");
            
            return this.RedirectToAction(a => a.ActivityCategories());
        }

        [AcceptPost]
        [Transaction]
        public ActionResult CreateActivityCategory(ActivityCategory newActivityCategory)
        {
            CreateEntity<ActivityCategory, int>(newActivityCategory, "Activity Category");

            return this.RedirectToAction(a => a.ActivityCategories());
        }


        [Transaction]
        public ActionResult ExpenseTypes()
        {
            var activeExpenseTypes =
                Repository.OfType<ExpenseType>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();

            return View(activeExpenseTypes);
        }

        [AcceptPost]
        [Transaction]
        public ActionResult InactivateExpenseType(int id)
        {
            InactivateEntity<ExpenseType, int>(id, "Expense Type");
            
            return this.RedirectToAction(a => a.ExpenseTypes());
        }

        [AcceptPost]
        [Transaction]
        public ActionResult CreateExpenseType(ExpenseType newExpenseType)
        {
            CreateEntity<ExpenseType, int>(newExpenseType, "Expense Type");

            return this.RedirectToAction(a => a.ExpenseTypes());
        }


        [Transaction]
        public ActionResult Accounts()
        {
            var activeAccounts =
                Repository.OfType<Account>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
            
            return View(activeAccounts);
        }

        [AcceptPost]
        [Transaction]
        public ActionResult InactivateAccount(int id)
        {
            InactivateEntity<Account, int>(id, "Account");

            return this.RedirectToAction(a => a.Accounts());
        }

        [AcceptPost]
        [Transaction]
        public ActionResult CreateAccount(Account newAccount)
        {
            CreateEntity<Account, int>(newAccount, "Account");
            
            return this.RedirectToAction(a => a.Accounts());
        }


        /// <summary>
        /// Return a list of all projects
        /// </summary>
        [Transaction]
        public ActionResult Projects()
        {
            var activeProjects =
                Repository.OfType<Project>().Queryable.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
            
            return View(activeProjects);
        }

        [AcceptPost]
        [Transaction]
        public ActionResult InactivateProject(int id)
        {
            InactivateEntity<Project, int>(id, "Project");

            return this.RedirectToAction(a => a.Projects());
        }

        [AcceptPost]
        [Transaction]
        public ActionResult CreateProject(Project newProject)
        {
            CreateEntity<Project, int>(newProject, "Project");

            return this.RedirectToAction(a => a.Projects());
        }

        private void CreateEntity<T, IdT>(T entity, string type) where T : LookupObject<T, IdT>
        {
            entity.IsActive = true;

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