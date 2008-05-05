using System;

using Machine.MonoRail.Extensions.Calendars.Monthly;

namespace Machine.MonoRail.Extensions.Calendars.Weekly
{
  public interface IWeeklyCalendarParameters : ICalendarParameters
  {
    bool StartFromToday
    {
      get;
      set;
    }
    DateTime FirstVisibleDate
    {
      set;
    }
    DateTime LastVisibleDate
    {
      set;
    }
    WeeklyNavigationParameters Navigation
    {
      set;
    }
    WeeklyColumnParameters Column
    {
      set;
    }
    CalendarCellParameters Cell
    {
      set;
    }
  }
}