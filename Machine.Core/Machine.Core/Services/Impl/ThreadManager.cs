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
      return new DotNotThread(new Thread(start), null);
    }

    public IThread CreateThread(IRunnable runnable)
    {
      return new DotNotThread(new Thread(RunnableHandler), runnable);
    }

    public ITimer StartTimer(TimeSpan delay, TimeSpan period, IRunnable runnable)
    {
      return new DotNetTimer(delay, period, runnable);
    }

    public void Sleep(TimeSpan duration)
    {
      Thread.Sleep(duration);
    }
    #endregion

    private static void RunnableHandler(object parameter)
    {
      ((IRunnable)parameter).Run();
    }
  }
  public class DotNetTimer : ITimer
  {
    #region Member Data
    private readonly Timer _timer;
    private readonly IRunnable _runnable;
    #endregion

    #region DotNetTimer()
    public DotNetTimer(TimeSpan delay, TimeSpan period, IRunnable runnable)
    {
      _runnable = runnable;
      _timer = new Timer(OnTimer, this, delay, period);
    }
    #endregion

    #region ITimer Members
    public void Stop()
    {
      _timer.Dispose();
    }
    #endregion

    private void OnTimer(object state)
    {
      _runnable.Run();
    }
  }
  public class DotNotThread : IThread
  {
    private readonly Thread _thread;
    private readonly object _parameter;

    public Thread SystemThread
    {
      get { return _thread; }
    }

    public bool IsAlive
    {
      get { return _thread.IsAlive; }
    }

    public DotNotThread(Thread thread, object parameter)
    {
      _thread = thread;
      _parameter = parameter;
    }

    public void Start()
    {
      if (_parameter != null)
      {
        _thread.Start(_parameter);
      }
      else
      {
        _thread.Start();
      }
    }

    public void Join()
    {
      _thread.Join();
    }
  }
}
