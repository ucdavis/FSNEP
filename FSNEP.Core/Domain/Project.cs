using System.Collections.Generic;
using CAESArch.Core.Domain;

namespace FSNEP.Core.Domain
{
    public class Project : LookupObject<Project, int>
    {
        public virtual IList<Account> Accounts { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}