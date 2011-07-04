using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;

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

    public MSpecUnitTestProvider(ISolution solution, PsiModuleManager psiModuleManager, CacheManager cacheManager)
    {
      Solution = solution;
      PsiModuleManager = psiModuleManager;
      CacheManager = cacheManager;
      Debug.Listeners.Add(new DefaultTraceListener());
    }

    public PsiModuleManager PsiModuleManager { get; private set; }
    public CacheManager CacheManager { get; private set; }

    public string ID
    {
      get { return ProviderId; }
    }

    public string Name
    {
      get { return ID; }
    }

    public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
    {
      throw new NotImplementedException();
    }

    public Image Icon
    {
      get { return Resources.Logo; }
    }

    public ISolution Solution { get; private set; }

    public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
    {
    }

    public void ExploreExternal(UnitTestElementConsumer consumer)
    {
    }

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
          return element is ContextSpecificationElement;
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

    public void SerializeElement(XmlElement parent, IUnitTestElement element)
    {
      throw new NotImplementedException();
    }
  }
}