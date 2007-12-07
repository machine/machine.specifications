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
    private IMonthlyCalendarView _view;
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
      _view = GetView<IMonthlyCalendarView>();
      base.Initialize();
    }

    public override void Render()
    {
      DateTime selectedDay = _parameters.SelectedDate;

      _view.SelectedDate = _parameters.SelectedDate;
      _view.FirstVisibleDate = _parameters.FirstVisibleDate;
      _view.LastVisibleDate = _parameters.LastVisibleDate;
      _view.Navigation = new MonthlyNavigationParameters(GetDateOneMonthAway(selectedDay, false), GetDateOneMonthAway(selectedDay, true));

      RenderSection(BeginSectionName);
      RenderSection(NavigationSectionName);
      RenderSection(RowBeginSectionName);
      for (int i = 0; i < 7; ++i )
      {
        _view.Column = new MonthlyColumnParameters((DayOfWeek)i);
        RenderSection(ColumnHeaderSectionName);
        _view.Column = null;
      }
      RenderSection(RowEndSectionName);
      RenderDays(_parameters.FirstVisibleDate, _parameters.LastVisibleDate, selectedDay);
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
        _view.Cell = new CalendarCellParameters(currentDay, IsSameDay(currentDay, selectedDay), IsSameMonth(currentDay, selectedDay), IsToday(currentDay));
        RenderSection(DaySectionName);
        _view.Cell = null;
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