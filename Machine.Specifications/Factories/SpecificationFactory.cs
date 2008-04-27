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

      foreach (FieldInfo info in fieldInfos)
      {
        if (info.FieldType == typeof (When))
        {
          whenFieldInfo = info;
          whenClause = info.Name.ReplaceUnderscores();
          when = (When)whenFieldInfo.GetValue(instance);
        }
        if (info.FieldType == typeof(It) ||
            info.FieldType == typeof(It_should_throw))
        {
          itFieldInfos.Add(info);
        }
      }

      var instanceWithContext = instance as IHasContext;
      Action<VerificationContext> contextSetup = context=>
      {
        if (instanceWithContext != null)
          instanceWithContext.SetupContext();
        if (when != null)
        {
          try
          {
            when();
          }
          catch (Exception exception)
          {
            context.ThrownException = exception;
          }
        }
      };

      var specification = new Specification(type.Name.ReplaceUnderscores(), instance, contextSetup) { 
            WhenClause = whenClause
          };

      foreach (FieldInfo info in itFieldInfos)
      {
        Requirement requirement = _requirementFactory.CreateRequirement(instance, info);
        specification.AddRequirement(requirement);
      }

      return specification;
    }
  }
}
