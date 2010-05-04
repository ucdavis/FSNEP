using System;
using CAESArch.BLL;
using CAESArch.Core.DataInterfaces;
using FSNEP.BLL.Interfaces;
using NHibernate;
using CAESArch.Core.Utils;
using FSNEP.Core.Domain;

namespace FSNEP
{
    public class AuditInterceptor : EmptyInterceptor
    {
        public IUserAuth UserAuth { get; private set; }
        public IRepository Repository { get; set; }

        public AuditInterceptor(IUserAuth userAuth, IRepository repository)
        {
            Check.Require(userAuth != null, "User Authorization Context is Required");

            UserAuth = userAuth;
            Repository = repository;
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            base.OnDelete(entity, id, state, propertyNames, types);
        }

        public void AuditObjectModification(object entity, object id, AuditActionType auditActionType)
        {
            var audit = new Audit
            {
                AuditDate = DateTime.Now,
                ObjectName = entity.GetType().Name,
                ObjectId = id == null ? null : id.ToString(),
                Username = UserAuth.CurrentUserName
            };

            audit.SetActionCode(auditActionType);

            Repository.OfType<Audit>().EnsurePersistent(audit);
        }
    }
}