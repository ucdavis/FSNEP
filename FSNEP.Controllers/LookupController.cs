using System.Linq;
using System.Web.Mvc;
using CAESArch.BLL;
using CAESArch.Core.DataInterfaces;
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
        public ActionResult InactivateActivityType(int id)
        {
            //get the account
            var activityType = Repository.OfType<ActivityType>().GetNullableByID(id);

            if (activityType == null)
            {
                Message = "Activity Type Not Found";

                return this.RedirectToAction(a => a.ActivityTypes());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                activityType.IsActive = false;

                Repository.OfType<ActivityType>().EnsurePersistent(activityType);

                ts.CommitTransaction();
            }

            Message = "Activity Type Removed Successfully";

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
        public ActionResult InactivateActivityCategory(int id)
        {
            //get the account
            var activityCategory = Repository.OfType<ActivityCategory>().GetNullableByID(id);

            if (activityCategory == null)
            {
                Message = "Activity Category Not Found";

                return this.RedirectToAction(a => a.ActivityCategories());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                activityCategory.IsActive = false;

                Repository.OfType<ActivityCategory>().EnsurePersistent(activityCategory);

                ts.CommitTransaction();
            }

            Message = "Activity Category Removed Successfully";

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
        public ActionResult InactivateExpenseType(int id)
        {
            //get the account
            var expenseType = Repository.OfType<ExpenseType>().GetNullableByID(id);

            if (expenseType == null)
            {
                Message = "Expense Type Not Found";

                return this.RedirectToAction(a => a.ExpenseTypes());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                expenseType.IsActive = false;

                Repository.OfType<ExpenseType>().EnsurePersistent(expenseType);

                ts.CommitTransaction();
            }

            Message = "Expense Type Removed Successfully";

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
        public ActionResult InactivateAccount(int id)
        {
            //get the account
            var account = Repository.OfType<Account>().GetNullableByID(id);

            if (account == null)
            {
                Message = "Account Not Found";

                return this.RedirectToAction(a => a.Accounts());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                account.IsActive = false;

                Repository.OfType<Account>().EnsurePersistent(account);

                ts.CommitTransaction();
            }

            Message = "Account Removed Successfully";

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
            //get the project
            var project = Repository.OfType<Project>().GetNullableByID(id);

            if (project == null)
            {
                Message = "Project Not Found";

                return this.RedirectToAction(a => a.Projects());
            }

            //inactivate and save the project
            project.IsActive = false;
            Repository.OfType<Project>().EnsurePersistent(project);

            Message = "Project Removed Successfully";

            return this.RedirectToAction(a => a.Projects());
        }

        [AcceptPost]
        [Transaction]
        public ActionResult CreateProject(string newProjectName)
        {
            var newProject = new Project {IsActive = true, Name = newProjectName};

            ValidationHelper<Project>.Validate(newProject, ModelState);

            if (!ModelState.IsValid)
            {
                Message = "Project Creation Failed";

                return this.RedirectToAction(a => a.Projects());
            }

            //Add the new project
            Repository.OfType<Project>().EnsurePersistent(newProject);

            Message = "Project Created Successfully";

            return this.RedirectToAction(a => a.Projects());
        }
    }
}