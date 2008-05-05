using System.Collections.Generic;
using System.Web.Mvc;

namespace Machine.MsMvc
{
  public class DefaultAvailableViewEngines : IAvailableViewEnginesProvider
  {
    private readonly List<AvailableViewEngine> _availableViewEngines = new List<AvailableViewEngine>();

    public void AddViewEngine(string extension, IViewEngine viewEngine)
    {
      _availableViewEngines.Add(new AvailableViewEngine(extension, viewEngine));
    }

    public IList<AvailableViewEngine> GetAvailableViewEngines()
    {
      return _availableViewEngines;
    }
  }
}