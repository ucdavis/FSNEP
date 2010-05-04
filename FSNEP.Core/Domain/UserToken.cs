using System;
using CAESArch.Core.Domain;

namespace FSNEP.Core.Domain
{
    public class UserToken : DomainObject<UserToken, Guid>
    {
        public virtual User AssociatedUser { get; set; }

        public virtual void SetID(Guid token)
        {
            this.id = token;
        }
    }
}