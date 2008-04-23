using System;
using System.Collections.Generic;

namespace Machine.Core.Services.Impl
{
  public class DotNetObjectActivator : IObjectActivator
  {
    #region IObjectActivator Members
    public TType LookupAndActivate<TType>(string typeName, params object[] parameters)
    {
      Type type = Type.GetType(typeName);
      if (type == null)
      {
        throw new ArgumentException("typeName");
      }
      return (TType)Activator.CreateInstance(type, parameters);
    }

    public object LookupAndActivate(string typeName, params object[] parameters)
    {
      Type type = Type.GetType(typeName);
      if (type == null)
      {
        throw new ArgumentException("typeName");
      }
      return Activator.CreateInstance(type, parameters);
    }

    public TType Activate<TType>(params object[] parameters)
    {
      return (TType)Activator.CreateInstance(typeof(TType), parameters);
    }

    public object Activate(Type type, params object[] parameters)
    {
      return Activator.CreateInstance(type, parameters);
    }
    #endregion
  }
}
