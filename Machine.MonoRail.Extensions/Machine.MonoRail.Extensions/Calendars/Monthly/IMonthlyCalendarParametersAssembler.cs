using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public interface IMonthlyCalendarParametersAssembler
  {
    IMonthlyCalendarParameters AssembleMonthlyParameters(DateTime selectedDate);
  }
}