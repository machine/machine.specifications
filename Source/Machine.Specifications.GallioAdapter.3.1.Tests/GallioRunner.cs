using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Utilities;
using Gallio.Runner.Reports.Schema;
using Gallio.Common.Reflection;
using System.Reflection;

namespace Machine.Specifications.GallioAdapter.Tests
{
  /// <summary>
  /// This is a helper class to wrap the underlying Gallio test runner
  /// </summary>
  /// <remarks>
  /// I have opted to add some caching to this class so that tests are only executed by the runner once.
  /// It will also execute the entire assembly at once which should be acceptable in most cases.
  /// </remarks>
  public class GallioRunner
  {
    static Dictionary<Assembly, SampleRunner> _runners = new Dictionary<Assembly, SampleRunner>();

    static SampleRunner Run<T>()
    {      
      Assembly assembly = typeof(T).Assembly;
      SampleRunner runner;

      if (!_runners.TryGetValue( assembly, out runner))
      {
        runner = new SampleRunner();
        runner.AddAssembly(assembly);

        if (runner.TestPackage.Files.Count != 0)
          runner.Run();

        _runners.Add(assembly, runner);
      }

      return runner;
    }

    public static void RunAssemblyOf<T>()
    {             
      Run<T>();
    }

    public static TestStepRun RunAllSpecificationsFor<T>()
    {
      SampleRunner runner = Run<T>();
      return runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(T)));      
    }    
  }
}
