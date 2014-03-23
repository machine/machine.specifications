using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Shims;

using ICache = Machine.Specifications.ReSharperRunner.Shims.ICache;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class BehaviorSpecificationElement : FieldElement
  {
    readonly string _id;

    public BehaviorSpecificationElement(MSpecUnitTestProvider provider,
                                        IPsi psiModuleManager,
                                        ICache cacheManager,
                                        ProjectModelElementEnvoy projectEnvoy,
                                        BehaviorElement behavior,
                                        IClrTypeName declaringTypeName,
                                        string fieldName,
                                        bool isIgnored
      )
      : base(provider,
             psiModuleManager,
             cacheManager,
             behavior,
             projectEnvoy,
             declaringTypeName,
             fieldName,
             isIgnored || behavior.Explicit)
    {
      _id = CreateId(behavior, fieldName);
    }

    public BehaviorElement Behavior
    {
      get { return (BehaviorElement) Parent; }
    }

    public override string Kind
    {
      get { return "Behavior Specification"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get
      {
        var parent = Parent ?? Behavior;
        if (parent == null)
        {
          return UnitTestElementCategory.Uncategorized;
        }

        return parent.Categories;
      }
    }

    public override string Id
    {
      get { return _id; }
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent,
                                               IUnitTestElement parentElement,
                                               ISolution solution,
                                               BehaviorSpecificationFactory factory)
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(solution, projectId) as IProject;
      if (project == null)
      {
        return null;
      }

      var behavior = parentElement as BehaviorElement;
      if (behavior == null)
      {
        return null;
      }

      var typeName = parent.GetAttribute("typeName");
      var methodName = parent.GetAttribute("methodName");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));

      return factory.GetOrCreateBehaviorSpecification(behavior,
                                                      new ClrTypeName(typeName),
                                                      methodName,
                                                      isIgnored);
    }

    public static string CreateId(BehaviorElement behaviorElement, string fieldName)
    {
      var result = new[] {behaviorElement.Id, fieldName};
      return result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
    }
  }
}
