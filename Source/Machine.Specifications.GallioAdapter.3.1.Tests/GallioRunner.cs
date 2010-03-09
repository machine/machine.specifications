using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Utilities;
using Gallio.Runner.Reports.Schema;
using Gallio.Common.Reflection;

namespace Machine.Specifications.GallioAdapter.Tests
{
    /// <summary>
    /// This is a helper class to wrap the underlying Gallio test runner
    /// </summary>
    public class GallioRunner
    {
        public static void RunAllSpecsInAssemblyOf<T>()
        {            
            SampleRunner runner = new SampleRunner();
            runner.AddAssembly(typeof(T).Assembly);

            SafelyRun(runner);
        }

        public static TestStepRun RunAllSpecsFor<T>()
        {
            SampleRunner runner = new SampleRunner();
            runner.AddFixture(typeof(T));

            SafelyRun(runner);

            return runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(T)));            
        }

        private static void SafelyRun(SampleRunner runner)
        {            
            if (runner.TestPackage.Files.Count != 0)
                runner.Run();
        }
    }
}
