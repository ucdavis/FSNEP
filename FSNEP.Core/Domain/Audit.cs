using System;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using CAESArch.Core.Validators;

namespace FSNEP.Core.Domain
{
    public class Audit : DomainObject<Audit,Guid>
    {
        [RequiredValidator]
        [StringLengthValidator(50)]
        public virtual string ObjectName { get; set; }

        [IgnoreNulls]
        [StringLengthValidator(50)]
        public virtual string ObjectId { get; set; }

        [RequiredValidator]
        [StringLengthValidator(1)]
        public virtual string ActionCodeId { get; set; }

        [RequiredValidator]
        [StringLengthValidator(256)]
        public virtual string Username { get; set; }

        public virtual DateTime AuditDate { get; set; }

        public virtual void SetActionCode(AuditActionType auditActionType)
        {
            switch (auditActionType)
            {
                case AuditActionType.Create:
                    ActionCodeId = "C";
                    break;
                case AuditActionType.Update:
                    ActionCodeId = "U";
                    break;
                case AuditActionType.Delete:
                    ActionCodeId = "D";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("auditActionType");
            }
        }
    }

    public enum AuditActionType
    {
        Create, Update, Delete
    }
}