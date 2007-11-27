using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public interface IMonthlyCalendarParameters : ICalendarParameters
  {
    DateTime FirstVisibleDate
    {
      set;
    }
    DateTime LastVisibleDate
    {
      set;
    }
    MonthlyNavigationParameters Navigation
    {
      set;
    }
    MonthlyColumnParameters Column
    {
      set;
    }
    CalendarCellParameters Cell
    {
      set;
    }
  }
}