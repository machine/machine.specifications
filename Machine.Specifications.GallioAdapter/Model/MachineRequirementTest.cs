using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model;
using Gallio.Reflection;
using Machine.Specifications.Model;

namespace Machine.SpecificationsAdapter.Model
{
  public class MachineRequirementTest : BaseTest
  {
    public MachineRequirementTest(Requirement requirement) : base(requirement.ItClause, Reflector.Wrap(requirement.Field))
    {
      this.Kind = TestKinds.Test;
      this.IsTestCase = true;
    }
  }
}
