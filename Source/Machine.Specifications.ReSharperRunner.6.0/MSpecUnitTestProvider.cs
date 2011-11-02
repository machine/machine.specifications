using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif

using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Properties;
using Machine.Specifications.ReSharperRunner.Runners;

namespace Machine.Specifications.ReSharperRunner
{
  [UnitTestProvider]
  public class MSpecUnitTestProvider : IUnitTestProvider
  {
    const string ProviderId = "Machine.Specifications";
    readonly UnitTestElementComparer _unitTestElementComparer = new UnitTestElementComparer();
    private UnitTestManager _unitTestManager;

#if RESHARPER_61
    public MSpecUnitTestProvider()
    {
#else
    public MSpecUnitTestProvider(ISolution solution, PsiModuleManager psiModuleManager, CacheManager cacheManager)
    {
      Solution = solution;
      PsiModuleManager = psiModuleManager;
      CacheManager = cacheManager;
#endif
      Debug.Listeners.Add(new DefaultTraceListener());
    }

#if !RESHARPER_61
    public PsiModuleManager PsiModuleManager { get; private set; }
    public CacheManager CacheManager { get; private set; }

    public UnitTestManager UnitTestManager
    {
      get { return _unitTestManager ?? (_unitTestManager = Solution.GetComponent<UnitTestManager>());  }
    }
#endif

    public string ID
    {
      get { return ProviderId; }
    }

    public string Name
    {
      get { return ID; }
    }

    public Image Icon
    {
      get { return Resources.Logo; }
    }

    public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
    {
    }

    public void ExploreExternal(UnitTestElementConsumer consumer)
    {
    }

#if !RESHARPER_61
    public ISolution Solution { get; private set; }

    public void SerializeElement(XmlElement parent, IUnitTestElement element)
    {
      var e = element as ISerializableElement;
      if (e != null)
      {
        e.WriteToXml(parent);
        parent.SetAttribute("elementType", e.GetType().Name);
      }
    }

    public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
    {
      var typeName = parent.GetAttribute("elemenType");

      if (Equals(typeName, "ContextElement"))
        return ContextElement.ReadFromXml(parent, parentElement, this, Solution);
      if (Equals(typeName, "BehaviorElement"))
        return BehaviorElement.ReadFromXml(parent, parentElement, this, Solution);
      if (Equals(typeName, "BehaviorSpecificationElement"))
        return BehaviorSpecificationElement.ReadFromXml(parent, parentElement, this, Solution);
      if (Equals(typeName, "ContextSpecificationElement"))
        return ContextSpecificationElement.ReadFromXml(parent, parentElement, this, Solution);

      return null;
    }
#endif

    public RemoteTaskRunnerInfo GetTaskRunnerInfo()
    {
      return new RemoteTaskRunnerInfo(typeof(RecursiveMSpecTaskRunner));
    }

    public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
    {
      return _unitTestElementComparer.Compare(x, y);
    }

    public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
    {
      switch (elementKind)
      {
        case UnitTestElementKind.Test:
          return element is ContextSpecificationElement || element is BehaviorSpecificationElement;
        case UnitTestElementKind.TestContainer:
          return element is ContextElement || element is BehaviorElement;
      }

      return false;
    }

    public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
    {
      switch (elementKind)
      {
        case UnitTestElementKind.Test:
          return declaredElement.IsSpecification();
        case UnitTestElementKind.TestContainer:
          return declaredElement.IsContext() || declaredElement.IsBehavior();
      }

      return false;
    }

    public bool IsSupported(IHostProvider hostProvider)
    {
      return true;
    }
  }
}