using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  [Castle.Core.Singleton]
  public class MonthlyCalendarParametersAssembler : AbstractCalendarParametersAssembler, IMonthlyCalendarParametersAssembler
  {
    public IMonthlyCalendarParameters AssembleMonthlyParameters(DateTime selectedDate)
    {
      DateTime firstDayOfMonth = GetFirstDayOfMonth(selectedDate);
      DateTime firstVisibleDate = GetFirstDayOfWeek(firstDayOfMonth);
      DateTime lastDayOfMonth = GetLastDayOfMonth(selectedDate);
      DateTime lastVisibleDate = GetLastDayOfWeek(lastDayOfMonth);
      return new MonthlyCalendarParameters(selectedDate, firstVisibleDate, lastVisibleDate);
    }
  }
}
