using System;
using System.Globalization;

namespace Machine.MonoRail.Extensions.Calendars
{
  public abstract class AbstractCalendarParametersAssembler
  {
    protected readonly Calendar _calendar = new GregorianCalendar();

    protected virtual bool IsSameDay(DateTime day1, DateTime day2)
    {
      return day1.Date == day2.Date;
    }

    protected virtual bool IsSameMonth(DateTime day1, DateTime day2)
    {
      return day1.Year == day2.Year && day1.Month == day2.Month;
    }

    protected virtual bool IsToday(DateTime day)
    {
      return day.Date == DateTime.Now.Date;
    }

    protected virtual DateTime GetFirstDayOfMonth(DateTime day)
    {
      return new DateTime(day.Year, day.Month, 1);
    }

    protected virtual DateTime GetLastDayOfMonth(DateTime day)
    {
      return new DateTime(day.Year, day.Month, _calendar.GetDaysInMonth(day.Year, day.Month));
    }

    protected virtual DateTime GetFirstDayOfWeek(DateTime day)
    {
      while (day.DayOfWeek != DayOfWeek.Sunday)
      {
        day = day.AddDays(-1);
      }
      return day;
    }

    protected virtual DateTime GetLastDayOfWeek(DateTime day)
    {
      while (day.DayOfWeek != DayOfWeek.Saturday)
      {
        day = day.AddDays(1);
      }
      return day;
    }

    protected virtual DateTime GetDateOneMonthAway(DateTime day, bool forward)
    {
      int year = day.Year;
      int month = day.Month + (forward ? 1 : -1);
      int date = day.Day;
      if (month > 12)
      {
        month = 1;
        year++;
      }
      if (month < 1)
      {
        month = 12;
        year--;
      }
      if (date > _calendar.GetDaysInMonth(year, month))
      {
        date = _calendar.GetDaysInMonth(year, month);
      }
      return new DateTime(year, month, date);
    }

    protected virtual DateTime GetDateOneWeekAway(DateTime day, bool forward)
    {
      return day.AddDays(forward ? 7 : -7);
    }

    protected virtual DateTime GetStartOfNextWeek(DateTime day)
    {
      return GetDateOneWeekAway(day, true).AddDays(1);
    }

    protected virtual DateTime GetStartOfPreviousWeek(DateTime day)
    {
      return GetDateOneWeekAway(day, false).AddDays(-1);
    }
  }
}
