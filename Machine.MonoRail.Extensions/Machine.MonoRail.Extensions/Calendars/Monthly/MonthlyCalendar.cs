using System;

namespace Machine.MonoRail.Extensions.Calendars.Monthly
{
  public class MonthlyCalendar : AbstractCalendarViewComponent
  {
    #region Member Data
    private const string DaySectionName = "Day";
    private const string ColumnHeaderSectionName = "ColumnHeader";
    private const string RowHeaderSectionName = "RowHeader";
    private const string NavigationSectionName = "Navigation";
    private const string BeginSectionName = "Begin";
    private const string EndSectionName = "End";
    private const string RowBeginSectionName = "BeginRow";
    private const string RowEndSectionName = "EndRow";
    private IMonthlyCalendarParameters _parameters;
    #endregion

    #region Methods
    public override bool SupportsSection(string name)
    {
      return
        name == DaySectionName || name == ColumnHeaderSectionName || name == RowHeaderSectionName ||
        name == NavigationSectionName || name == BeginSectionName || name == EndSectionName ||
        name == RowBeginSectionName || name == RowEndSectionName;
    }

    public override void Initialize()
    {
      _parameters = GetParameters<IMonthlyCalendarParameters>();
      base.Initialize();
    }

    public override void Render()
    {
      DateTime selectedDay = _parameters.SelectedDate;
      DateTime firstDayOfMonth = GetFirstDayOfMonth(selectedDay);
      DateTime firstVisibleDay = GetFirstDayOfWeek(firstDayOfMonth);
      DateTime lastDayOfMonth = GetLastDayOfMonth(selectedDay);
      DateTime lastVisibleDay = GetLastDayOfWeek(lastDayOfMonth);

      _parameters.FirstVisibleDate = firstVisibleDay;
      _parameters.LastVisibleDate = lastVisibleDay;
      _parameters.Navigation = new MonthlyNavigationParameters(GetDateOneMonthAway(selectedDay, false), GetDateOneMonthAway(selectedDay, true));

      RenderSection(BeginSectionName);
      RenderSection(NavigationSectionName);
      RenderSection(RowBeginSectionName);
      for (int i = 0; i < 7; ++i )
      {
        _parameters.Column = new MonthlyColumnParameters((DayOfWeek)i);
        RenderSection(ColumnHeaderSectionName);
        _parameters.Column = null;
      }
      RenderSection(RowEndSectionName);
      RenderDays(firstVisibleDay, lastVisibleDay, selectedDay);
      RenderSection(EndSectionName);
    }

    protected virtual void RenderDays(DateTime firstVisibleDay, DateTime lastVisibleDay, DateTime selectedDay)
    {
      DateTime currentDay = firstVisibleDay;
      while (currentDay <= lastVisibleDay)
      {
        if (currentDay.DayOfWeek == DayOfWeek.Sunday)
        {
          RenderSection(RowBeginSectionName);
        }
        _parameters.Cell = new CalendarCellParameters(currentDay, IsSameDay(currentDay, selectedDay), IsSameMonth(currentDay, selectedDay), IsToday(currentDay));
        RenderSection(DaySectionName);
        _parameters.Cell = null;
        if (currentDay.DayOfWeek == DayOfWeek.Saturday)
        {
          RenderSection(RowEndSectionName);
        }
        currentDay = currentDay.AddDays(1);
      }
    }
    #endregion
  }
}