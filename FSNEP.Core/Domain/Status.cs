using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class Status : DomainObject
    {
        [Required]
        [Length(50)]
        public virtual string Name { get; set; }

        public enum Option
        {
            Current, PendingReview, Disapproved, Approved
        }
    }
}