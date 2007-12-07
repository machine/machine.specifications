using System;

namespace Machine.MonoRail.Extensions.Calendars.Daily
{
  public class DailyCalendar : AbstractCalendarViewComponent
  {
    #region Member Data
    private const string BeginSectionName = "Begin";
    private const string EndSectionName = "End";
    private const string CellSectionName = "Cell";
    private const string HeaderSectionName = "Header";
    private IDailyCalendarParameters _parameters;
    #endregion

    #region Methods
    public override bool SupportsSection(string name)
    {
      return name == BeginSectionName || name == EndSectionName || name == CellSectionName || name == HeaderSectionName;
    }

    public override void Initialize()
    {
      _parameters = GetView<IDailyCalendarParameters>();
      ValidateParameters();
      base.Initialize();
    }

    public override void Render()
    {
      DateTime selectedDay = _parameters.SelectedDate;
      RenderSection(BeginSectionName);
      RenderSection(HeaderSectionName);

      TimeSpan currentTime = _parameters.StartTime;
      while (currentTime < _parameters.EndTime)
      {
        DateTime dateAndTime = selectedDay + currentTime;
        _parameters.Cell = new DailyCalendarCellParameters(dateAndTime, IsToday(dateAndTime), IsMajorStep(currentTime));
        RenderSection(CellSectionName);
        _parameters.Cell = null;
        currentTime += _parameters.MinorStep;
      }

      RenderSection(EndSectionName);
    }

    private void ValidateParameters()
    {
      if (_parameters.MinorStep <= TimeSpan.Zero)
      {
        throw new ArgumentException("MinorStep");
      }
      if (_parameters.MajorStep <= TimeSpan.Zero)
      {
        throw new ArgumentException("MajorStep");
      }
      if (_parameters.StartTime <= TimeSpan.Zero)
      {
        _parameters.StartTime = TimeSpan.Zero;
      }
      if (_parameters.StartTime <= TimeSpan.Zero)
      {
        _parameters.EndTime = TimeSpan.FromDays(1.0);
      }
    }

    protected virtual bool IsMajorStep(TimeSpan time)
    {
      return (time.Ticks % _parameters.MajorStep.Ticks) == 0;
    }
    #endregion
  }
}