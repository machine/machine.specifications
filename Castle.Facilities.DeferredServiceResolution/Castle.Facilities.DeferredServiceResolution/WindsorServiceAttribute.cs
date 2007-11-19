using System;

namespace Castle.Facilities.DeferredServiceResolution
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
  public class WindsorServiceAttribute : Attribute
  {
    private readonly string _name;
    private readonly Type _serviceType;

    public WindsorServiceAttribute(string name, Type serviceType)
    {
      _name = name;
      _serviceType = serviceType;
    }
    public WindsorServiceAttribute(Type serviceType)
    {
      _serviceType = serviceType;
    }

    public string Name
    {
      get { return _name; }
    }

    public Type ServiceType
    {
      get { return _serviceType; }
    }
  }
}