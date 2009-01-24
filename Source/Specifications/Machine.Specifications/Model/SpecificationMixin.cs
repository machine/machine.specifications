using System.Collections.Generic;
using System.Reflection;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Model
{
  public class SpecificationMixin : Specification
  {
    readonly object _source;
    readonly object _target;

    public SpecificationMixin(string name, It it, bool isIgnored, FieldInfo fieldInfo, object source, object target)
      : base(name, it, isIgnored, fieldInfo)
    {
      _source = source;
      _target = target;
    }

    protected override void InvokeSpecificationField()
    {
      ConventionMapper.MapPropertiesOf(_source).To(_target);
      base.InvokeSpecificationField();
    }
  }

  static class ConventionMapper
  {
    internal static ConventionMap MapPropertiesOf(object source)
    {
      return new ConventionMap(source);
    }

    internal class ConventionMap
    {
      readonly object _source;

      public ConventionMap(object source)
      {
        _source = source;
      }

      public void To(object target)
      {
        IEnumerable<FieldInfo> sourceFields = _source.GetType().GetStaticProtectedOrInheritedFields();
        
        foreach (var sourceField in sourceFields)
        {
          FieldInfo targetField = target.GetType().GetPrivateOrInheritedFieldNamed(sourceField.Name);
          if(targetField == null)
          {
            continue;
          }

          targetField.SetValue(target, sourceField.GetValue(_source));
        }
      }
    }
  }
}