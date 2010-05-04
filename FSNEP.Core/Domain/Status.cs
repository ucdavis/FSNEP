using System;
using NHibernate.Validator.Constraints;
using UCDArch.Core.DomainModel;
using UCDArch.Core.NHibernateValidator.Extensions;

namespace FSNEP.Core.Domain
{
    public class Status : DomainObject
    {
        [Required]
        [Length(50)]
        public virtual string Name { get; set; }

        public virtual Option NameOption
        {
            get
            {
                switch (Name)
                {
                    case "Current":
                        return Option.Current;
                    case "PendingReview":
                        return Option.PendingReview;
                    case "Disapproved":
                        return Option.Disapproved;
                    case "Approved":
                        return Option.Approved;
                    default:
                        return Option.Current;
                }
            }
            set
            {
                Name = value.ToString();
            }
        }

        public enum Option
        {
            Current, PendingReview, Disapproved, Approved
        }

        public static string GetName(Option current)
        {
            return current.ToString();
        }
    }
}