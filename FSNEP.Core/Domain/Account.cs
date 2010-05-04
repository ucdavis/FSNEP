using System.Collections.Generic;
using FSNEP.Core.Validators;

namespace FSNEP.Core.Domain
{
    public class Account : LookupObject
    {
        [RangeDouble(0,0.3)]
        public virtual double IndirectCost { get; set; }

        public virtual double IndirectCostPercent
        {
            get
            {
                return IndirectCost * 100.0;
            }
            set
            {
                IndirectCost = value / 100.0;
            }
        }

        public virtual IList<Project> Projects { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}%)", Name, IndirectCostPercent);
        }
    }
}