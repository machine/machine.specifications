using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml;

using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Explorers;
using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Properties;
using Machine.Specifications.ReSharperRunner.Runners;

using IUnitTestProvider = JetBrains.ReSharper.UnitTestFramework.IUnitTestProvider;
using UnitTestElementLocationConsumer = JetBrains.ReSharper.UnitTestFramework.UnitTestElementLocationConsumer;

namespace Machine.Specifications.ReSharperRunner
{
    [UnitTestProvider]
    internal class MSpecUnitTestProvider : IUnitTestProvider
    {
      const string ProviderId = "Machine.Specifications";
        readonly UnitTestTaskFactory _taskFactory = new UnitTestTaskFactory(ProviderId);
        readonly UnitTestElementComparer _unitTestElementComparer = new UnitTestElementComparer();

        public MSpecUnitTestProvider(PsiModuleManager psiModuleManager, CacheManager cacheManager)
        {
          PsiModuleManager = psiModuleManager;
          CacheManager = cacheManager;
          Debug.Listeners.Add(new DefaultTraceListener());
        }

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

        public ISolution Solution
        {
            get { throw new NotImplementedException(); }
        }

        public PsiModuleManager PsiModuleManager { get; private set; }
        public CacheManager CacheManager { get; private set; }

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

        public void ExploreAssembly(string assemblyLocation, UnitTestElementConsumer consumer)
        {
            throw new NotImplementedException();
        }

        public RemoteTaskRunnerInfo GetTaskRunnerInfo()
        {
            return new RemoteTaskRunnerInfo(typeof(RecursiveMSpecTaskRunner));
        }

        // TODO: HADI - MOVE THIS TO GETTASKSEQUENCE 
        //public IList<UnitTestTask> GetTaskSequence(UnitTestElement element, IList<UnitTestElement> explicitElements)
        //{
        //    if (element is ContextSpecificationElement)
        //    {
        //        var contextSpecification = element as ContextSpecificationElement;
        //        var context = contextSpecification.Context;

        //        return new List<UnitTestTask>
        //       {
        //         _taskFactory.CreateAssemblyLoadTask(context),
        //         _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
        //         _taskFactory.CreateContextSpecificationTask(context,
        //                                                     contextSpecification,
        //                                                     explicitElements.Contains(contextSpecification))
        //       };
        //    }

        //    if (element is BehaviorElement)
        //    {
        //        var behavior = element as BehaviorElement;
        //        var context = behavior.Context;

        //        return new List<UnitTestTask>
        //       {
        //         _taskFactory.CreateAssemblyLoadTask(context),
        //         _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
        //         _taskFactory.CreateBehaviorTask(context, behavior, explicitElements.Contains(behavior))
        //       };
        //    }

        //    if (element is BehaviorSpecificationElement)
        //    {
        //        var behaviorSpecification = element as BehaviorSpecificationElement;
        //        var behavior = behaviorSpecification.Behavior;
        //        var context = behavior.Context;

        //        return new List<UnitTestTask>
        //       {
        //         _taskFactory.CreateAssemblyLoadTask(context),
        //         _taskFactory.CreateContextTask(context, explicitElements.Contains(context)),
        //         _taskFactory.CreateBehaviorTask(context, behavior, explicitElements.Contains(behavior)),
        //         _taskFactory.CreateBehaviorSpecificationTask(context,
        //                                                      behaviorSpecification,
        //                                                      explicitElements.Contains(behaviorSpecification))
        //       };
        //    }

        //    if (element is ContextElement)
        //    {
        //        return EmptyArray<UnitTestTask>.Instance;
        //    }

        //    throw new ArgumentException(String.Format("Element is not a Machine.Specifications element: '{0}'", element));
        //}

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