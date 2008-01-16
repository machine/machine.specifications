using System;
using System.Collections.Generic;

namespace Machine.Core.Services
{
  public interface IClock
  {
    DateTime CurrentTime
    {
      get;
    }
  }
}
