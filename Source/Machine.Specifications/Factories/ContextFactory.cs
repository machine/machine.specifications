using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
  public class ContextFactory
  {
    readonly BehaviorFactory _behaviorFactory;
    readonly SpecificationFactory _specificationFactory;
    static int _allowedNumberOfBecauseBlocks = 1;

    public ContextFactory()
    {
      _specificationFactory = new SpecificationFactory();
      _behaviorFactory = new BehaviorFactory();
    }

    public Context CreateContextFrom(object instance, FieldInfo fieldInfo)
    {
      if (fieldInfo.IsOfUsage(DelegateUsage.Assert))
      {
        return CreateContextFrom(instance, new[] { fieldInfo });
      }
      return CreateContextFrom(instance);
    }

    public Context CreateContextFrom(object instance)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetInstanceFieldsOfUsage(DelegateUsage.Assert, DelegateUsage.Behavior);
      return CreateContextFrom(instance, fieldInfos);
    }

    Context CreateContextFrom(object instance, IEnumerable<FieldInfo> acceptedSpecificationFields)
    {
      var type = instance.GetType();
      var fieldInfos = type.GetInstanceFields();
      var itFieldInfos = new List<FieldInfo>();
      var itShouldBehaveLikeFieldInfos = new List<FieldInfo>();

      var contextClauses = ExtractPrivateFieldValues(instance, true, DelegateUsage.Setup);
      contextClauses.Reverse();

      var cleanupClauses = ExtractPrivateFieldValues(instance, true, DelegateUsage.Cleanup);

      var becauses = ExtractPrivateFieldValues(instance, false, DelegateUsage.Act);
      becauses.Reverse();

      if (becauses.Count > _allowedNumberOfBecauseBlocks)
      {
        var message = String.Format("There can only be one Because clause. Found {0} Becauses in the type hierarchy of {1}.",
                                    becauses.Count,
                                    instance.GetType().FullName);
        throw new SpecificationUsageException(message);
      }

      var concern = ExtractSubject(type);
      var isSetupForEachSpec = IsSetupForEachSpec(type);

      var isIgnored = type.HasAttribute<IgnoreAttribute>();
      var tags = ExtractTags(type);
      var context = new Context(type,
                                instance,
                                contextClauses,
                                becauses,
                                cleanupClauses,
                                concern,
                                isIgnored,
                                tags,
                                isSetupForEachSpec);

      foreach (var info in fieldInfos)
      {
        if (acceptedSpecificationFields.Contains(info) &&
            info.IsOfUsage(DelegateUsage.Assert))
        {
          itFieldInfos.Add(info);
        }

        if (acceptedSpecificationFields.Contains(info) &&
            info.IsOfUsage(DelegateUsage.Behavior))
        {
          itShouldBehaveLikeFieldInfos.Add(info);
        }
      }

      CreateSpecifications(itFieldInfos, context);
      CreateSpecificationsFromBehaviors(itShouldBehaveLikeFieldInfos, context);

      return context;
    }

    static IEnumerable<Tag> ExtractTags(Type type)
    {
      var extractor = new AttributeTagExtractor();
      return extractor.ExtractTags(type);
    }

    static bool IsSetupForEachSpec(Type type)
    {
      return type.GetCustomAttributes(typeof(SetupForEachSpecification), true).Any();
    }

    static Subject ExtractSubject(Type type)
    {
      var attributes = TypeDescriptor.GetAttributes(type).OfType<SubjectAttribute>();

      if (attributes.Count() > 1)
      {
        throw new SpecificationUsageException("Cannot have more than one Subject on a Context");
      }

      if (!attributes.Any())
      {
        if (type.DeclaringType == null)
        {
          return null;
        }

        return ExtractSubject(type.DeclaringType);
      }

      return attributes.First().CreateSubject();
    }

    void CreateSpecifications(IEnumerable<FieldInfo> itFieldInfos, Context context)
    {
      foreach (var itFieldInfo in itFieldInfos)
      {
        var specification = _specificationFactory.CreateSpecification(context, itFieldInfo);
        context.AddSpecification(specification);
      }
    }

    void CreateSpecificationsFromBehaviors(IEnumerable<FieldInfo> itShouldBehaveLikeFieldInfos,
                                           Context context)
    {
      foreach (var itShouldBehaveLikeFieldInfo in itShouldBehaveLikeFieldInfos)
      {
        var behavior = _behaviorFactory.CreateBehaviorFrom(itShouldBehaveLikeFieldInfo, context);

        foreach (var specification in behavior.Specifications)
        {
          context.AddSpecification(specification);
        }
      }
    }

    static void CollectDetailsOf<T>(Type target, Func<object> instanceResolver, ICollection<T> items, bool ensureMaximumOfOne, DelegateUsage usage)
    {
      if (target == typeof(Object) || target == null)
      {
        return;
      }

      if (target.IsAbstract)
      {

      }

      if (!IsStatic(target))
      {
        var instance = instanceResolver();
        if (instance == null)
        {
          return;
        }

        var fields = target.GetInstanceFieldsOfUsage(usage);

        if (ensureMaximumOfOne && fields.Count() > 1)
        {
          throw new SpecificationUsageException(String.Format("You cannot have more than one {0} clause in {1}",
                                                              fields.First().FieldType.Name,
                                                              target.FullName));
        }
        var field = fields.FirstOrDefault();

        if (field != null)
        {
          var val = (T)field.GetValue(instance);
          items.Add(val);
        }

        CollectDetailsOf(target.BaseType, () => instance, items, ensureMaximumOfOne, usage);
      }

      CollectDetailsOf(target.DeclaringType, () => Activator.CreateInstance(target.DeclaringType), items, ensureMaximumOfOne, usage);
    }

    static bool IsStatic(Type target)
    {
      return target.IsAbstract && target.IsSealed;
    }

    static List<Delegate> ExtractPrivateFieldValues(object instance, bool ensureMaximumOfOne, DelegateUsage usage)
    {
      var delegates = new List<Delegate>();
      var type = instance.GetType();
      CollectDetailsOf(type, () => instance, delegates, ensureMaximumOfOne, usage);

      return delegates;
    }

    public static void ChangeAllowedNumberOfBecauseBlocksTo(int newValue)
    {
      _allowedNumberOfBecauseBlocks = newValue;
    }
  }
}