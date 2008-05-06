using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Example;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Testing;
using NUnit.Framework;

namespace Machine.Specifications.Verifiers
{
  [TestFixture]
  public class SpecificationVerifier_Verify_SpecificationsFromExample : TestsFor<DescriptionVerifier>
  {
    private IEnumerable<DescriptionVerificationResult> results;
    public override void BeforeEachTest()
    {
      AssemblyExplorer explorer = new AssemblyExplorer();
      var specifications = explorer.FindDescriptionsIn(typeof(Account).Assembly);
      results = Target.VerifyDescription(specifications);
    }

    [Test]
    public void ShouldBeExactlyThreeResults()
    {
      results.Count().ShouldEqual(3);
    }

    [Test]
    public void ShouldBeExactlyFiveRequirementResults()
    {
      results.SelectMany(x => x.RequirementResults).Count().ShouldEqual(5);
    }
  }
}
