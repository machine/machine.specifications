using System;
using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Sdk;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Factories
{
  public class CollectionArguments<T>
  {
    Type _target;
    Type _baseType;
    Func<object> _instanceResolver;
    bool _ensureMaximumOfOne;
    AttributeFullName _attributeFullName;
    object _instance; 

    public ICollection<T> Items { get; private set; }

    public CollectionArguments(Type target,
      Func<object> instanceResolver,
      ICollection<T> items,
      bool ensureMaximumOfOne,
      AttributeFullName attributeFullName)
    {
      _target = target;
      _instanceResolver = instanceResolver;
      _ensureMaximumOfOne = ensureMaximumOfOne;
      _attributeFullName = attributeFullName;
      _instance = instanceResolver();
      _baseType = target.BaseType;

      Items = items;
    }

    public bool TargetIsStatic
    {
      get { return _target.IsAbstract && _target.IsSealed; }
    }

    public void CollectFieldValue()
    {
      var fields = _target.GetInstanceFieldsOfUsage(_attributeFullName);
      if (_ensureMaximumOfOne && fields.Count() > 1)
      {
        throw new SpecificationUsageException(String.Format("You cannot have more than one {0} clause in {1}",
          fields.First().FieldType.Name,
          _target.FullName));
      }
      var field = fields.FirstOrDefault();
      if (field != null)
      {
        var val = (T) field.GetValue(_instance);
        Items.Add(val);
      }
    }

    public bool HasNoInstance
    {
      get { return _instance == null; }
    }

    public bool AreNotValidForCollection
    {
      get { return _target == typeof(object); }
    }

    public Func<Type> GetDeclaringTypeResolver()
    {
      Func<Type> declaringTypeResolution = () => _target.DeclaringType;
      Func<Type> resolveDeclaringTypeUsingAnInstanceToMaintainCorrectGenericParameters = () => GetDeclaringType();

      var typeResolver = _instance == null
        ? declaringTypeResolution
        : resolveDeclaringTypeUsingAnInstanceToMaintainCorrectGenericParameters;

      return typeResolver;
    }

    Type GetDeclaringType()
    {
      var targetType = _instance.GetType();
      var declaringType = targetType.DeclaringType;

      if (declaringType == typeof(object))
      {
        return declaringType;
      }
      if (!declaringType.ContainsGenericParameters)
      {
        return declaringType;
      }

      var numberOfGenericParametersToProvideToEnclosingType =
        declaringType.GetGenericTypeDefinition().GetGenericArguments().Count();
      var parameters = targetType.GetGenericArguments().Take(numberOfGenericParametersToProvideToEnclosingType);
      var typeDefinition = declaringType.MakeGenericType(parameters.ToArray());

      return typeDefinition;
    }

    public CollectionArguments<T> DetailsForBaseType()
    {
      return new CollectionArguments<T>(_target.BaseType, 
        _instanceResolver, 
        Items, 
        _ensureMaximumOfOne, 
        _attributeFullName);
    }

    public CollectionArguments<T> DetailsForDeclaringType()
    {
      return new CollectionArguments<T>(_target.DeclaringType,
        () => Activator.CreateInstance(GetDeclaringTypeResolver()()),
        Items,
        _ensureMaximumOfOne,
        _attributeFullName);
    }

    public static CollectionArguments<Delegate> CreateToInspectDelegates(object instance,
      bool ensureMaximumOfOne,
      AttributeFullName attributeFullName)
    {
      var delegates = new List<Delegate>();
      var type = instance.GetType();

      var fields = type.GetInstanceFieldsOfUsage(attributeFullName);

      return new CollectionArguments<Delegate>(type,
        () => instance,
        delegates,
        ensureMaximumOfOne,
        attributeFullName);
    }
  }
}