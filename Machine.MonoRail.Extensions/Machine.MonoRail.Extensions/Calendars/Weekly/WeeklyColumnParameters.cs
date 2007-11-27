using System;

namespace Machine.MonoRail.Extensions.Calendars.Weekly
{
  public class WeeklyColumnParameters
  {
    private readonly DateTime _date;

    public DateTime Date
    {
      get { return _date; }
    }

    public WeeklyColumnParameters(DateTime date)
    {
      _date = date;
    }
  }
}