using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class RecordTracking : DomainObject
    {
        [Required]
        [Length(256)]
        public virtual string UserName { get; set; }

        public virtual DateTime ActionDate { get; set; }

        [NotNull]
        public virtual Record Record { get; set; }

        [NotNull]
        public virtual Status Status { get; set; }

        [NotNull]
        public virtual byte[] DigitalSignature { get; set; }
    }
}