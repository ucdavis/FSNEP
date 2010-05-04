using CAESArch.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;

namespace FSNEP.Tests.Core
{
    public abstract class RepositoryTestBase
    {
        [TestInitialize]
        public void CreateDB()
        {
            Configuration config = new Configuration().Configure();
            new NHibernate.Tool.hbm2ddl.SchemaExport(config).Execute(false, true, false,
                                                                    CAESArch.Data.NHibernate.NHibernateSessionManager.
                                                                        Instance.GetSession().Connection, null);

            LoadData();
        }

        protected virtual void LoadData()
        {
            using (var ts = new TransactionScope())
            {
                //load base data

                ts.CommitTransaction();
            }
        }
    }
}