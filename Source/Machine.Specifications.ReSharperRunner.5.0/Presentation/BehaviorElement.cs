using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class BehaviorElement : FieldElement
  {
      private readonly string _fullyQualifiedTypeName;

      public BehaviorElement(IUnitTestProvider provider,
                           // ReSharper disable SuggestBaseTypeForParameter
                           ContextElement context,
                           // ReSharper restore SuggestBaseTypeForParameter
                           ProjectModelElementEnvoy projectEnvoy,
                           string declaringTypeName,
                           string fieldName,
                           bool isIgnored,
                           string fullyQualifiedTypeName)
      : base(provider, context, projectEnvoy, declaringTypeName, fieldName, isIgnored || context.IsExplicit)
    {
        _fullyQualifiedTypeName = fullyQualifiedTypeName;
    }

      public ContextElement Context
    {
      get { return (ContextElement)Parent; }
    }

    public string FullyQualifiedTypeName { get { return _fullyQualifiedTypeName; } }

    public override string GetTitlePrefix()
    {
      return "behaves like";
    }

    public override string GetKind()
    {
      return "Behavior";
    }
  }
}