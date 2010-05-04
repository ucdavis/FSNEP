using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class CostShareEntry : Entry
    {
        [NotNull]
        public virtual ExpenseType ExpenseType { get; set; }

        [NotNull]
        public virtual EntryFile EntryFile { get; set; }

        public virtual double Amount { get; set; }

        [Required]
        [Length(128)]
        public virtual string Description { get; set; }

        public virtual bool Exclude { get; set; }

        [Length(256)]
        public virtual string ExcludeReason { get; set; }
    }
}