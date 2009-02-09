using System;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Application;
using JetBrains.CommonControls;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Explorers;
using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Runners;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner
{
  [UnitTestProvider]
  internal class MSpecUnitTestProvider : IUnitTestProvider
  {
    internal const string ProviderId = "Machine.Specifications";
    static readonly Presenter Presenter = new Presenter();

    public MSpecUnitTestProvider()
    {
      Debug.Listeners.Add(new DefaultTraceListener());
    }

    #region Implementation of IUnitTestProvider
    public string ID
    {
      get { return ProviderId; }
    }

    public string Serialize(UnitTestElement element)
    {
      return null;
    }

    public UnitTestElement Deserialize(ISolution solution, string elementString)
    {
      return null;
    }

    public void ProfferConfiguration(TaskExecutorConfiguration configuration, UnitTestSession session)
    {
    }

    public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
    {
    }

    public void ExploreAssembly(IMetadataAssembly assembly, IProject project, UnitTestElementConsumer consumer)
    {
      AssemblyExplorer explorer = new AssemblyExplorer(this, assembly, project, consumer);
      explorer.Explore();
    }

    public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
    {
      if (psiFile == null)
      {
        throw new ArgumentNullException("psiFile");
      }

      psiFile.ProcessDescendants(new FileExplorer(this, consumer, psiFile, interrupted));
    }

    public void ExploreExternal(UnitTestElementConsumer consumer)
    {
    }

    public RemoteTaskRunnerInfo GetTaskRunnerInfo()
    {
      return new RemoteTaskRunnerInfo(typeof(TaskRunner));
    }

    public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
    {
      Debug.WriteLine(element.GetType().FullName + ": " + element.GetTitle());

      SpecificationElement specification = element as SpecificationElement;
      if (specification != null)
      {
        var context = specification.Context;
        return new List<UnitTestTask>
               {
                 new UnitTestTask(null,
                                  new AssemblyLoadTask(context.AssemblyLocation)),
                 new UnitTestTask(context,
                                  new ContextTask(ProviderId,
                                                  context.AssemblyLocation,
                                                  context.GetTypeClrName(),
                                                  // TODO
                                                  false)),//explicitElements.Contains(context))),
                 new UnitTestTask(specification,
                                  new SpecificationTask(ProviderId,
                                                        context.GetTypeClrName(),
                                                        specification.FieldName,
                                                        // TODO
                                                        false,//explicitElements.Contains(specification),
                                                        specification.Context.AssemblyLocation)
                   )
               };
      }

      if (element is ContextElement)
      {
        return EmptyArray<UnitTestTask>.Instance;
      }

      throw new ArgumentException(String.Format("Element is not a Machine.Specification element: '{0}'", element));
    }

    public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
    {
      if (Equals(x, y))
      {
        return 0;
      }

      int compare = StringComparer.CurrentCultureIgnoreCase.Compare(x.GetTypeClrName(), y.GetTypeClrName());
      if (compare != 0)
      {
        return compare;
      }

      if (x is SpecificationElement && y is ContextElement)
      {
        return -1;
      }

      if (x is ContextElement && y is SpecificationElement)
      {
        return 1;
      }

      if (x is ContextElement && y is ContextElement)
      {
        return 0;
      }

      SpecificationElement xe = (SpecificationElement) x;
      SpecificationElement ye = (SpecificationElement) y;
      return xe.GetTitle().CompareTo(ye.GetTitle());
    }

    public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
    {
      Presenter.UpdateItem(element, node, item, state);
    }

    public bool IsUnitTestElement(IDeclaredElement element)
    {
      return element.IsContext() || element.IsSpecification();
    }
    #endregion
  }
}