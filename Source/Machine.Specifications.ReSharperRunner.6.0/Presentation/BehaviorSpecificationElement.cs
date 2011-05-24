using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class BehaviorSpecificationElement : FieldElement
  {
    public BehaviorSpecificationElement(MSpecUnitTestProvider provider,
      // ReSharper disable SuggestBaseTypeForParameter
                                        BehaviorElement behavior,
      // ReSharper restore SuggestBaseTypeForParameter
                                        ProjectModelElementEnvoy projectEnvoy,
                                        string declaringTypeName,
                                        string fieldName,
                                        bool isIgnored)
      : base(provider, behavior, projectEnvoy, declaringTypeName, fieldName, isIgnored || behavior.Explicit)
    {
    }

    public BehaviorElement Behavior
    {
      get { return (BehaviorElement)Parent; }
    }

    public override string Kind
    {
      get { return "Behavior Specification"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return UnitTestElementCategory.Uncategorized; }
    }
  }
}