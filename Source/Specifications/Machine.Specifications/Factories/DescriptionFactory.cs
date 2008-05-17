using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Factories
{
  public class DescriptionFactory
  {
    private SpecificationFactory _specificationFactory;

    public DescriptionFactory()
    {
      _specificationFactory = new SpecificationFactory();
    }

    public Description CreateDescriptionFrom(object instance, FieldInfo fieldInfo)
    {
      return CreateDescriptionFrom(instance, new[] {fieldInfo});
    }

    public Description CreateDescriptionFrom(object instance)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();

      return CreateDescriptionFrom(instance, fieldInfos);
    }

    private Description CreateDescriptionFrom(object instance, IEnumerable<FieldInfo> acceptedSpecificationFields)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();
      string whenClause = "";
      List<FieldInfo> itFieldInfos = new List<FieldInfo>();
      FieldInfo whenFieldInfo = null;

      var beforeAlls = ExtractPrivateFieldValues<Context>(instance, "before_all");
      var beforeEachs = ExtractPrivateFieldValues<Context>(instance, "before_each");
      beforeAlls.Reverse();
      beforeEachs.Reverse();

      var afterAlls = ExtractPrivateFieldValues<Context>(instance, "after_all");
      var afterEachs = ExtractPrivateFieldValues<Context>(instance, "after_each");

      var description = new Description(type, instance, beforeEachs, beforeAlls, afterEachs, afterAlls);

      foreach (FieldInfo info in fieldInfos)
      {
        if (info.FieldType == typeof(When))
        {
          whenFieldInfo = info;
        }
        else if (acceptedSpecificationFields.Contains(info) &&
          (info.FieldType == typeof(It) ||
           info.FieldType == typeof(It_should_throw)))
        {
          Specification specification = _specificationFactory.CreateSpecification(info, whenFieldInfo);
          description.AddSpecification(specification);
        }
      }

      return description;
    }

    private List<T> ExtractPrivateFieldValues<T>(object instance, string name)
    {
      var delegates = new List<T>();
      var type = instance.GetType();
      while (type != null)
      {
        FieldInfo field = type.GetPrivateFieldsWith(typeof(T)).Where(x => x.Name == name).FirstOrDefault();
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
