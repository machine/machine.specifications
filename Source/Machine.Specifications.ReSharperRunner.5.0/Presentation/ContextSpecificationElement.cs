using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class ContextSpecificationElement : FieldElement
  {
    public ContextSpecificationElement(IUnitTestProvider provider,
                                       // ReSharper disable SuggestBaseTypeForParameter
                                       ContextElement context,
                                       // ReSharper restore SuggestBaseTypeForParameter
                                       ProjectModelElementEnvoy project,
                                       string declaringTypeName,
                                       string fieldName,
                                       ICollection<string> tags,
                                       bool isIgnored)
      : base(provider, context, project, declaringTypeName, fieldName, isIgnored || context.IsExplicit)
    {
        if (tags != null)
        {
            AssignCategories(tags);
        }
    }

    public ContextElement Context
    {
      get { return (ContextElement)Parent; }
    }

    public override string GetKind()
    {
      return "Specification";
    }
  }
}
