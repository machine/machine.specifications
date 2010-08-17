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
using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Runners;

namespace Machine.Specifications.ReSharperRunner
{
  [UnitTestProvider]
  internal class MSpecUnitTestProvider : IUnitTestProvider
  {
    const string ProviderId = "Machine.Specifications";
    static readonly Presenter Presenter = new Presenter();
    readonly UnitTestTaskFactory _taskFactory = new UnitTestTaskFactory(ProviderId);
    readonly UnitTestElementComparer _unitTestElementComparer = new UnitTestElementComparer();

    public MSpecUnitTestProvider()
    {
      Debug.Listeners.Add(new DefaultTraceListener());
    }

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
      return new RemoteTaskRunnerInfo(typeof(RecursiveMSpecTaskRunner));
    }

    public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
    {
      if (element is ContextSpecificationElement)
      {
        var contextSpecification = element as ContextSpecificationElement;
        var context = contextSpecification.Context;

        return new List<UnitTestTask>
               {
                 _taskFactory.CreateAssemblyLoadTask(context),
                 _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
                 _taskFactory.CreateContextSpecificationTask(context,
                                                             contextSpecification,
                                                             explicitElements.Contains(contextSpecification))
               };
      }

      if (element is BehaviorElement)
      {
        var behavior = element as BehaviorElement;
        var context = behavior.Context;

        return new List<UnitTestTask>
               {
                 _taskFactory.CreateAssemblyLoadTask(context),
                 _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
                 _taskFactory.CreateBehaviorTask(context, behavior, explicitElements.Contains(behavior))
               };
      }

      if (element is BehaviorSpecificationElement)
      {
        var behaviorSpecification = element as BehaviorSpecificationElement;
        var behavior = behaviorSpecification.Behavior;
        var context = behavior.Context;

        return new List<UnitTestTask>
               {
                 _taskFactory.CreateAssemblyLoadTask(context),
                 _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
                 _taskFactory.CreateBehaviorTask(context,
                                                 behavior,
                                                 explicitElements.Contains(behavior)),
                 _taskFactory.CreateBehaviorSpecificationTask(context,
                                                              behaviorSpecification,
                                                              explicitElements.Contains(behaviorSpecification))
               };
      }

      if (element is ContextElement)
      {
        return EmptyArray<UnitTestTask>.Instance;
      }

      throw new ArgumentException(String.Format("Element is not a Machine.Specifications element: '{0}'", element));
    }

    public int CompareUnitTestElements(UnitTestElement x, UnitTestElement y)
    {
      return _unitTestElementComparer.Compare(x, y);
    }

    public void Present(UnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
    {
      Presenter.UpdateItem(element, node, item, state);
    }

    public bool IsUnitTestElement(IDeclaredElement element)
    {
      return element.IsContext() || element.IsSpecification();
    }
  }
}