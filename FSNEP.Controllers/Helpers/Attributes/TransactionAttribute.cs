using System.Web.Mvc;
using FSNEP.Data;
using System;

namespace FSNEP.Controllers.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TransactionAttribute : ActionFilterAttribute
    {
        private readonly ITransaction _currentTransaction;

        public TransactionAttribute()
        {
            _currentTransaction = new Transaction();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _currentTransaction.Begin();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (_currentTransaction.IsActive)
            {
                if (filterContext.Exception == null)
                {
                    _currentTransaction.Commit();
                }
                else
                {
                    _currentTransaction.Rollback();
                }
            }
        }
    }
}