using System;
using FSNEP.Core.Calendar;
using FSNEP.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSNEP.Tests.Core.GeneralTests
{
    [TestClass]
    public class TimeRecordCalendarGeneratorTests
    {
        /// <summary>
        /// Time record calendar generator returns correct days for all months between 2000 and 2010 inclusive.
        /// </summary>
        [TestMethod]
        public void TimeRecordCalendarGeneratorReturnsCorrectDays()
        {
            for (int year = 2000; year <= 2010; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    TimeRecord timeRecord = CreateValidTimeRecord(month, year);
                    var timeRecordCalendarGenerator = new TimeRecordCalendarGenerator();

                    var calendarDays = timeRecordCalendarGenerator.GenerateCalendar(timeRecord);
                    Assert.IsNotNull(calendarDays, "1) Year: " + year + " Month:" + month);
                    foreach (var day in calendarDays)
                    {
                        var validDate = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day);
                        Assert.AreEqual(validDate.Year, day.Date.Year, "2) Year: " + year + " Month:" + month);
                        Assert.AreEqual(validDate.Month, day.Date.Month, "3) Year: " + year + " Month:" + month);
                        Assert.AreEqual(validDate.Month, day.Month, "4) Year: " + year + " Month:" + month);
                        Assert.AreEqual(validDate.Day, day.Date.Day, "5) Year: " + year + " Month:" + month);
                        Assert.AreEqual(validDate.Day, day.Day, "6) Year: " + year + " Month:" + month);
                    }
                    int numberofDaysInMonth = DateTime.DaysInMonth(year, month);
                    var startPossition = (int)new DateTime(year, month, 1).DayOfWeek;
                    var endPossition = (int)new DateTime(year, month, numberofDaysInMonth).DayOfWeek;


                    int counter = 0;
                    //Validate that the month we want is where we expect it
                    for (int i = startPossition; i <= numberofDaysInMonth + startPossition - 1; i++)
                    {
                        counter++;
                        Assert.AreEqual(month, calendarDays[i].Month, "7) Year: " + year + " Month:" + month + " Position:" + i);
                        Assert.AreEqual(i + 1 - startPossition, calendarDays[i].Day, "8) Year: " + year + " Month:" + month + " Position:" + i);
                        Assert.AreEqual(year, calendarDays[i].Date.Year, "9) Year: " + year + " Month:" + month + " Position:" + i);
                        Assert.IsTrue(calendarDays[i].IsActive, "9a) Year: " + year + " Month:" + month + " Position:" + i);
                    }
                    Assert.AreEqual(counter, numberofDaysInMonth, "9b) Didn't process the right number of days. Year: " + year + " Month:" + month);

                    //validate that the previous month's days show as we expect
                    var tempDate = new DateTime(year, month, 1).AddDays(-1);
                    int previousmonthDayOffset = 
                        DateTime.DaysInMonth(tempDate.Year, tempDate.Month) - startPossition + 1;
                    for (int i = 0; i < startPossition; i++)
                    {
                        Assert.AreEqual(tempDate.Month, calendarDays[i].Month, "10) Year: " + year + " Month:" + month + " Position:" + i); //previous month
                        Assert.AreEqual(tempDate.Year, calendarDays[i].Date.Year, "11) Year: " + year + " Month:" + month + " Position:" + i); //Previous year
                        Assert.AreEqual(previousmonthDayOffset + i, calendarDays[i].Day, "12) Year: " + year + " Month:" + month + " Position:" + i);
                        Assert.IsFalse(calendarDays[i].IsActive, "12a) Year: " + year + " Month:" + month + " Position:" + i);
                    }

                    if (endPossition != 6)
                    {
                        int tempInt = 0;
                        int nextMonth = new DateTime(year, month, 1).AddMonths(1).Month;
                        int nextYear = new DateTime(year, month, 1).AddMonths(1).Year;
                        Assert.AreEqual(numberofDaysInMonth + startPossition + (6 - endPossition), calendarDays.Count, "13) Year: " + year + " Month:" + month);
                        for (int i = numberofDaysInMonth + startPossition; i < calendarDays.Count; i++)
                        {
                            tempInt++;
                            Assert.AreEqual(tempInt, calendarDays[i].Day, "14) Year: " + year + " Month:" + month + " Position:" + i);
                            Assert.AreEqual(nextMonth, calendarDays[i].Month, "15) Year: " + year + " Month:" + month + " Position:" + i);
                            Assert.AreEqual(nextYear, calendarDays[i].Date.Year, "16) Year: " + year + " Month:" + month + " Position:" + i);
                            Assert.IsFalse(calendarDays[i].IsActive, "17) Year: " + year + " Month:" + month + " Position:" + i);
                        }
                    }
                    else
                    {
                        
                        Assert.AreEqual(numberofDaysInMonth + startPossition, calendarDays.Count, "Year: " + year + " Month:" + month);
                    }
                }
            }
        }

        #region Commented out non-looping tests
        //[TestMethod]
        //public void TimeRecordCalendarGeneratorReturnsCorrectDays()
        //{
        //    TimeRecord timeRecord = CreateValidTimeRecord(1, 2000);
        //    var timeRecordCalendarGenerator = new TimeRecordCalendarGenerator();

        //    var calendarDays = timeRecordCalendarGenerator.GenerateCalendar(timeRecord);
        //    Assert.IsNotNull(calendarDays);
        //    foreach (var day in calendarDays)
        //    {
        //        var validDate = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day);
        //        Assert.AreEqual(validDate.Year, day.Date.Year);                
        //        Assert.AreEqual(validDate.Month, day.Date.Month);
        //        Assert.AreEqual(validDate.Month, day.Month);
        //        Assert.AreEqual(validDate.Day, day.Date.Day);
        //        Assert.AreEqual(validDate.Day, day.Day);
        //    }
        //    int numberofDaysInMonth = DateTime.DaysInMonth(2000, 1);
        //    var startPossition = (int)new DateTime(2000, 1, 1).DayOfWeek;
        //    var endPossition = (int)new DateTime(2000, 1, numberofDaysInMonth).DayOfWeek;

        //    //Validate that the month we want is where we expect it
        //    for (int i = startPossition; i <= numberofDaysInMonth; i++)
        //    {
        //        Assert.AreEqual(1, calendarDays[i].Month);
        //        Assert.AreEqual(i + 1 - startPossition, calendarDays[i].Day);
        //        Assert.AreEqual(2000, calendarDays[i].Date.Year);
        //    }

        //    //validate that the previous month's days show as we expect
        //    var tempDate = new DateTime(2000, 1, 1).AddDays(-1);
        //    int previousmonthDayOffset = DateTime.DaysInMonth(tempDate.Year, tempDate.Month) - startPossition + 1;
        //    for (int i = 0; i < startPossition; i++)
        //    {
        //        Assert.AreEqual(tempDate.Month, calendarDays[i].Month); //previous month
        //        Assert.AreEqual(tempDate.Year, calendarDays[i].Date.Year); //Previous year
        //        Assert.AreEqual(previousmonthDayOffset + i, calendarDays[i].Day);
        //    }

        //    if(endPossition != 6)
        //    {
        //        int tempInt = 0;
        //        int nextMonth = new DateTime(2000, 1, 1).AddMonths(1).Month;
        //        int nextYear = new DateTime(2000, 1, 1).AddMonths(1).Year;
        //        Assert.AreEqual(numberofDaysInMonth + startPossition + (6 - endPossition), calendarDays.Count);
        //        for (int i = numberofDaysInMonth + startPossition; i < calendarDays.Count; i++)
        //        {
        //            tempInt++;
        //            Assert.AreEqual(tempInt, calendarDays[i].Day);
        //            Assert.AreEqual(nextMonth, calendarDays[i].Month);
        //            Assert.AreEqual(nextYear, calendarDays[i].Date.Year);
        //        }
        //    }
        //    else
        //    {
        //        
        //        Assert.AreEqual(numberofDaysInMonth + startPossition, calendarDays.Count);
        //    }

        //}

        //[TestMethod]
        //public void TimeRecordCalendarGeneratorReturnsCorrectDays2()
        //{
        //    TimeRecord timeRecord = CreateValidTimeRecord(9, 2000);
        //    var timeRecordCalendarGenerator = new TimeRecordCalendarGenerator();

        //    var calendarDays = timeRecordCalendarGenerator.GenerateCalendar(timeRecord);
        //    Assert.IsNotNull(calendarDays);
        //    foreach (var day in calendarDays)
        //    {
        //        var validDate = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day);
        //        Assert.AreEqual(validDate.Year, day.Date.Year);
        //        Assert.AreEqual(validDate.Month, day.Date.Month);
        //        Assert.AreEqual(validDate.Month, day.Month);
        //        Assert.AreEqual(validDate.Day, day.Date.Day);
        //        Assert.AreEqual(validDate.Day, day.Day);
        //    }
        //    int numberofDaysInMonth = DateTime.DaysInMonth(2000, 9);
        //    var startPossition = (int)new DateTime(2000, 9, 1).DayOfWeek;
        //    var endPossition = (int)new DateTime(2000, 9, numberofDaysInMonth).DayOfWeek;

        //    //Validate that the month we want is where we expect it
        //    for (int i = startPossition; i <= numberofDaysInMonth; i++)
        //    {
        //        Assert.AreEqual(9, calendarDays[i].Month);
        //        Assert.AreEqual(i + 1 - startPossition, calendarDays[i].Day);
        //        Assert.AreEqual(2000, calendarDays[i].Date.Year);
        //    }

        //    //validate that the previous month's days show as we expect
        //    var tempDate = new DateTime(2000, 9, 1).AddDays(-1);
        //    int previousmonthDayOffset = DateTime.DaysInMonth(tempDate.Year, tempDate.Month) - startPossition + 1;
        //    for (int i = 0; i < startPossition; i++)
        //    {
        //        Assert.AreEqual(tempDate.Month, calendarDays[i].Month); //previous month
        //        Assert.AreEqual(tempDate.Year, calendarDays[i].Date.Year); //Previous year
        //        Assert.AreEqual(previousmonthDayOffset + i, calendarDays[i].Day);
        //    }

        //    if (endPossition != 6)
        //    {
        //        int tempInt = 0;
        //        int nextMonth = new DateTime(2000, 9, 1).AddMonths(1).Month;
        //        int nextYear = new DateTime(2000, 9, 1).AddMonths(1).Year;
        //        Assert.AreEqual(numberofDaysInMonth + startPossition + (6 - endPossition), calendarDays.Count);
        //        for (int i = numberofDaysInMonth + startPossition; i < calendarDays.Count; i++)
        //        {
        //            tempInt++;
        //            Assert.AreEqual(tempInt, calendarDays[i].Day);
        //            Assert.AreEqual(nextMonth, calendarDays[i].Month);
        //            Assert.AreEqual(nextYear, calendarDays[i].Date.Year);
        //        }
        //    }
        //    else
        //    {
        //        
        //        Assert.AreEqual(numberofDaysInMonth + startPossition, calendarDays.Count);
        //    }

        //}

        //[TestMethod]
        //public void TimeRecordCalendarGeneratorReturnsCorrectDays3()
        //{
        //    TimeRecord timeRecord = CreateValidTimeRecord(10, 2000);
        //    var timeRecordCalendarGenerator = new TimeRecordCalendarGenerator();

        //    var calendarDays = timeRecordCalendarGenerator.GenerateCalendar(timeRecord);
        //    Assert.IsNotNull(calendarDays);
        //    foreach (var day in calendarDays)
        //    {
        //        var validDate = new DateTime(day.Date.Year, day.Date.Month, day.Date.Day);
        //        Assert.AreEqual(validDate.Year, day.Date.Year);
        //        Assert.AreEqual(validDate.Month, day.Date.Month);
        //        Assert.AreEqual(validDate.Month, day.Month);
        //        Assert.AreEqual(validDate.Day, day.Date.Day);
        //        Assert.AreEqual(validDate.Day, day.Day);
        //    }
        //    int numberofDaysInMonth = DateTime.DaysInMonth(2000, 10);
        //    var startPossition = (int)new DateTime(2000, 10, 1).DayOfWeek;
        //    var endPossition = (int)new DateTime(2000, 10, numberofDaysInMonth).DayOfWeek;

        //    //Validate that the month we want is where we expect it
        //    int counter = 0;
        //    for (int i = startPossition; i <= numberofDaysInMonth + startPossition - 1; i++)
        //    {
        //        counter++;
        //        Assert.AreEqual(10, calendarDays[i].Month, "Position: " + i);
        //        Assert.AreEqual(i + 1 - startPossition, calendarDays[i].Day,"Position: " + i);
        //        Assert.AreEqual(2000, calendarDays[i].Date.Year, "Position: " + i);
        //    }
        //    Assert.AreEqual(counter, numberofDaysInMonth);
        //    //validate that the previous month's days show as we expect
        //    var tempDate = new DateTime(2000, 10, 1).AddDays(-1);
        //    int previousmonthDayOffset = DateTime.DaysInMonth(tempDate.Year, tempDate.Month) - startPossition + 1;
        //    for (int i = 0; i < startPossition; i++)
        //    {
        //        Assert.AreEqual(tempDate.Month, calendarDays[i].Month); //previous month
        //        Assert.AreEqual(tempDate.Year, calendarDays[i].Date.Year); //Previous year
        //        Assert.AreEqual(previousmonthDayOffset + i, calendarDays[i].Day);
        //    }

        //    if (endPossition != 6)
        //    {
        //        int tempInt = 0;
        //        int nextMonth = new DateTime(2000, 10, 1).AddMonths(1).Month;
        //        int nextYear = new DateTime(2000, 10, 1).AddMonths(1).Year;
        //        Assert.AreEqual(numberofDaysInMonth + startPossition + (6 - endPossition), calendarDays.Count);
        //        for (int i = numberofDaysInMonth + startPossition; i < calendarDays.Count; i++)
        //        {
        //            tempInt++;
        //            Assert.AreEqual(tempInt, calendarDays[i].Day);
        //            Assert.AreEqual(nextMonth, calendarDays[i].Month);
        //            Assert.AreEqual(nextYear, calendarDays[i].Date.Year);
        //        }
        //    }
        //    else
        //    {
        //        
        //        Assert.AreEqual(numberofDaysInMonth + startPossition, calendarDays.Count);
        //    }
        //}
        #endregion Commented out non-looping tests

        #region Helper Methods
        /// <summary>
        /// Creates the valid time record.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        private static TimeRecord CreateValidTimeRecord(int month, int year)
        {
            return new TimeRecord{Month = month, Year = year};
        }
        #endregion Helper Methods
    }
}
