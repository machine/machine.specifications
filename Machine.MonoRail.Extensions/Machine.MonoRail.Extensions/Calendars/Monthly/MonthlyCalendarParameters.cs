using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public class MonthlyCalendarParameters : IMonthlyCalendarParameters
  {
    private DateTime _selectedDate;
    private DateTime _firstVisibleDate;
    private DateTime _lastVisibleDate;

    public DateTime SelectedDate
    {
      get { return _selectedDate; }
      set { _selectedDate = value; }
    }

    public DateTime FirstVisibleDate
    {
      get { return _firstVisibleDate; }
      set { _firstVisibleDate = value; }
    }

    public DateTime LastVisibleDate
    {
      get { return _lastVisibleDate; }
      set { _lastVisibleDate = value; }
    }

    public MonthlyCalendarParameters(DateTime selectedDate, DateTime firstVisibleDate, DateTime lastVisibleDate)
    {
      _selectedDate = selectedDate;
      _firstVisibleDate = firstVisibleDate;
      _lastVisibleDate = lastVisibleDate;
    }
  }
}
