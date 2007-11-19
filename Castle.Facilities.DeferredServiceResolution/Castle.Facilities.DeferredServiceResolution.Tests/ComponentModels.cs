using System;
using System.Collections.Generic;
using Castle.Core;

namespace Castle.Facilities.DeferredServiceResolution
{
  public static class ComponentModels
  {
    public static bool UseInterfacesAsServiceType = false;
    public static ComponentModel Service1Model = MakeModel("1", typeof(Service1), typeof(Service1));
    public static ComponentModel Service2Model = MakeModel("2", typeof(Service2), typeof(Service2));
    public static ComponentModel Service3Model = MakeModel("3", typeof(Service3), typeof(Service3));
    public static ComponentModel NoServicesModel = MakeModel("NoServices", typeof(NoServices), typeof(NoServices));
    public static ComponentModel Service1And2Model = MakeModel("1And2", typeof(Service1And2), typeof(Service1And2));

    public static List<ComponentModel> WithService1And2()
    {
      List<ComponentModel> models = WithNoServices();
      models.Add(Service1Model);
      models.Add(Service2Model);
      return models;
    }
    public static List<ComponentModel> WithService1And2And3()
    {
      List<ComponentModel> models = WithService1And2();
      models.Add(Service3Model);
      return models;
    }
    public static List<ComponentModel> WithDuplicateService1()
    {
      List<ComponentModel> models = Empty();
      models.Add(Service1And2Model);
      models.Add(Service1Model);
      return models;
    }
    public static List<ComponentModel> Empty()
    {
      return new List<ComponentModel>();
    }
    public static List<ComponentModel> WithNoServices()
    {
      List<ComponentModel> models = Empty();
      models.Add(NoServicesModel);
      return models;
    }
    public static ComponentModel MakeModel(string name, Type serviceType, Type implementationType)
    {
      if (UseInterfacesAsServiceType)
      {
        return new ComponentModel(name, serviceType, implementationType);
      }
      return new ComponentModel(name, implementationType, implementationType);
    }
  }
  [Singleton]
  public class Service1And2 : IService1, IService2
  {
  }
  [Transient]
  public class Service2 : IService2
  {
  }
  [PerThread]
  public class Service1 : IService1
  {
  }
  public interface IService1
  {
  }
  public interface IService2
  {
  }
  public class NoServices
  {
  }
  public interface IService3
  {
  }
  [Singleton]
  public class Service3 : IService3
  {
  }
  public abstract class SomethingAbstract
  {
  }
  public class Service2DependsOnService1 : IService2
  {
    public Service2DependsOnService1(IService1 s1) { }
  }
  public class Service3DependsOnService1 : IService3
  {
    public Service3DependsOnService1(IService1 s1) { }
  }
  public class Service3DependsOnService1And2 : IService3
  {
    public Service3DependsOnService1And2(IService1 s1, IService2 s2) { }
  }
}