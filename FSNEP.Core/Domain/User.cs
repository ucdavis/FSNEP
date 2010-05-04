using System;
using System.Collections.Generic;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class User : DomainObjectWithTypedId<Guid>, IHasAssignedId<Guid>
    {
        [Length(1, 256)]
        [Required(Message = "Username should be set upon creation")]
        public virtual string UserName { get; set; }

        [Length(50)]
        [Required]
        public virtual string FirstName { get; set; }

        [Length(50)]
        [Required]
        public virtual string LastName { get; set; }

        public virtual User CreatedBy { get; set; }

        [NotNull]
        public virtual User Supervisor { get; set; }

        public virtual IList<FundType> FundTypes { get; set; }

        // [NotNullValidator] Must be at least one project?
        public virtual IList<Project> Projects { get; set; }

        [RangeDouble(Min=0.01, Message = "Must be greater than zero")]
        public virtual double Salary { get; set; }

        [RangeDouble(0, 2)]
        public virtual double BenefitRate { get; set; }

        [RangeDouble(0.01,1)]
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
            this.Id = userID;
        }

        public virtual void SetAssignedIdTo(Guid assignedId)
        {
            Id = assignedId;
        }
    }
}