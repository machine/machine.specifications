using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public class MonthlyNavigationParameters
  {
    private readonly DateTime _dayNextMonth;
    private readonly DateTime _dayPreviousMonth;

    public DateTime DayNextMonth
    {
      get { return _dayNextMonth; }
    }

    public DateTime DayPreviousMonth
    {
      get { return _dayPreviousMonth; }
    }

    public MonthlyNavigationParameters(DateTime dayPreviousMonth, DateTime dayNextMonth)
    {
      _dayPreviousMonth = dayPreviousMonth;
      _dayNextMonth = dayNextMonth;
    }
  }
}