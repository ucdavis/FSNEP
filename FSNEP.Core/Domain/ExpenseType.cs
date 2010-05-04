using CAESArch.Core.Domain;

namespace FSNEP.Core.Domain
{
    public class ExpenseType : LookupObject<ExpenseType, int>
    {
        public override string ToString()
        {
            return Name;
        }
    }
}