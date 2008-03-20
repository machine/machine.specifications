using System;

namespace Machine.Core.Aspects
{
  public class BackgroundInvokeAttribute : Attribute
  {
    private readonly bool _onlyIfInUiThread;

    public bool OnlyIfInUiThread
    {
      get { return _onlyIfInUiThread; }
    }

    public BackgroundInvokeAttribute(bool onlyIfInUiThread)
    {
      _onlyIfInUiThread = onlyIfInUiThread;
    }

    public BackgroundInvokeAttribute()
    {
    }
  }
}