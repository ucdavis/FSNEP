using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class ExpenseType : DomainObject<ExpenseType, int>
    {
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string Name { get; set; }

        public ExpenseType()
        {

        }
    }
}