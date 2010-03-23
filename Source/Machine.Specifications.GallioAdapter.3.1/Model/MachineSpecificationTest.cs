using System;
using System.Collections.Generic;
using System.Linq;
using Gallio.Common.Reflection;
using Gallio.Model;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  public class MachineSpecificationTest : MachineGallioTest
  {
    readonly Specification _specification;
    public Specification Specification { get { return _specification; } }

    public MachineSpecificationTest(Specification specification) 
      : base(specification.Name, Reflector.Wrap(specification.FieldInfo))
    {
      this.Kind = TestKinds.Test;
      this.IsTestCase = true;
      _specification = specification;
    }   
  }
}
