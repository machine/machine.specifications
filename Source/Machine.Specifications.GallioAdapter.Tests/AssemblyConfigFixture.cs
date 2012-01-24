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
  public class AssemblyConfigFixture
  {
    static bool _running = false;

    [SetUp]
    public void Setup()
    {
      if (!_running)
      {
        //System.Diagnostics.Debugger.Launch();

        _running = true;
      }
    }
  }
}
