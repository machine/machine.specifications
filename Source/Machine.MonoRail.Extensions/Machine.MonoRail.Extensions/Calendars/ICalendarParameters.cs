using System;
using System.Collections.Generic;

namespace Machine.MonoRail.Extensions.Calendars
{
  public interface ICalendarParameters
  {
    DateTime SelectedDate
    {
      get;
      set;
    }
  }
}