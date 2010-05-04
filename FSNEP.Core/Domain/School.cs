using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class School : DomainObject<School, string>
    {
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public School()
        {

        }

        public virtual void SetID(string ID)
        {
            this.id = ID;
        }
    }
}