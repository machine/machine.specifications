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

    public SpecificationFactory()
    {
    }

    public Specification CreateSpecification(FieldInfo specificationField)
    {
      return new Specification(specificationField);
    }
  }
}