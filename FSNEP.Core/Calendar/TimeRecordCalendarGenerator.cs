using System.Collections.Generic;
using FSNEP.Core.Domain;

namespace FSNEP.Core.Calendar
{
    public interface ITimeRecordCalendarGenerator
    {
        IList<TimeRecordCalendarDay> GenerateCalendar(TimeRecord timeRecord);
    }

    public class TimeRecordCalendarGenerator : ITimeRecordCalendarGenerator
    {
        public IList<TimeRecordCalendarDay> GenerateCalendar(TimeRecord timeRecord)
        {
            // Create the calendar object, and populate
            var calendar = new TimeRecordCalendarMonth(timeRecord.Year, timeRecord.Month);
            calendar.Populate(timeRecord);

            return calendar.Days;
        }
    }
}