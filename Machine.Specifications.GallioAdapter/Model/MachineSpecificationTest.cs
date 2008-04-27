using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;

namespace Machine.SpecificationsAdapter.Model
{
  public class MachineSpecificationTest : BaseTest
  {
    public MachineSpecificationTest(Specification specification) 
      : base(specification.Name, Reflector.Wrap(specification.Type))
    {
      this.Kind = TestKinds.Fixture;
    }
  }
}
