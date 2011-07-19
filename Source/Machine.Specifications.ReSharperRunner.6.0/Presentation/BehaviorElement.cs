using System.Collections.Generic;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class BehaviorElement : FieldElement
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

    public override void WriteToXml(XmlElement parent)
    {
      base.WriteToXml(parent);
      parent.SetAttribute("typeFQN", FullyQualifiedTypeName);
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
      var fullyQualifiedTypeName = parent.GetAttribute("typeFQN");

      return new BehaviorElement(provider, context, ProjectModelElementEnvoy.Create(project), typeName, methodName, isIgnored, fullyQualifiedTypeName);
    }
  }
}