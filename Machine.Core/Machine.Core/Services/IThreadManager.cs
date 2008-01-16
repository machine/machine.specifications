using System;
using System.Collections.Generic;
using System.Threading;

namespace Machine.Core.Services
{
  public interface IThreadManager
  {
    IThread CreateThread(ThreadStart start);
  }
  public interface IThread
  {
    bool IsAlive
    {
      get;
    }
    void Start();
    void Join();
  }
}
