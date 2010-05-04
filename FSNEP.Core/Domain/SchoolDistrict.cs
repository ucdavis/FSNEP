using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class SchoolDistrict : DomainObject<SchoolDistrict, int>
    {
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public SchoolDistrict()
        {

        }
    }
}