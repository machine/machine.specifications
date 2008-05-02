using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  public class MachineSpecificationTest : MachineTest
  {
    private readonly Specification _specification;

    public Specification Specification
    {
      get { return _specification; }
    }

    public MachineSpecificationTest(Specification specification) 
      : base(specification.Name, Reflector.Wrap(specification.Type))
    {
      this.Kind = TestKinds.Fixture;
      _specification = specification;
    }

    public void SetupContext()
    {
      _specification.RunContextBeforeAll();
    }

    public void TeardownContext()
    {
      _specification.RunContextAfterAll();
    }
  }
}
