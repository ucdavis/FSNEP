using NHibernate.Validator.Constraints;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class ActivityType : LookupObject
    {
        [Length(2,2)]
        [Required]
        public virtual string Indicator { get; set; }

        [NotNull]
        public virtual ActivityCategory ActivityCategory { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}) -- {2}", Name, Indicator, ActivityCategory.Name);
        }
    }
}