using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

using MvcContrib.ViewFactories;

namespace Machine.MsMvc
{
  public class DemultiplexingViewEngine : IViewEngine
  {
    private readonly IViewSourceLoader _viewSourceLoader;
    private readonly IAvailableViewEnginesProvider _availableViewEngines;
    private readonly Dictionary<string, IViewEngine> _cache = new Dictionary<string, IViewEngine>();

    public DemultiplexingViewEngine(IViewSourceLoader viewSourceLoader, IAvailableViewEnginesProvider availableViewEngines)
    {
      _viewSourceLoader = viewSourceLoader;
      _availableViewEngines = availableViewEngines;
    }

    #region IViewEngine Members
    public void RenderView(ViewContext viewContext)
    {
      /* I found this code elsewhere, pretty eh? */
      string controller = (string)viewContext.RouteData.Values["controller"];
      string viewKey = controller + "/" + viewContext.ViewName;
      if (_cache.ContainsKey(viewKey))
      {
        _cache[viewKey].RenderView(viewContext);
        return;
      }
      foreach (AvailableViewEngine engine in _availableViewEngines.GetAvailableViewEngines())
      {
        string viewPath = viewKey + "." + engine.Extension;
        if (_viewSourceLoader.HasView(viewPath))
        {
          _cache[viewKey] = engine.ViewEngine;
          engine.ViewEngine.RenderView(viewContext);
          return;
        }
      }
      throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Couldn't find the template for {0}.", new object[] { viewKey }));
    }
    #endregion
  }
}