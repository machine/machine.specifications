using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Factories
{
  public class SpecificationFactory
  {
    private RequirementFactory _requirementFactory;

    public SpecificationFactory()
    {
      _requirementFactory = new RequirementFactory();
    }

    public Specification CreateSpecificationFrom(object instance)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetPrivateFields();
      string whenClause = "";
      List<FieldInfo> itFieldInfos = new List<FieldInfo>();
      FieldInfo whenFieldInfo = null;
      When when = null;

      var beforeAlls = ExtractPrivateFieldValues<Before>(instance, "all");
      var beforeEachs = ExtractPrivateFieldValues<Before>(instance, "each");
      beforeAlls.Reverse();
      beforeEachs.Reverse();

      var afterAlls = ExtractPrivateFieldValues<After>(instance, "_all");
      var afterEachs = ExtractPrivateFieldValues<After>(instance, "_each");

      foreach (FieldInfo info in fieldInfos)
      {
        if (info.FieldType == typeof (When))
        {
          whenFieldInfo = info;
          whenClause = info.Name.ReplaceUnderscores();
          when = (When)whenFieldInfo.GetValue(instance);
        }
        else if (info.FieldType == typeof(It) ||
            info.FieldType == typeof(It_should_throw))
        {
          itFieldInfos.Add(info);
        }
      }

      var specification = new Specification(type, instance, beforeEachs, beforeAlls, afterEachs, afterAlls, when) { 
            WhenClause = whenClause
          };

      foreach (FieldInfo info in itFieldInfos)
      {
        Requirement requirement = _requirementFactory.CreateRequirement(instance, info);
        specification.AddRequirement(requirement);
      }

      return specification;
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
