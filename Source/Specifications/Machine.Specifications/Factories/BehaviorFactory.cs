using System;
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
      Type behaviorType = behaviorField.FieldType.GetGenericArguments().First();

      if(!behaviorType.HasAttribute<BehaviorsAttribute>())
      {
        throw new SpecificationUsageException("Behaviors require the BehaviorsAttribute on the Behaviors class.");
      }

      object behaviorInstance = Activator.CreateInstance(behaviorType);

      if (behaviorType.GetPrivateFieldsOfType<Establish>().Any())
      {
        throw new SpecificationUsageException("You cannot have Establishs on Behaviors.");
      }
      
      if (behaviorType.GetPrivateFieldsOfType<Because>().Any())
      {
        throw new SpecificationUsageException("You cannot have Becauses on Behaviors.");
      }

      if (behaviorType.GetPrivateFieldsWith(typeof(Behaves_like<>)).Any())
      {
        throw new SpecificationUsageException("You cannot nest Behaviors.");
      }

      var isIgnored = behaviorField.HasAttribute<IgnoreAttribute>() ||
                      behaviorInstance.GetType().HasAttribute<IgnoreAttribute>();
      var behavior = new Behavior(behaviorInstance, context, isIgnored);

      IEnumerable<FieldInfo> itFieldInfos = behaviorType.GetPrivateFieldsOfType<It>();
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