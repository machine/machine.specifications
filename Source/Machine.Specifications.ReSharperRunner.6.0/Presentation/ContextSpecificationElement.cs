using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class ContextSpecificationElement : FieldElement
  {
    readonly IEnumerable<UnitTestElementCategory> _categories;

    public ContextSpecificationElement(MSpecUnitTestProvider provider,
      // ReSharper disable SuggestBaseTypeForParameter
                                       ContextElement context,
      // ReSharper restore SuggestBaseTypeForParameter
                                       ProjectModelElementEnvoy project,
                                       string declaringTypeName,
                                       string fieldName,
                                       IEnumerable<string> tags,
                                       bool isIgnored)
      : base(provider, context, project, declaringTypeName, fieldName, isIgnored || context.Explicit)
    {
      if (tags != null)
      {
        _categories = UnitTestElementCategory.Create(tags);
      }
    }

    public ContextElement Context
    {
      get { return (ContextElement)Parent; }
    }

    public override string Kind
    {
      get { return "Specification"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return _categories; }
    }
  }
}
