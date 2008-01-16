using System;
using System.Collections.Generic;
using System.Threading;

using NUnit.Framework;

namespace Machine.Core.Services.Impl
{
  [TestFixture]
  public class ThreadManagerTests : StandardFixture<ThreadManager>
  {
    private bool _invoked;
    private ManualResetEvent _event;

    public override ThreadManager Create()
    {
      _event = new ManualResetEvent(false);
      return new ThreadManager();
    }

    [Test]
    public void CreateThread_Always_IsNewThreadNotStarted()
    {
      using (_mocks.Record())
      {
      }
      IThread thread = _target.CreateThread(MyThreadMain);
      Thread.Sleep(200);
      Assert.IsFalse(_invoked);
      thread.Start();
      _event.WaitOne(TimeSpan.FromSeconds(10), false);
      thread.Join();
      Assert.IsTrue(_invoked);
    }

    private void MyThreadMain()
    {
      _invoked = true;
      _event.Set();
    }
  }
}