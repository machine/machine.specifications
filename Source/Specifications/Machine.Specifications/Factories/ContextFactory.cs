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
    readonly SpecificationFactory _specificationFactory;

    public ContextFactory()
    {
      _specificationFactory = new SpecificationFactory();
    }

    public Context CreateContextFrom(object instance, FieldInfo fieldInfo)
    {
      return CreateContextFrom(instance, new[] {fieldInfo});
    }

    public Context CreateContextFrom(object instance)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();

      return CreateContextFrom(instance, fieldInfos);
    }

    Context CreateContextFrom(object instance, IEnumerable<FieldInfo> acceptedSpecificationFields)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();
      List<FieldInfo> itFieldInfos = new List<FieldInfo>();

      var beforeAlls = ExtractPrivateFieldValues<Establish>(instance, "context_once");
      var beforeEachs = ExtractPrivateFieldValues<Establish>(instance, "context");
      beforeAlls.Reverse();
      beforeEachs.Reverse();

      var afterAlls = ExtractPrivateFieldValues<Cleanup>(instance, "after_all");
      var afterEachs = ExtractPrivateFieldValues<Cleanup>(instance, "after_each");

      var becauses = ExtractPrivateFieldValues<Because>(instance, "of");

      if (becauses.Count > 1)
      {
        throw new SpecificationUsageException("There can only be one Because clause.");
      }

      var concern = ExtractSubject(type);

      var isIgnored = type.HasAttribute<IgnoreAttribute>();
      var context = new Context(type, instance, beforeEachs, beforeAlls, becauses.FirstOrDefault(), afterEachs, afterAlls, concern, isIgnored);

      foreach (FieldInfo info in fieldInfos)
      {
        if (acceptedSpecificationFields.Contains(info) &&
          info.FieldType == typeof(It))
        {
          itFieldInfos.Add(info);
        }
      }

      CreateSpecifications(itFieldInfos, context);

      return context;
    }

    static Subject ExtractSubject(Type type)
    {
      var attributes = type.GetCustomAttributes(typeof(SubjectAttribute), true);

      if (attributes.Length > 1)
        throw new SpecificationUsageException("Cannot have more than one Subject on a Context");

      if (attributes.Length == 0) return null;

      var attribute = (SubjectAttribute)attributes[0];

      return new Subject(attribute.SubjectType, attribute.SubjectText);
    }

    void CreateSpecifications(IEnumerable<FieldInfo> itFieldInfos, Context context)
    {
      foreach (var itFieldInfo in itFieldInfos)
      {
        Specification specification = _specificationFactory.CreateSpecification(context, itFieldInfo);
        context.AddSpecification(specification);
      }
    }

    static List<T> ExtractPrivateFieldValues<T>(object instance, string name)
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