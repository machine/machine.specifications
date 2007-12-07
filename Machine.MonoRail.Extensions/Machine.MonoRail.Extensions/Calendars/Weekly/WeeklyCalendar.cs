using System;

using Machine.MonoRail.Extensions.Calendars.Monthly;

namespace Machine.MonoRail.Extensions.Calendars.Weekly
{
  public class WeeklyCalendar : AbstractCalendarViewComponent
  {
    #region Member Data
    private const string DaySectionName = "Day";
    private const string ColumnHeaderSectionName = "ColumnHeader";
    private const string NavigationSectionName = "Navigation";
    private const string BeginSectionName = "Begin";
    private const string EndSectionName = "End";
    private const string RowBeginSectionName = "BeginRow";
    private const string RowEndSectionName = "EndRow";
    private IWeeklyCalendarParameters _parameters;
    #endregion

    #region Methods
    public override bool SupportsSection(string name)
    {
      return 
        name == DaySectionName || name == ColumnHeaderSectionName || 
        name == NavigationSectionName || name == BeginSectionName || name == EndSectionName ||
        name == RowBeginSectionName || name == RowEndSectionName;
    }

    public override void Initialize()
    {
      _parameters = GetView<IWeeklyCalendarParameters>();
      base.Initialize();
    }

    public override void Render()
    {
      DateTime selectedDay = _parameters.SelectedDate;
      DateTime firstDayOfWeek = _parameters.StartFromToday ? selectedDay : GetFirstDayOfWeek(selectedDay);
      DateTime firstVisibleDay = firstDayOfWeek;
      DateTime lastVisibleDay = GetDateOneWeekAway(firstVisibleDay, true);

      _parameters.FirstVisibleDate = firstVisibleDay;
      _parameters.LastVisibleDate = lastVisibleDay;
      _parameters.Navigation = new WeeklyNavigationParameters(GetStartOfPreviousWeek(firstVisibleDay), GetStartOfNextWeek(firstVisibleDay));

      RenderSection(BeginSectionName);
      RenderSection(NavigationSectionName);
      RenderSection(RowBeginSectionName);
      RenderColumns(firstVisibleDay, lastVisibleDay, selectedDay);
      RenderSection(RowEndSectionName);
      RenderSection(RowBeginSectionName);
      RenderDays(firstVisibleDay, lastVisibleDay, selectedDay);
      RenderSection(RowEndSectionName);
      RenderSection(EndSectionName);
    }

    protected virtual void RenderColumns(DateTime firstVisibleDay, DateTime lastVisibleDay, DateTime selectedDay)
    {
      DateTime currentDay = firstVisibleDay;
      while (currentDay <= lastVisibleDay)
      {
        _parameters.Column = new WeeklyColumnParameters(currentDay);
        RenderSection(ColumnHeaderSectionName);
        _parameters.Column = null;
        currentDay = currentDay.AddDays(1);
      }
    }

    protected virtual void RenderDays(DateTime firstVisibleDay, DateTime lastVisibleDay, DateTime selectedDay)
    {
      DateTime currentDay = firstVisibleDay;
      while (currentDay <= lastVisibleDay)
      {
        _parameters.Cell = new CalendarCellParameters(currentDay, IsSameDay(currentDay, selectedDay), IsSameMonth(currentDay, selectedDay), IsToday(currentDay));
        RenderSection(DaySectionName);
        _parameters.Cell = null;
        currentDay = currentDay.AddDays(1);
      }
    }
    #endregion
  }
}