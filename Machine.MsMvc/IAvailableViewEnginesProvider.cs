using System.Collections.Generic;
using System.Web.Mvc;

namespace Machine.MsMvc
{
  public interface IAvailableViewEnginesProvider
  {
    void AddViewEngine(string extension, IViewEngine viewEngine);
    IList<AvailableViewEngine> GetAvailableViewEngines();
  }
  public class AvailableViewEngine
  {
    private readonly string _extension;
    private readonly IViewEngine _viewEngine;

    public string Extension
    {
      get { return _extension; }
    }

    public IViewEngine ViewEngine
    {
      get { return _viewEngine; }
    }

    public AvailableViewEngine(string extension, IViewEngine viewEngine)
    {
      _extension = extension;
      _viewEngine = viewEngine;
    }
  }
}