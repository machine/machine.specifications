using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Gallio.Runtime;

namespace Machine.Specifications.GallioAdapter.Tests
{
    /// <summary>
    /// A really easy way to make sure the debugger is attached when tests are running
    /// </summary>
    [SetUpFixture]
    public class AssemblyConfig
    {
        public static bool running = false;
        [SetUp]
        public void Setup()
        {
            if (!running)
            {
                //System.Diagnostics.Debugger.Launch();

                running = true;
            }
        }
    }
}
