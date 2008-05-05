using System;
using System.Collections.Generic;

using Machine.Container.Services;

namespace Machine.Container.Model
{
  public class ResolvedServiceEntry
  {
    private readonly ServiceEntry _serviceEntry;
    private readonly IActivator _activator;

    public ServiceEntry ServiceEntry
    {
      get { return _serviceEntry; }
    }

    public IActivator Activator
    {
      get { return _activator; }
    }

    public ResolvedServiceEntry(ServiceEntry serviceEntry, IActivator activator)
    {
      _serviceEntry = serviceEntry;
      _activator = activator;
    }

    public override string ToString()
    {
      return String.Format("ResolvedEntry<{0}, {1}>", _serviceEntry, _activator);
    }
  }
}