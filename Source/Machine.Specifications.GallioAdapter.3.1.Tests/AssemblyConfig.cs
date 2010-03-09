using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Gallio.Runtime;

namespace Machine.Specifications.GallioAdapter.Tests
{
    // Required to test the gallio integration using NUnit
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
