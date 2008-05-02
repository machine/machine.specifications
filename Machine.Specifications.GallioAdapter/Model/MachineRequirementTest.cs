using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Machine.Specifications.GallioAdapter.Services;
using Machine.Specifications.Model;

namespace Machine.Specifications.GallioAdapter.Model
{
  public class MachineRequirementTest : MachineTest
  {
    private readonly Requirement _requirement;
    private readonly Specification _specification;

    /*
    public Requirement Requirement
    {
      get { return _requirement; }
    }
    */

    public MachineRequirementTest(Requirement requirement) : base(requirement.ItClause, Reflector.Wrap(requirement.Field))
    {
      this.Kind = TestKinds.Test;
      this.IsTestCase = true;
      _requirement = requirement;
    }

    public RequirementVerificationResult Execute()
    {
      MachineSpecificationTest parent = this.Parent as MachineSpecificationTest;
      if (parent == null) throw new Exception("Requirement has non-specification parent???");

      var result = parent.Specification.VerifyRequirement(_requirement);
      return result;
    }
  }
}
