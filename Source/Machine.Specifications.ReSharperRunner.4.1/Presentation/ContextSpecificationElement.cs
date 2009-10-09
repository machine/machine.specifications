using JetBrains.ProjectModel;
#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class ContextSpecificationElement : FieldElement
  {
    public ContextSpecificationElement(IUnitTestProvider provider,
                                       // ReSharper disable SuggestBaseTypeForParameter
                                       ContextElement context,
                                       // ReSharper restore SuggestBaseTypeForParameter
                                       IProjectModelElement project,
                                       string declaringTypeName,
                                       string fieldName,
                                       bool isIgnored)
      : base(provider, context, project, declaringTypeName, fieldName, isIgnored || context.IsExplicit)
    {
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