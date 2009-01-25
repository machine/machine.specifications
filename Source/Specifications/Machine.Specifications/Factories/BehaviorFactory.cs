using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
  public class BehaviorFactory
  {
    readonly SpecificationFactory _specificationFactory;

    public BehaviorFactory()
    {
      _specificationFactory = new SpecificationFactory();
    }

    public Behavior CreateBehaviorFrom(FieldInfo behaviorField, Context context)
    {
      var behaviorFieldValue = (It_should_behave_like) behaviorField.GetValue(context.Instance);
      object behaviorInstance = behaviorFieldValue.Invoke();

      var fieldInfos = behaviorInstance.GetType().GetPrivateFields();

      if (fieldInfos.Where(info => info.FieldType == typeof(It_should_behave_like)).Any())
      {
        throw new SpecificationUsageException("You cannot nest behaviors.");
      }

      var isIgnored = behaviorField.HasAttribute<IgnoreAttribute>() ||
                      behaviorInstance.GetType().HasAttribute<IgnoreAttribute>();
      var behavior = new Behavior(behaviorInstance, context, isIgnored);

      List<FieldInfo> itFieldInfos = new List<FieldInfo>();
      foreach (FieldInfo info in fieldInfos)
      {
        if (info.FieldType == typeof(It))
        {
          itFieldInfos.Add(info);
        }
      }

      CreateBehaviorSpecifications(itFieldInfos, behavior);

      return behavior;
    }

    void CreateBehaviorSpecifications(IEnumerable<FieldInfo> itFieldInfos,
                                      Behavior behavior)
    {
      foreach (var itFieldInfo in itFieldInfos)
      {
        Specification specification = _specificationFactory.CreateSpecificationFromBehavior(behavior,
                                                                                            itFieldInfo);
        behavior.AddSpecification(specification);
      }
    }
  }
}