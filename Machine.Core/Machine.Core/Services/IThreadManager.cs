using System;
using System.Collections.Generic;
using System.Threading;

namespace Machine.Core.Services
{
  public interface IRunnable
  {
    void Run();
  }
  public interface IThreadManager
  {
    IThread CreateThread(ThreadStart start);
    IThread CreateThread(IRunnable runnable);
    ITimer StartTimer(TimeSpan delay, TimeSpan period, IRunnable runnable);
    void Sleep(TimeSpan duration);
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
  public interface ITimer
  {
    void Stop();
  }
}
