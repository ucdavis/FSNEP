using FSNEP.BLL.Interfaces;
using FSNEP.Core.Domain;
namespace FSNEP.BLL.Impl
{
    public interface IHoursInMonthBLL : ILookupBLL<HoursInMonth, YearMonthComposite>
    {

    }

    public class HoursInMonthBLL : LookupBLL<HoursInMonth, YearMonthComposite>, IHoursInMonthBLL
    {
        
    }
}