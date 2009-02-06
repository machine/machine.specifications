using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class BehaviorElement : SpecificationElement
  {
    public BehaviorElement(IUnitTestProvider provider,
                           ContextElement context,
                           IProjectModelElement project,
                           string declaringTypeName,
                           string fieldName)
      : base(provider, context, project, declaringTypeName, fieldName)
    {
    }

    public override string GetTitle()
    {
      return string.Format("{0}, behaves like {1}", Context.GetTitle(), FieldName.ReplaceUnderscores());
    }

    public override string GetKind()
    {
      return "Behavior";
    }
  }
}