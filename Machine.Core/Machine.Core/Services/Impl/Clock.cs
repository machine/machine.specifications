using System;
using System.Collections.Generic;

namespace Machine.Core.Services.Impl
{
  [Castle.Core.Transient]
  public class Clock : IClock
  {
    #region IClock Members
    public DateTime CurrentTime
    {
      get { return DateTime.Now; }
    }
    #endregion
  }
}
