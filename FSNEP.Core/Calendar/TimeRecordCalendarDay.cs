using System;
using System.Collections.Generic;
using FSNEP.Core.Domain;

namespace FSNEP.Core.Calendar
{
    public class TimeRecordCalendarDay
    {
        public TimeRecordCalendarDay(DateTime currentDay, bool isActive)
        {
            Date = currentDay;
            Entries = new List<TimeRecordEntry>();

            IsActive = isActive;
        }

        public DateTime Date { get; set; }

        public bool IsCurrent
        {
            get
            {
                return Day == DateTime.Now.Day && Month == DateTime.Now.Month;
            }
        }

        public int Month
        {
            get
            {
                return Date.Month;
            }
        }
        public int Day
        {
            get
            {
                return Date.Day;
            }
        }

        public IList<TimeRecordEntry> Entries { get; set; }

        public bool IsActive { get; set; }
    }
}