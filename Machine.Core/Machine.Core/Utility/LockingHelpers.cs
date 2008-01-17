using System;
using System.Collections.Generic;
using System.Threading;

namespace Machine.Core.Utility
{
  public static class RWLock
  {
    public static IDisposable AsWriter(ReaderWriterLock theLock)
    {
      theLock.AcquireWriterLock(Timeout.Infinite);
      return new RWLockWrapper(theLock);
    }

    public static IDisposable AsReader(ReaderWriterLock theLock)
    {
      theLock.AcquireReaderLock(Timeout.Infinite);
      return new RWLockWrapper(theLock);
    }
  }
  public class RWLockWrapper : IDisposable
  {
    private readonly ReaderWriterLock _readerWriterLock;

    public RWLockWrapper(ReaderWriterLock readerWriterLock)
    {
      _readerWriterLock = readerWriterLock;
    }

    public void Dispose()
    {
      _readerWriterLock.ReleaseLock();
    }
  }
}
