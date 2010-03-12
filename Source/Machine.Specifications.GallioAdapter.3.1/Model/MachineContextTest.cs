using System;
using System.Collections.Generic;
using System.Linq;
using Gallio.Common.Reflection;
using Gallio.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  // A fixture for a specification
  public class MachineContextTest : MachineGallioTest
  {
    readonly Specifications.Model.Context _context;

    public Specifications.Model.Context Context
    {
      get { return _context; }
    }

    public MachineContextTest(Specifications.Model.Context context)
      : base(context.Name, Reflector.Wrap(context.Type))
    {
      this.Kind = TestKinds.Fixture;
      this._context = context;
    }

    public void SetupContext()
    {
      _context.EstablishContext();
    }

    public void TeardownContext()
    {
      _context.Cleanup();
    }
  }
}
