using System.Collections.Generic;
using System.Reflection;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Model
{
  public class BehaviorSpecification : Specification
  {
    readonly object _contextInstance;
    readonly object _behaviorInstance;
    readonly ConventionMapper _mapper;

    public BehaviorSpecification(string name, It it, bool isIgnored, FieldInfo fieldInfo, Context context, Behavior behavior)
      : base(name, it, isIgnored, fieldInfo)
    {
      _contextInstance = context.Instance;
      _behaviorInstance = behavior.Instance;

      _mapper = new ConventionMapper();
    }

    protected override void InvokeSpecificationField()
    {
      _mapper.MapPropertiesOf(_contextInstance).To(_behaviorInstance);
      base.InvokeSpecificationField();
    }

    #region Nested type: ConventionMapper
    class ConventionMapper
    {
      internal ConventionMap MapPropertiesOf(object source)
      {
        return new ConventionMap(source);
      }

      #region Nested type: ConventionMap
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
            FieldInfo targetField = target.GetType().GetStaticProtectedOrInheritedFieldNamed(sourceField.Name);
            if (targetField == null)
            {
              continue;
            }

            targetField.SetValue(target, sourceField.GetValue(_source));
          }
        }
      }
      #endregion
    }
    #endregion
  }
}