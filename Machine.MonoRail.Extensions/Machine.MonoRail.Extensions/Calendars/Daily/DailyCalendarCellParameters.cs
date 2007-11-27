using System;

namespace Machine.MonoRail.Extensions.Calendars.Daily
{
  public class DailyCalendarCellParameters
  {
    private DateTime _dateAndTime;
    private bool _isToday;
    private bool _isMajorStep;

    public DateTime DateAndTime
    {
      get { return _dateAndTime; }
      set { _dateAndTime = value; }
    }

    public bool IsToday
    {
      get { return _isToday; }
      set { _isToday = value; }
    }

    public bool IsMajorStep
    {
      get { return _isMajorStep; }
      set { _isMajorStep = value; }
    }

    public DailyCalendarCellParameters(DateTime dateAndTime, bool isToday, bool isMajorStep)
    {
      _dateAndTime = dateAndTime;
      _isToday = isToday;
      _isMajorStep = isMajorStep;
    }
  }
}