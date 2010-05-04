using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class Status : DomainObject<Status, int>
    {
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string Name { get; set; }

        public Status()
        {

        }
    }
}