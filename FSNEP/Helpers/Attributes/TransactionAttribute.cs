using System.Web.Mvc;
using CAESArch.Data.NHibernate;
using NHibernate;

namespace FSNEP.Helpers.Attributes
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        private readonly ITransaction currentTransaction;

        public TransactionAttribute()
        {
            currentTransaction = NHibernateSessionManager.Instance.GetSession().Transaction;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            currentTransaction.Begin();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (currentTransaction.IsActive)
            {
                if (filterContext.Exception == null)
                {
                    currentTransaction.Commit();
                }
                else
                {
                    currentTransaction.Rollback();
                }
            }
        }
    }
}