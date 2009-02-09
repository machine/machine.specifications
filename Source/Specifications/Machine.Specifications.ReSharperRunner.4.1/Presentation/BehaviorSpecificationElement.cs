using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestExplorer;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class BehaviorSpecificationElement : FieldElement
  {
    public BehaviorSpecificationElement(IUnitTestProvider provider,
// ReSharper disable SuggestBaseTypeForParameter
                                        BehaviorElement behavior,
// ReSharper restore SuggestBaseTypeForParameter
                                        IProjectModelElement project,
                                        string declaringTypeName,
                                        string fieldName,
                                        bool isIgnored)
      : base(provider, behavior, project, declaringTypeName, fieldName, isIgnored || behavior.IsExplicit)
    {
    }

    public BehaviorElement Behavior
    {
      get { return (BehaviorElement) Parent; }
    }

    public override string GetKind()
    {
      return "Behavior Specification";
    }
  }
}