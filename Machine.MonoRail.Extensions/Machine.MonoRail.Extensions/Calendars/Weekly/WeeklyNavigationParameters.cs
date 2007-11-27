using System;

namespace Machine.MonoRail.Extensions.Calendars.Weekly
{
  public class WeeklyNavigationParameters
  {
    private readonly DateTime _dayNextWeek;
    private readonly DateTime _dayPreviousWeek;

    public DateTime DayNextWeek
    {
      get { return _dayNextWeek; }
    }

    public DateTime DayPreviousWeek
    {
      get { return _dayPreviousWeek; }
    }

    public WeeklyNavigationParameters(DateTime dayPreviousWeek, DateTime dayNextWeek)
    {
      _dayPreviousWeek = dayPreviousWeek;
      _dayNextWeek = dayNextWeek;
    }
  }
}