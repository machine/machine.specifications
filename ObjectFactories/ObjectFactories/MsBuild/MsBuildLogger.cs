using System;
using System.Collections.Generic;

using Microsoft.Build.Utilities;

using ObjectFactories.Services;

namespace ObjectFactories.MsBuild
{
  public class MsBuildLogger : ILog
  {
    #region Member Data
    private readonly TaskLoggingHelper _logging;
    #endregion

    #region MsBuildLogger()
    public MsBuildLogger(TaskLoggingHelper logging)
    {
      _logging = logging;
    }
    #endregion

    #region ILogger Members
    public void Log(string format, params object[] arguments)
    {
      _logging.LogMessage(format, arguments);
    }
    #endregion
  }
}
