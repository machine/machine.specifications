using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class BehaviorElement : FieldElement
  {
    public BehaviorElement(MSpecUnitTestProvider provider,
      // ReSharper disable SuggestBaseTypeForParameter
                           ContextElement context,
      // ReSharper restore SuggestBaseTypeForParameter
                           ProjectModelElementEnvoy projectEnvoy,
                           string declaringTypeName,
                           string fieldName,
                           bool isIgnored,
                           string fullyQualifiedTypeName)
      : base(provider, context, projectEnvoy, declaringTypeName, fieldName, isIgnored || context.Explicit)
    {
      FullyQualifiedTypeName = fullyQualifiedTypeName;
    }

    public ContextElement Context
    {
      get { return (ContextElement)Parent; }
    }

    public string FullyQualifiedTypeName { get; private set; }

    public override string GetTitlePrefix()
    {
      return "behaves like";
    }

    public override string Kind
    {
      get { return "Behavior"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return UnitTestElementCategory.Uncategorized; }
    }
  }
}