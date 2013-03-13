using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Machine.Specifications.GallioAdapter.Model
{
  public abstract class MachineGallioTest : Test
  {
    protected MachineGallioTest(string name, ICodeElementInfo codeElement)
      : base(name, codeElement)
    {
    }
  }
}
