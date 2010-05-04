using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class ActivityType : LookupObject<ActivityType, int>
    {
        [NotNullValidator]
        [StringLengthValidator(2, 2)]
        public virtual string Indicator { get; set; }

        [NotNullValidator]
        public virtual ActivityCategory ActivityCategory { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}) -- {2}", Name, Indicator, ActivityCategory.Name);
        }
    }
}