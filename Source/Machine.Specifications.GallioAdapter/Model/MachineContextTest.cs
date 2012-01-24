using Gallio.Common.Reflection;
using Gallio.Model;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  public class MachineContextTest : MachineGallioTest
  {
    readonly Context _context;

    public Context Context
    {
      get { return _context; }
    }

    public MachineContextTest(Context context)
      : base(context.Name, Reflector.Wrap(context.Type))
    {
      this.Kind = TestKinds.Fixture;
      this._context = context;
    }   
  }
}
