using Newtonsoft.Json;
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

        [Length(256)]
        [JsonProperty]
        public virtual string Comment { get; set; }

        [NotNull]
        public virtual Project Project { get; set; }

        [NotNull]
        public virtual FundType FundType { get; set; }

        [NotNull]
        public virtual Account Account { get; set; }
    }
}