using System.Linq;
using System.Web.Mvc;
using CAESArch.BLL;
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
        public ActionResult CreateActivityType(ActivityType newActivityType, int activityCategoryId)
        {
            newActivityType.IsActive = true;
            newActivityType.ActivityCategory = Repository.OfType<ActivityCategory>().GetNullableByID(activityCategoryId);
            
            ValidationHelper<ActivityType>.Validate(newActivityType, ModelState);

            if (!ModelState.IsValid)
            {
                Message = "Activity Type Creation Failed";

                return this.RedirectToAction(a => a.ActivityTypes());
            }

            //Add the new project
            using (var ts = new TransactionScope())
            {
                Repository.OfType<ActivityType>().EnsurePersistent(newActivityType);

                ts.CommitTransaction();
            }

            Message = "Activity Type Created Successfully";

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
        public ActionResult CreateActivityCategory(ActivityCategory newActivityCategory)
        {
            newActivityCategory.IsActive = true;

            ValidationHelper<ActivityCategory>.Validate(newActivityCategory, ModelState);

            if (!ModelState.IsValid)
            {
                Message = "Activity Category Creation Failed";

                return this.RedirectToAction(a => a.ActivityCategories());
            }

            //Add the new project
            using (var ts = new TransactionScope())
            {
                Repository.OfType<ActivityCategory>().EnsurePersistent(newActivityCategory);

                ts.CommitTransaction();
            }

            Message = "Activity Category Created Successfully";

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
        public ActionResult CreateExpenseType(ExpenseType newExpenseType)
        {
            newExpenseType.IsActive = true;

            ValidationHelper<ExpenseType>.Validate(newExpenseType, ModelState);

            if (!ModelState.IsValid)
            {
                Message = "Expense Type Creation Failed";

                return this.RedirectToAction(a => a.ExpenseTypes());
            }

            //Add the new project
            using (var ts = new TransactionScope())
            {
                Repository.OfType<ExpenseType>().EnsurePersistent(newExpenseType);

                ts.CommitTransaction();
            }

            Message = "Expense Type Created Successfully";

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
        public ActionResult CreateAccount(Account newAccount)
        {
            newAccount.IsActive = true;

            ValidationHelper<Account>.Validate(newAccount, ModelState);

            if (!ModelState.IsValid)
            {
                Message = "Account Creation Failed";

                return this.RedirectToAction(a => a.Accounts());
            }

            //Add the new project
            using (var ts = new TransactionScope())
            {
                Repository.OfType<Account>().EnsurePersistent(newAccount);

                ts.CommitTransaction();
            }

            Message = "Account Created Successfully";

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