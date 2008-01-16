using System;
using System.Collections.Generic;
using System.Threading;

namespace Machine.Core.Services.Impl
{
  public class ThreadManager : IThreadManager
  {
    #region IThreadManager Members
    public IThread CreateThread(ThreadStart start)
    {
      return new DotNotThread(new Thread(start));
    }
    #endregion
  }
  public class DotNotThread : IThread
  {
    private readonly Thread _thread;

    public Thread SystemThread
    {
      get { return _thread; }
    }

    public bool IsAlive
    {
      get { return _thread.IsAlive; }
    }

    public DotNotThread(Thread thread)
    {
      _thread = thread;
    }

    public void Start()
    {
      _thread.Start();
    }

    public void Join()
    {
      _thread.Join();
    }
  }
}
