using System.Web.Mvc;
using CAESArch.Data.NHibernate;
using NHibernate;

namespace FSNEP.Controllers.Helpers.Attributes
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        private readonly ITransaction _currentTransaction;

        public TransactionAttribute()
        {
            _currentTransaction = NHibernateSessionManager.Instance.GetSession().Transaction;
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