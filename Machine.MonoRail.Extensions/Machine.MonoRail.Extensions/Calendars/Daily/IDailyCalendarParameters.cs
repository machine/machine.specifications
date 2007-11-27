using System;

namespace Machine.MonoRail.Extensions.Calendars.Daily
{
  public interface IDailyCalendarParameters : ICalendarParameters
  {
    TimeSpan StartTime
    {
      get; set;
    }
    TimeSpan EndTime
    {
      get; set;
    }
    TimeSpan MinorStep
    {
      get; set;
    }
    TimeSpan MajorStep
    {
      get; set;
    }
    DailyCalendarCellParameters Cell
    {
      set;
    }
  }
}