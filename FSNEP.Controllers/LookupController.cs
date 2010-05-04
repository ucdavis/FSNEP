using System.Web.Mvc;
using CAESArch.BLL;
using FSNEP.BLL.Impl;
using FSNEP.Controllers.Helpers;
using FSNEP.Controllers.Helpers.Attributes;
using FSNEP.Core.Domain;
using MvcContrib;
using MvcContrib.Attributes;

namespace FSNEP.Controllers
{
    public class LookupController : SuperController
    {
        public IProjectBLL ProjectBLL;
        public IAccountBLL AccountBLL;
        public IExpenseTypeBLL ExpenseTypeBLL;

        public LookupController(IProjectBLL projectBLL, IAccountBLL accountBLL)
        {
            ProjectBLL = projectBLL;
            AccountBLL = accountBLL;
        }

        [Transaction]
        public ActionResult ExpenseTypes()
        {
            var activeAccounts = ExpenseTypeBLL.GetActive();

            return View(activeAccounts);
        }

        [AcceptPost]
        public ActionResult InactivateExpenseType(int expenseTypeId)
        {
            //get the account
            var expenseType = ExpenseTypeBLL.Repository.GetNullableByID(expenseTypeId);

            if (expenseType == null)
            {
                Message = "Expense Type Not Found";

                return this.RedirectToAction(a => a.ExpenseTypes());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                expenseType.IsActive = false;

                ExpenseTypeBLL.Repository.EnsurePersistent(expenseType);

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
                ExpenseTypeBLL.Repository.EnsurePersistent(newExpenseType);

                ts.CommitTransaction();
            }

            Message = "Expense Type Created Successfully";

            return this.RedirectToAction(a => a.ExpenseTypes());
        }


        [Transaction]
        public ActionResult Accounts()
        {
            var activeAccounts = AccountBLL.GetActive();
            
            return View(activeAccounts);
        }

        [AcceptPost]
        public ActionResult InactivateAccount(int accountId)
        {
            //get the account
            var account = AccountBLL.Repository.GetNullableByID(accountId);

            if (account == null)
            {
                Message = "Account Not Found";

                return this.RedirectToAction(a => a.Accounts());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                account.IsActive = false;

                AccountBLL.Repository.EnsurePersistent(account);

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
                AccountBLL.Repository.EnsurePersistent(newAccount);

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
            var activeProjects = ProjectBLL.GetActive();
            
            return View(activeProjects);
        }

        [AcceptPost]
        public ActionResult InactivateProject(int projectId)
        {
            //get the project
            var project = ProjectBLL.Repository.GetNullableByID(projectId);

            if (project == null)
            {
                Message = "Project Not Found";

                return this.RedirectToAction(a => a.Projects());
            }

            //inactivate the project
            using (var ts = new TransactionScope())
            {
                project.IsActive = false;

                ProjectBLL.Repository.EnsurePersistent(project);

                ts.CommitTransaction();
            }

            Message = "Project Removed Successfully";

            return this.RedirectToAction(a => a.Projects());
        }

        [AcceptPost]
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
            using (var ts = new TransactionScope())
            {
                ProjectBLL.Repository.EnsurePersistent(newProject);

                ts.CommitTransaction();
            }

            Message = "Project Created Successfully";

            return this.RedirectToAction(a => a.Projects());
        }
    }
}