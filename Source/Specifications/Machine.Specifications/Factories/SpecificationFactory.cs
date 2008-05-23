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
    private Dictionary<Type, Func<FieldInfo, FieldInfo, Specification>> _creationMethods;
    public SpecificationFactory()
    {
      _creationMethods = new Dictionary<Type, Func<FieldInfo, FieldInfo, Specification>>();

      _creationMethods[typeof(It)] = CreateItSpecification;
      //_creationMethods[typeof(It_should_throw)] = CreateItThrowsSpecification;
    }

    public Specification CreateSpecification(FieldInfo specificationField, FieldInfo whenField)
    {
      return _creationMethods[specificationField.FieldType](specificationField, whenField);
    }

    private static Specification CreateItSpecification(FieldInfo specificationField, FieldInfo whenField)
    {
      var specification = new ItSpecification(specificationField, whenField);
      return specification;
    }
  }
}
