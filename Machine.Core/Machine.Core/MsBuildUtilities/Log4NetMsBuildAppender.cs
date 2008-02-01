using System;
using System.Collections.Generic;

using log4net.Appender;
using log4net.Core;
using log4net.Layout;

using Microsoft.Build.Utilities;

namespace Machine.Core.MsBuildUtilities
{
  public class Log4NetMsBuildAppender : AppenderSkeleton
  {
    #region Member Data
    private readonly TaskLoggingHelper _loggingHelper;
    #endregion

    #region Log4NetMsBuildAppender()
    public Log4NetMsBuildAppender(TaskLoggingHelper loggingHelper, ILayout layout)
    {
      _loggingHelper = loggingHelper;
      this.Layout = layout;
    }
    #endregion

    #region AppenderSkeleton Members
    protected override void Append(LoggingEvent loggingEvent)
    {
      if (loggingEvent.Level == Level.Warn)
      {
        _loggingHelper.LogWarning(RenderLoggingEvent(loggingEvent));
      }
      else if (loggingEvent.Level == Level.Fatal || loggingEvent.Level == Level.Error || loggingEvent.Level == Level.Emergency || loggingEvent.Level == Level.Critical)
      {
        _loggingHelper.LogError(RenderLoggingEvent(loggingEvent));
      }
      else
      {
        _loggingHelper.LogMessage(RenderLoggingEvent(loggingEvent));
      }
    }
    #endregion
  }
}
