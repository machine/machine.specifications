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
    private readonly Description _description;

    public Description Description
    {
      get { return _description; }
    }

    public MachineSpecificationTest(Description description) 
      : base(description.Name, Reflector.Wrap(description.Type))
    {
      this.Kind = TestKinds.Fixture;
      _description = description;
    }

    public void SetupContext()
    {
      _description.RunContextBeforeAll();
    }

    public void TeardownContext()
    {
      _description.RunContextAfterAll();
    }
  }
}
