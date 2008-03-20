using System;
using System.Collections.Generic;
using System.Text;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ResolutionMessageBuilder
  {
    #region Member Data
    private readonly ServiceEntry _target;
    private readonly Stack<ServiceEntry> _progress;
    #endregion

    #region ResolutionMessageBuilder()
    public ResolutionMessageBuilder(ServiceEntry target, Stack<ServiceEntry> progress)
    {
      _target = target;
      _progress = progress;
    }
    #endregion

    #region Methods
    public static string BuildMessage(ServiceEntry target, Stack<ServiceEntry> progress)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Error resolving: " + target);
      foreach (ServiceEntry entry in progress)
      {
        sb.AppendLine(entry.ToString());
      }
      sb.AppendLine();
      sb.AppendLine("");
      return sb.ToString();
    }

    public override string ToString()
    {
      return BuildMessage(_target, _progress);
    }
    #endregion
  }
}
