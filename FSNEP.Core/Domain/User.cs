using System;
using System.Collections.Generic;
using CAESArch.Core.Domain;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace FSNEP.Core.Domain
{
    public class User : DomainObject<User, Guid>
    {
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string FirstName { get; set; }
        
        [NotNullValidator]
        [StringLengthValidator(50)]
        public virtual string LastName { get; set; }

        public virtual User CreatedBy { get; set; }

        [NotNullValidator]
        public virtual User Supervisor { get; set; }

        [IgnoreNulls]
        public virtual IList<FundType> FundTypes { get; set; }

        // [NotNullValidator] Must be at least one project?
        public virtual IList<Project> Projects { get; set; }

        [RangeValidator(0.00, RangeBoundaryType.Exclusive, 0.00, RangeBoundaryType.Ignore, MessageTemplate = "Must be greater than zero")]
            public virtual double Salary { get; set; }

        [RangeValidator(0.00, RangeBoundaryType.Inclusive, 2.00, RangeBoundaryType.Inclusive)]
        public virtual double BenefitRate { get; set; }

        [RangeValidator(0.00, RangeBoundaryType.Exclusive, 1.00, RangeBoundaryType.Inclusive)] //FTE is (0,1]
            public virtual double FTE { get; set; }

        public virtual bool ResetPassword { get; set; }

        public virtual bool IsActive { get; set; }

        public User()
        {
            Projects = new List<Project>();
            FundTypes = new List<FundType>();
        }

        public virtual string FullName
        {
            get {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }

        public virtual string FullNameLastFirst
        {
            get
            {
                return string.Format("{0}, {1}", LastName, FirstName);
            }
        }

        public virtual Guid Token { get; set; }

        public virtual void SetUserID(Guid userID)
        {
            this.id = userID;
        }
    }
}