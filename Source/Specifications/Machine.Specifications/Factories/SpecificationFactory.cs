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
    Dictionary<Type, Func<FieldInfo, Specification>> _creationMethods;

    public SpecificationFactory()
    {
      _creationMethods = new Dictionary<Type, Func<FieldInfo, Specification>>();

      _creationMethods[typeof(It)] = CreateItSpecification;
      //_creationMethods[typeof(It_should_throw)] = CreateItThrowsSpecification;
    }

    public Specification CreateSpecification(FieldInfo specificationField)
    {
      return _creationMethods[specificationField.FieldType](specificationField);
    }

    static Specification CreateItSpecification(FieldInfo specificationField)
    {
      var specification = new ItSpecification(specificationField);
      return specification;
    }
  }
}