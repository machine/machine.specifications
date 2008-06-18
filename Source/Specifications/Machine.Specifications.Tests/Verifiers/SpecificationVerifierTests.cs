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
  public class SpecificationVerifier_Verify_SpecificationsFromExample : TestsFor<ContextVerifier>
  {
    private IEnumerable<ContextVerificationResult> results;
    public override void BeforeEachTest()
    {
      AssemblyExplorer explorer = new AssemblyExplorer();
      var specifications = explorer.FindContextsIn(typeof(Account).Assembly);
      results = Target.VerifyContext(specifications);
    }

    [Test]
    public void ShouldBeExactlyOneResult()
    {
      results.Count().ShouldEqual(2);
    }

    [Test]
    public void ShouldBeExactlyThreeSpecificationResults()
    {
      results.SelectMany(x => x.SpecificationResults).Count().ShouldEqual(3);
    }
  }
}
