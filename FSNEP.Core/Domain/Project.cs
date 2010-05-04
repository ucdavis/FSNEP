using System.Collections.Generic;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class Project : DomainObject<Project, int>
    {
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual IList<Account> Accounts { get; set; }

        public Project()
        {

        }
    }
}