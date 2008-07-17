using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
  public class ContextFactory
  {
    SpecificationFactory _specificationFactory;

    public ContextFactory()
    {
      _specificationFactory = new SpecificationFactory();
    }

    public Model.Context CreateContextFrom(object instance, FieldInfo fieldInfo)
    {
      return CreateContextFrom(instance, new[] {fieldInfo});
    }

    public Model.Context CreateContextFrom(object instance)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();

      return CreateContextFrom(instance, fieldInfos);
    }

    Model.Context CreateContextFrom(object instance, IEnumerable<FieldInfo> acceptedSpecificationFields)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();
      List<FieldInfo> itFieldInfos = new List<FieldInfo>();
      List<FieldInfo> whenFieldInfos = new List<FieldInfo>();

      var beforeAlls = ExtractPrivateFieldValues<Establish>(instance, "context_once");
      var beforeEachs = ExtractPrivateFieldValues<Establish>(instance, "context");
      beforeAlls.Reverse();
      beforeEachs.Reverse();

      var afterAlls = ExtractPrivateFieldValues<Cleanup>(instance, "after_all");
      var afterEachs = ExtractPrivateFieldValues<Cleanup>(instance, "after_each");

      var concern = ExtractConcern(type);

      var description = new Model.Context(type, instance, beforeEachs, beforeAlls, afterEachs, afterAlls, concern);

      foreach (FieldInfo info in fieldInfos)
      {
        if (info.FieldType == typeof(Because))
        {
          whenFieldInfos.Add(info);
        }
        else if (acceptedSpecificationFields.Contains(info) &&
          info.FieldType == typeof(It))
        {
          itFieldInfos.Add(info);
        }
      }

      CreateSpecifications(whenFieldInfos, itFieldInfos, description);

      return description;
    }

    Concern ExtractConcern(Type type)
    {
      var attributes = type.GetCustomAttributes(typeof(ConcernAttribute), true);

      if (attributes.Length > 1)
        throw new SpecificationUsageException("Cannot have more than one Concern on a Context");

      if (attributes.Length == 0) return null;

      var attribute = (ConcernAttribute)attributes[0];

      return new Concern(attribute.TypeConcernedWith, attribute.SpecificConcern);
    }

    void CreateSpecifications(List<FieldInfo> whenFieldInfos, List<FieldInfo> itFieldInfos, Model.Context context)
    {
      if (whenFieldInfos.Count > 0)
      {
        foreach (var whenFieldInfo in whenFieldInfos)
        {
          CreateSpecificationsForEachIt(whenFieldInfo, itFieldInfos, context);
        }
      }
      else
      {
        CreateSpecificationsForEachIt(null, itFieldInfos, context);
      }
    }

    void CreateSpecificationsForEachIt(FieldInfo whenFieldInfo, List<FieldInfo> itFieldInfos, Model.Context context)
    {
      foreach (var itFieldInfo in itFieldInfos)
      {
        Specification specification = _specificationFactory.CreateSpecification(itFieldInfo, whenFieldInfo);
        context.AddSpecification(specification);
      }
    }

    List<T> ExtractPrivateFieldValues<T>(object instance, string name)
    {
      var delegates = new List<T>();
      var type = instance.GetType();
      while (type != null)
      {
        FieldInfo field = type.GetPrivateFieldsWith(typeof(T)).Where(x=>x.Name == name).FirstOrDefault();
        if (field != null)
        {
          T val = (T)field.GetValue(instance);
          delegates.Add(val);
        }

        type = type.BaseType;
      }

      return delegates;
    }
  }
}