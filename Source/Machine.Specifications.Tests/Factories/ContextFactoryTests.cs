using Machine.Specifications.Model;
using NUnit.Framework;

namespace Machine.Specifications.Factories
{
  [TestFixture]
  public class ContextFactoryTests : With<ContextWithSingleSpecification>
  {
    [Test]
    public void ShouldSetType()
    {
      context.Type.Name.ShouldEqual("ContextWithSingleSpecification");
    }
  }
}