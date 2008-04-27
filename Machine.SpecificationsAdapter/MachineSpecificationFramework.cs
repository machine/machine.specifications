using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;

namespace Machine.Specifications.GallioAdapter
{
  public class MachineSpecificationFramework : BaseTestFramework
  {
    public override string Name
    {
      get { return "Machine.Specifications"; }
    }

    public override ITestExplorer CreateTestExplorer(TestModel testModel)
    {
      return new MachineSpecificationExplorer(testModel);
    }
  }
}
