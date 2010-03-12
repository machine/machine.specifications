using Machine.Specifications.Example;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{ 
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
