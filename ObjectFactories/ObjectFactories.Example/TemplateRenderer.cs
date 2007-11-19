using System;
using System.Collections.Generic;

namespace ObjectFactories.Example
{
  public class TemplateRenderer
  {
    private readonly string _formatting;

    public TemplateRenderer()
    {
      throw new ObjectFactoryException();
    }

    public TemplateRenderer(string formatting)
    {
      _formatting = formatting;
    }

    public string Render(string template)
    {
      return String.Format(_formatting, template);
    }
  }
}
