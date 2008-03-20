using System;
using System.Collections.Generic;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IServiceEntryResolver
  {
    ServiceEntry CreateEntryIfMissing(Type serviceType);
    ServiceEntry CreateEntryIfMissing(Type serviceType, Type implementationType);
    ResolvedServiceEntry ResolveEntry(ICreationServices services, Type serviceType);
  }
}
