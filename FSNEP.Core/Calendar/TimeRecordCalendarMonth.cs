using System;
using System.Collections.Generic;
using FSNEP.Core.Domain;

namespace FSNEP.Core.Calendar
{
    public class TimeRecordCalendarMonth
    {
        // This object can be bound directly to a repeater.  It includes the blank objects needed when a month
        // begins in the middle of the week.
        private List<TimeRecordCalendarDay> _days;
        private DateTime _firstDay;
        private DateTime _lastDay;

        private readonly int _month;

        public TimeRecordCalendarMonth(int year, int month)
        {
            _month = month;

            _firstDay = new DateTime(year, month, 1);
            _lastDay = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);

            _days = new List<TimeRecordCalendarDay>();

            // Puts in the days for the first week when the month starts in the middle of the week

            // Calculate the first day of the calendar view (the sunday before the month starts)
            DateTime preceedingDays = _firstDay.AddDays(-ConvertDayOfWeek(_firstDay.DayOfWeek.ToString()));

            // iterate through until we reach the first of the month
            while (DateTime.Compare(_firstDay, preceedingDays) != 0)
            {
                // Add the day to the list
                _days.Add(new TimeRecordCalendarDay(preceedingDays, false));

                // Go to the next day
                preceedingDays = preceedingDays.AddDays(1);
            }

            // Populate the main days of the month
            for (int i = 0; i < _lastDay.Day; i++)
            {
                _days.Add(new TimeRecordCalendarDay(_firstDay.AddDays(i), true));
            }

            // Populate the lead out days of the month (to get to the saturday on the last week)
            // Calculates the remaining days of the final lead out week and adds those appropriately
            for (int i = 1; i < 7 - ConvertDayOfWeek(_lastDay.DayOfWeek.ToString()); i++)
            {
                _days.Add(new TimeRecordCalendarDay(_lastDay.AddDays(i), false));
            }
        }

        public List<TimeRecordCalendarDay> Days
        {
            get { return _days; }
            set { _days = value; }
        }

        public DateTime FirstDay
        {
            get { return _firstDay; }
            set { _firstDay = value; }
        }

        public DateTime LastDay
        {
            get { return _lastDay; }
            set { _lastDay = value; }
        }

        private static int ConvertDayOfWeek(string day)
        {
            switch (day)
            {
                case "Sunday":
                    return 0;
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Given a time sheet, it populates the day with the proper entries
        /// </summary>
        /// <param name="timeRecord"></param>
        public void Populate(TimeRecord timeRecord)
        {
            // Iterate through the timesheet entries
            foreach (TimeRecordEntry tes in timeRecord.Entries)
            {
                // Add the entry if it is not an adjustment
                if (tes.AdjustmentDate == null)
                  AddEntry(tes);
            }
        }

        private void AddEntry(TimeRecordEntry timeSheetEntry)
        {
            // First find the day matching the time sheet entry
            foreach (TimeRecordCalendarDay d in _days)
            {
                // Match the day and the month
                if (_month == d.Month && timeSheetEntry.Date == d.Day)
                {
                    // Add the entry into the day object
                    d.Entries.Add(timeSheetEntry);

                    // We can end the function now
                    return;
                }
            }
        }
    }
}