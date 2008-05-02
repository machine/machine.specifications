using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Machine.Specifications.GallioAdapter.Services;

namespace Machine.Specifications.GallioAdapter
{
  public class MachineSpecificationsFramework : BaseTestFramework
  {
    public override string Name
    {
      get { return "Machine.Specifications"; }
    }

    public override ITestExplorer CreateTestExplorer(TestModel testModel)
    {
      return new MachineSpecificationsExplorer(testModel);
    }
  }
}
