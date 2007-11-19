using System;
using ObjectFactories.Services;

namespace ObjectFactories.Example
{
  public class TemplateRendererFactory : IObjectFactory<TemplateRenderer>
  {
    #region IObjectFactory<TemplateRenderer> Members
    public TemplateRenderer Create()
    {
      Console.WriteLine("Creating TemplateRenderer...");
      return new TemplateRenderer("~{0}~");
    }
    #endregion
  }
}