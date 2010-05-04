using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    /// <summary>
    /// Base class for entry types
    /// </summary>
    public class Entry : DomainObject
    {
        [NotNull]
        public virtual Record Record { get; set; }

        [Required]
        [Length(256)]
        public virtual string Comment { get; set; }

        //Project/FundType/FinanceAccount?
    }
}