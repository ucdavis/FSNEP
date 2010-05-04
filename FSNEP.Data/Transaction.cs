using CAESArch.Data.NHibernate;

namespace FSNEP.Data
{
    public class Transaction : ITransaction
    {
        private NHibernate.ITransaction _transaction;

        public Transaction()
        {
            _transaction = NHibernateSessionManager.Instance.GetSession().Transaction;
        }

        public void Begin()
        {
            _transaction = NHibernateSessionManager.Instance.GetSession().BeginTransaction();
        }

        public bool IsActive
        {
            get { return _transaction.IsActive; }
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}