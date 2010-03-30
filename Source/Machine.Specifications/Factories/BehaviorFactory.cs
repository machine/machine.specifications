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
        throw new SpecificationUsageException("Behaviors require the BehaviorsAttribute on the type containing the Specifications. Attribute is missing from " + behaviorType.FullName);
      }

      object behaviorInstance = Activator.CreateInstance(behaviorType);

      if (behaviorType.GetPrivateFieldsOfType<Establish>().Any())
      {
        throw new SpecificationUsageException("You cannot have Establishs on Behaviors. Establish found in " + behaviorType.FullName);
      }
      
      if (behaviorType.GetPrivateFieldsOfType<Because>().Any())
      {
        throw new SpecificationUsageException("You cannot have Becauses on Behaviors. Because found in " + behaviorType.FullName);
      }

      if (behaviorType.GetPrivateFieldsWith(typeof(Behaves_like<>)).Any())
      {
        throw new SpecificationUsageException("You cannot nest Behaviors. Nested Behaviors found in " + behaviorType.FullName);
      }

      EnsureAllBehaviorFieldsAreInContext(behaviorType, behaviorInstance, context);

      var isIgnored = behaviorField.HasAttribute<IgnoreAttribute>() ||
                      behaviorInstance.GetType().HasAttribute<IgnoreAttribute>();
      var behavior = new Behavior(behaviorInstance, context, isIgnored);

      IEnumerable<FieldInfo> itFieldInfos = behaviorType.GetPrivateFieldsOfType<It>();
      CreateBehaviorSpecifications(itFieldInfos, behavior);

      return behavior;
    }
    
    void EnsureAllBehaviorFieldsAreInContext(Type behaviorType, object behaviorInstance, Context context)
    {
      var behaviorFieldsRequiredInContext = behaviorInstance.GetType().GetStaticProtectedOrInheritedFields().Where(x => !x.IsLiteral);
      var contextFields = context.Instance.GetType().GetStaticProtectedOrInheritedFields();

      foreach (FieldInfo requiredField in behaviorFieldsRequiredInContext)
      {
        string requiredFieldName = requiredField.Name;
        FieldInfo contextField = contextFields.Where(x => x.Name == requiredFieldName).SingleOrDefault();
        
        EnsureContextFieldExists(context, contextField, requiredField, behaviorType);
        EnsureContextFieldIsCompatibleType(context, contextField, requiredField, behaviorType);
      }
    }

    void EnsureContextFieldExists(Context context, FieldInfo contextField, FieldInfo requiredField, Type behaviorType)
    {
      if (contextField == null)
      {
        string format = "The context {0} behaves like {1} but does not define the behavior required field {2} {3}";
        string message = String.Format(format, context.Instance.GetType().FullName, behaviorType.FullName,
                                                requiredField.FieldType.FullName, requiredField.Name);
        throw new SpecificationUsageException(message);
      }
    }

    void EnsureContextFieldIsCompatibleType(Context context, FieldInfo contextField, FieldInfo requiredField, Type behaviorType)
    {
      if (!requiredField.FieldType.IsAssignableFrom(contextField.FieldType))
      {
        string format = "The context {0} behaves like {1} but defines the behavior field {2} {3} using an incompatible type.";
        string message = String.Format(format, context.Instance.GetType().FullName, behaviorType.FullName,
                                                requiredField.FieldType.FullName, requiredField.Name);
        throw new SpecificationUsageException(message);
      }
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