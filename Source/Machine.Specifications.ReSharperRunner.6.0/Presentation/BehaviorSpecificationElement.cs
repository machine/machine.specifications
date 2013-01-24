using System;
using System.Collections.Generic;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif

using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class BehaviorSpecificationElement : FieldElement
  {
    readonly string _id;

    public BehaviorSpecificationElement(MSpecUnitTestProvider provider,
                                        PsiModuleManager psiModuleManager,
                                        CacheManager cacheManager,
      // ReSharper disable SuggestBaseTypeForParameter
                                        BehaviorElement behavior,
      // ReSharper restore SuggestBaseTypeForParameter
                                        ProjectModelElementEnvoy projectEnvoy,
                                        IClrTypeName declaringTypeName,
                                        string fieldName,
                                        bool isIgnored)
      : base(provider, psiModuleManager, cacheManager, behavior, projectEnvoy, declaringTypeName, fieldName, isIgnored || behavior.Explicit)
    {
      _id = CreateId(behavior, fieldName);
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

    public static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider, ISolution solution
#if RESHARPER_61
      , IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager
#endif
      )
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

      return BehaviorSpecificationFactory.GetOrCreateBehaviorSpecification(provider,
#if RESHARPER_61
        manager, psiModuleManager, cacheManager,
#endif
        project, behavior, ProjectModelElementEnvoy.Create(project), new ClrTypeName(typeName), methodName, isIgnored);
    }

    public override string Id
    {
      get { return _id; }
    }

    public static string CreateId(BehaviorElement behaviorElement, string fieldName)
    {
      return String.Format("{0}.{1}", behaviorElement.Id, fieldName);
    }
  }
}