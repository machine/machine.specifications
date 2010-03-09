using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{
    using Example;

    [TestFixture]
    public class AssemblyTestsFixture
    {
        [SetUp]
        public void Setup()
        {
            GallioRunner.RunAllSpecsInAssemblyOf<TestAssemblyContext>();
        }

        [Test]
        public void ShouldRunOnAssemblyStartMethodOfAssemblyContext()
        {
            TestAssemblyContext.OnAssemblyStartRun.ShouldBeTrue();
        }

        [Test]
        public void ShouldRunOnAssemblyCompleteMethodOfAssemblyContext()
        {
            TestAssemblyContext.OnAssemblyCompleteRun.ShouldBeTrue();
        }       
    }
}
