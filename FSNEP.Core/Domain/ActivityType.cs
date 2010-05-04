using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class ActivityType : DomainObject<ActivityType, int>
    {
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string Name { get; set; }

        [NotNullValidator]
        [StringLengthValidator(2, 2)]
        public virtual string Indicator { get; set; }
    }
}