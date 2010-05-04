using System.Collections.Generic;

namespace FSNEP.Core.Domain
{
    public class ActivityCategory : LookupObject
    {
        public virtual IList<ActivityType> ActivityTypes { get; set; }

        public ActivityCategory()
        {
            ActivityTypes = new List<ActivityType>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}