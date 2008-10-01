using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  public class MachineContextTest : MachineGallioTest
  {
    private readonly Specifications.Model.Context context;

    public Specifications.Model.Context Context
    {
      get { return context; }
    }

    public MachineContextTest(Specifications.Model.Context context) 
      : base(context.Name, Reflector.Wrap(context.Type))
    {
      this.Kind = TestKinds.Fixture;
      this.context = context;
    }

    public void SetupContext()
    {
      context.EstablishContext();
    }

    public void TeardownContext()
    {
      context.Cleanup();
    }
  }
}
