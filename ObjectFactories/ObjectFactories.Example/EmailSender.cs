using System;
using System.Collections.Generic;

namespace ObjectFactories.Example
{
  public class EmailSender
  {
    public void TellEveryone(string message)
    {
      TemplateRenderer renderer = new TemplateRenderer();
      Console.WriteLine(renderer.Render(message));
    }
  }
}
