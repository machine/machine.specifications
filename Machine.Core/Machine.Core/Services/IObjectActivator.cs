using System;
using System.Collections.Generic;

namespace Machine.Core.Services
{
  public interface IObjectActivator
  {
    TType LookupAndActivate<TType>(string typeName, params object[] parameters);
    object LookupAndActivate(string typeName, params object[] parameters);
    TType Activate<TType>(params object[] parameters);
    object Activate(Type type, params object[] parameters);
  }
}
