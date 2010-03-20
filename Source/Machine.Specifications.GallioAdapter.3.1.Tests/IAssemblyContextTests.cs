using Machine.Specifications.Example;
using NUnit.Framework;

namespace Machine.Specifications.GallioAdapter.Tests
{ 
  [TestFixture]
  public class IAssemblyContextTests
  {
    [SetUp]
    public void Setup()
    {
      GallioRunner.RunAssemblyOf<TestAssemblyContext>();
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
