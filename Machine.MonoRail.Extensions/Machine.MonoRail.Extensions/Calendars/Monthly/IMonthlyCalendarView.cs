using System;
using System.Collections.Generic;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public interface IMonthlyCalendarView : IMonthlyCalendarParameters
  {
    MonthlyNavigationParameters Navigation
    {
      set;
    }
    MonthlyColumnParameters Column
    {
      set;
    }
    CalendarCellParameters Cell
    {
      set;
    }
  }
}
