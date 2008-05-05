using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public class MonthlyColumnParameters
  {
    private readonly DayOfWeek _day;

    public DayOfWeek DayOfWeek
    {
      get { return _day; }
    }

    public MonthlyColumnParameters(DayOfWeek day)
    {
      _day = day;
    }
  }
}