using CAESArch.Core.Domain;

namespace FSNEP.Core.Domain
{
    public class ActivityCategory : LookupObject<ActivityCategory,int>
    {
        public override string ToString()
        {
            return Name;
        }
    }
}