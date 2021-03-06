using Newtonsoft.Json;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;

namespace FSNEP.Core.Domain
{
    /// <summary>
    /// Base class for entry types
    /// </summary>
    public class Entry : DomainObject
    {
        [NotNull]
        public virtual Record Record { get; set; }

        private string _comment;

        [Length(256)]
        [JsonProperty]
        public virtual string Comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;

                if (_comment == "null") _comment = null;
            }
        }

        [NotNull]
        public virtual Project Project { get; set; }

        [NotNull]
        public virtual FundType FundType { get; set; }

        [NotNull]
        public virtual Account Account { get; set; }
    }
}