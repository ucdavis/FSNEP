using System;
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
            AuditObjectModification(entity, id, AuditActionType.Delete);

            base.OnDelete(entity, id, state, propertyNames, types);
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            AuditObjectModification(entity, id, AuditActionType.Create);

            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
        {
            AuditObjectModification(entity, id, AuditActionType.Update);

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public void AuditObjectModification(object entity, object id, AuditActionType auditActionType)
        {
            if (entity is Audit) return;

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