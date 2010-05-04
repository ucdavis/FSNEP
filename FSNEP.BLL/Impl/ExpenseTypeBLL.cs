using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;

namespace FSNEP.BLL.Impl
{
    public interface IExpenseTypeBLL : ILookupBLL<ExpenseType,int>
    {

    }

    public class ExpenseTypeBLL : LookupBLL<ExpenseType,int>, IExpenseTypeBLL
    {
        
    }
}