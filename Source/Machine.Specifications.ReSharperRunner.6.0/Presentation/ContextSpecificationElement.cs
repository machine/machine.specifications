using System.Collections.Generic;
using System.Linq;
using System.Xml;

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

        get {  return "Specification"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return _categories; }
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider)
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(provider.Solution, projectId) as IProject;
      if (project == null)
        return null;

      var context = parentElement as ContextElement;
      if (context == null)
        return null;

      var typeName = parent.GetAttribute("typeName");
      var methodName = parent.GetAttribute("methodName");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));

      return new ContextSpecificationElement(provider, context, ProjectModelElementEnvoy.Create(project), typeName, methodName, Enumerable.Empty<string>(), isIgnored);
    }
  }
}
