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
      return new RemoteTaskRunnerInfo(typeof(RecursiveMSpecTaskRunner));
    }

    public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
    {
      ContextSpecificationElement contextSpecification = element as ContextSpecificationElement;
      if (contextSpecification != null)
      {
        var context = contextSpecification.Context;
        return new List<UnitTestTask>
               {
                 new UnitTestTask(null,
                                  new AssemblyLoadTask(context.AssemblyLocation)),
                 new UnitTestTask(context,
                                  new ContextTask(ProviderId,
                                                  context.AssemblyLocation,
                                                  context.GetTypeClrName(),
                                                  // TODO: explicitElements.Contains(context)
                                                  false)),
                 new UnitTestTask(contextSpecification,
                                  new ContextSpecificationTask(ProviderId,
                                                        context.AssemblyLocation,
                                                        context.GetTypeClrName(),
                                                        contextSpecification.FieldName,
                                                        // TODO: explicitElements.Contains(specification)
                                                        false)
                   )
               };
      }

      BehaviorElement behavior = element as BehaviorElement;
      if (behavior != null)
      {
        var context = behavior.Context;
        return new List<UnitTestTask>
               {
                 new UnitTestTask(null,
                                  new AssemblyLoadTask(context.AssemblyLocation)),
                 new UnitTestTask(context,
                                  new ContextTask(ProviderId,
                                                  context.AssemblyLocation,
                                                  context.GetTypeClrName(),
                                                  // TODO: explicitElements.Contains(context)
                                                  false)),
                 new UnitTestTask(behavior,
                                  new BehaviorTask(ProviderId,
                                                   context.AssemblyLocation,
                                                   context.GetTypeClrName(),
                                                   behavior.FieldName,
                                                   // TODO: explicitElements.Contains(behavior)
                                                   false))
               };
      }

      BehaviorSpecificationElement behaviorSpecification = element as BehaviorSpecificationElement;
      if (behaviorSpecification != null)
      {
        var context = behaviorSpecification.Behavior.Context;
        return new List<UnitTestTask>
               {
                 new UnitTestTask(null,
                                  new AssemblyLoadTask(context.AssemblyLocation)),
                 new UnitTestTask(context,
                                  new ContextTask(ProviderId,
                                                  context.AssemblyLocation,
                                                  context.GetTypeClrName(),
                                                  // TODO: explicitElements.Contains(context)
                                                  false)),
                 new UnitTestTask(behavior,
                                  new BehaviorTask(ProviderId,
                                                   context.AssemblyLocation,
                                                   context.GetTypeClrName(),
                                                   behaviorSpecification.Behavior.FieldName,
                                                   // TODO: explicitElements.Contains(behavior)
                                                   false)),
                 new UnitTestTask(behaviorSpecification,
                                  new BehaviorSpecificationTask(ProviderId,
                                                        context.AssemblyLocation,
                                                        context.GetTypeClrName(),
                                                        behaviorSpecification.FieldName,
                                                        behaviorSpecification.GetTypeClrName(),
                                                        // TODO: explicitElements.Contains(specification)
                                                        false)
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

      if ((x is ContextSpecificationElement || x is BehaviorElement) && y is ContextElement)
      {
        return -1;
      }

      if (x is ContextElement && (y is ContextSpecificationElement || y is BehaviorElement))
      {
        return 1;
      }

      if (x is ContextSpecificationElement && y is BehaviorElement)
      {
        return 1;
      }

      if (x is BehaviorElement && y is ContextSpecificationElement)
      {
        return -1;
      }

      return StringComparer.CurrentCultureIgnoreCase.Compare(x.GetTitle(), y.GetTitle());
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