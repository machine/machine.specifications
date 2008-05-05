using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public interface IMonthlyCalendarParameters : ICalendarParameters
  {
    DateTime FirstVisibleDate
    {
      set;
      get;
    }
    DateTime LastVisibleDate
    {
      set;
      get;
    }
  }
}