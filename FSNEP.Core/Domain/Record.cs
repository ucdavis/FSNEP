using UCDArch.Core.DomainModel;
using NHibernate.Validator.Constraints;

namespace FSNEP.Core.Domain
{
    public class Record : DomainObject
    {
        [Range(1, 12)]
        public virtual int Month { get; set; }

        [Min(1)]
        public virtual int Year { get; set; }

        [NotNull]
        public virtual User User { get; set; }

        [NotNull]
        public virtual Status Status { get; set; }

        [Length(512)]
        public virtual string ReviewComment { get; set; }
    }
}