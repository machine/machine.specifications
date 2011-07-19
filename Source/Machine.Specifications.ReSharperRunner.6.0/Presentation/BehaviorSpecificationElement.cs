using System.Collections.Generic;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class BehaviorSpecificationElement : FieldElement
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

    public static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider)
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(provider.Solution, projectId) as IProject;
      if (project == null)
        return null;

      var behavior = parentElement as BehaviorElement;
      if (behavior == null)
        return null;

      var typeName = parent.GetAttribute("typeName");
      var methodName = parent.GetAttribute("methodName");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));

      return new BehaviorSpecificationElement(provider, behavior, ProjectModelElementEnvoy.Create(project), typeName, methodName, isIgnored);
    }
  }
}