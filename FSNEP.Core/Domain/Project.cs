using System.Collections.Generic;

namespace FSNEP.Core.Domain
{
    public class Project : LookupObject
    {
        public virtual IList<Account> Accounts { get; set; }

        public Project()
        {
            Accounts = new List<Account>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}