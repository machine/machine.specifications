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
      var behaviorType = behaviorField.FieldType.GetGenericArguments().First();

      EnsureBehaviorHasBehaviorsAttribute(behaviorType);
      EnsureBehaviorDoesNotHaveFrameworkFieldsExceptIt(behaviorType);

      var behaviorInstance = Activator.CreateInstance(behaviorType);

      EnsureAllBehaviorFieldsAreInContext(behaviorType, context);

      var isIgnored = behaviorField.HasAttribute<IgnoreAttribute>() ||
                      behaviorInstance.GetType().HasAttribute<IgnoreAttribute>();
      var behavior = new Behavior(behaviorField.FieldType, behaviorInstance, context, isIgnored);

      var itFieldInfos = behaviorType.GetInstanceFieldsOfUsage(DelegateUsage.Assert);
      CreateBehaviorSpecifications(itFieldInfos, behavior);

      return behavior;
    }

    static void EnsureBehaviorHasBehaviorsAttribute(Type behaviorType)
    {
      if (!behaviorType.HasAttribute<BehaviorsAttribute>())
      {
        throw new SpecificationUsageException(
          "Behaviors require the BehaviorsAttribute on the type containing the Specifications. Attribute is missing from " +
          behaviorType.FullName);
      }
    }

    static void EnsureBehaviorDoesNotHaveFrameworkFieldsExceptIt(Type behaviorType)
    {
      if (behaviorType.GetInstanceFieldsOfUsage(DelegateUsage.Setup).Any())
      {
        throw new SpecificationUsageException("You cannot have setup actions on Behaviors. Setup action found in " +
                                              behaviorType.FullName);
      }

      if (behaviorType.GetInstanceFieldsOfUsage(DelegateUsage.Act).Any())
      {
        throw new SpecificationUsageException("You cannot have act actions on Behaviors. Act action found in " +
                                              behaviorType.FullName);
      }

      if (behaviorType.GetInstanceFieldsOfUsage(DelegateUsage.Cleanup).Any())
      {
        throw new SpecificationUsageException("You cannot have cleanup actions on Behaviors. Cleanup action found in " +
                                              behaviorType.FullName);
      }

      if (behaviorType.GetInstanceFieldsOfUsage(DelegateUsage.Behavior).Any())
      {
        throw new SpecificationUsageException("You cannot nest behaviors. Nested behaviors found in " +
                                              behaviorType.FullName);
      }
    }

    void EnsureAllBehaviorFieldsAreInContext(Type behaviorType, Context context)
    {
      var behaviorFieldsRequiredInContext = behaviorType.GetStaticProtectedOrInheritedFields().Where(x => !x.IsLiteral);
      var contextFields = context.Instance.GetType().GetStaticProtectedOrInheritedFields();

      foreach (var requiredField in behaviorFieldsRequiredInContext)
      {
        var requiredFieldName = requiredField.Name;
        var contextField = contextFields.SingleOrDefault(x => x.Name == requiredFieldName);
        
        EnsureContextFieldExists(context, contextField, requiredField, behaviorType);
        EnsureContextFieldIsCompatibleType(context, contextField, requiredField, behaviorType);
      }
    }

    static void EnsureContextFieldExists(Context context, FieldInfo contextField, FieldInfo requiredField, Type behaviorType)
    {
      if (contextField != null)
      {
        return;
      }
      
      const string Format = "The context {0} behaves like {1} but does not define the behavior required field {2} {3}";
      var message = String.Format(Format,
                                  context.Instance.GetType().FullName,
                                  behaviorType.FullName,
                                  requiredField.FieldType.FullName,
                                  requiredField.Name);

      throw new SpecificationUsageException(message);
    }

    static void EnsureContextFieldIsCompatibleType(Context context, FieldInfo contextField, FieldInfo requiredField, Type behaviorType)
    {
      if (requiredField.FieldType.IsAssignableFrom(contextField.FieldType))
      {
        return;
      }
      
      const string Format = "The context {0} behaves like {1} but defines the behavior field {2} {3} using an incompatible type.";
      var message = String.Format(Format,
                                  context.Instance.GetType().FullName,
                                  behaviorType.FullName,
                                  requiredField.FieldType.FullName,
                                  requiredField.Name);

      throw new SpecificationUsageException(message);
    }

    void CreateBehaviorSpecifications(IEnumerable<FieldInfo> itFieldInfos,
                                      Behavior behavior)
    {
      foreach (var itFieldInfo in itFieldInfos)
      {
        var specification = _specificationFactory.CreateSpecificationFromBehavior(behavior,
                                                                                  itFieldInfo);
        behavior.AddSpecification(specification);
      }
    }
  }
}