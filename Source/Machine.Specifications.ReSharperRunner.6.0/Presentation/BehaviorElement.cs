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

using Machine.Specifications.Factories;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class BehaviorElement : FieldElement
  {
    public BehaviorElement(MSpecUnitTestProvider provider,
                           PsiModuleManager psiModuleManager,
                           CacheManager cacheManager,
      // ReSharper disable SuggestBaseTypeForParameter
                           ContextElement context,
      // ReSharper restore SuggestBaseTypeForParameter
                           ProjectModelElementEnvoy projectEnvoy,
                           string declaringTypeName,
                           string fieldName,
                           bool isIgnored,
                           string fullyQualifiedTypeName)
      : base(provider, psiModuleManager, cacheManager, context, projectEnvoy, declaringTypeName, fieldName, isIgnored || context.Explicit)
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

      var context = parentElement as ContextElement;
      if (context == null)
      {
        return null;
      }

      var typeName = parent.GetAttribute("typeName");
      var methodName = parent.GetAttribute("methodName");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));
      var fullyQualifiedTypeName = parent.GetAttribute("typeFQN");

      return Factories.BehaviorFactory.GetOrCreateBehavior(provider,
#if RESHARPER_61
                                                           manager, psiModuleManager, cacheManager,
#endif
                                                           project,
                                                           ProjectModelElementEnvoy.Create(project),
                                                           context,
                                                           typeName,
                                                           methodName,
                                                           isIgnored,
                                                           fullyQualifiedTypeName);
    }

    public override string Id
    {
      get { return CreateId(TypeName, FieldName); }
    }

    public static string CreateId(string contextElementTypeName, string fieldName)
    {
      var id = String.Format("{0}.{1}", contextElementTypeName, fieldName);
      System.Diagnostics.Debug.WriteLine("BE  " + id);
      return id;
    }
  }
}