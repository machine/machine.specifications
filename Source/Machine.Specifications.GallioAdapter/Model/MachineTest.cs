using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Machine.Specifications.GallioAdapter.Services;

namespace Machine.Specifications.GallioAdapter.Model
{
  public abstract class MachineTest : BaseTest
  {
    protected MachineTest(string name, ICodeElementInfo codeElement) : base(name, codeElement)
    {
    }

    public override Gallio.Func<Gallio.Model.Execution.ITestController> TestControllerFactory
    {
      get
      {
        return CreateTestController;
      }
    }

    private static ITestController CreateTestController()
    {
      return new MachineSpecificationController();
    }
  }
}
