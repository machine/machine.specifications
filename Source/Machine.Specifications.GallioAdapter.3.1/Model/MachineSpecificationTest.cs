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

    public MachineSpecificationTest(Specification specification) 
      : base(specification.Name, Reflector.Wrap(specification.FieldInfo))
    {
      this.Kind = TestKinds.Test;
      this.IsTestCase = true;
      _specification = specification;
    }

    public Result Execute()
    {
      MachineContextTest parent = this.Parent as MachineContextTest;
      if (parent == null) throw new Exception("Specification has non-Context parent???");
      
      return _specification.Verify();   
    }
  }
}
