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
    private Dictionary<Type, Func<object, FieldInfo, Specification>> _creationMethods;
    public SpecificationFactory()
    {
      _creationMethods = new Dictionary<Type, Func<object, FieldInfo, Specification>>();

      _creationMethods[typeof(It)] = CreateItSpecification;
      _creationMethods[typeof(It_should_throw)] = CreateItThrowsSpecification;
    }

    public Specification CreateSpecification(object instance, FieldInfo specificationField)
    {
      return _creationMethods[specificationField.FieldType](instance, specificationField);
    }

    private static Specification CreateItSpecification(object instance, FieldInfo specificationField)
    {
      It it = (It)specificationField.GetValue(instance);

      var specification = new ItSpecification(specificationField, it);
      return specification;
    }

    private static Specification CreateItThrowsSpecification(object instance, FieldInfo specificationField)
    {
      It_should_throw it = (It_should_throw)specificationField.GetValue(instance);

      var specification = new ItShouldThrowSpecification(specificationField, it);
      return specification;
    }
  }
}
