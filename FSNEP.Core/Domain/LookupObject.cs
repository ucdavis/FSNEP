using Newtonsoft.Json;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class LookupObject : DomainObject
    {
        [Required]
        [Length(50)]
        [JsonProperty]
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }
    }
}