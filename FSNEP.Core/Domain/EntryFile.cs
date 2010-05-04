using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;
using FSNEP.Core.Abstractions;

namespace FSNEP.Core.Domain
{
    public class EntryFile : DomainObject
    {
        public EntryFile()
        {
            DateAdded = SystemTime.Now();
        }

        [NotNullNotEmpty]
        public virtual byte[] Content { get; set; }

        [Required]
        [Length(50)]
        public virtual string Name { get; set; }

        public virtual DateTime DateAdded { get; set; }
    }
}