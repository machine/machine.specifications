using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif

using Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers;
using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  internal class FileExplorer : IRecursiveElementProcessor
  {
    readonly UnitTestElementLocationConsumer _consumer;
    readonly IEnumerable<IElementHandler> _elementHandlers;
    readonly IFile _file;
    readonly CheckForInterrupt _interrupted;

    public FileExplorer(MSpecUnitTestProvider provider,
#if RESHARPER_61
                        IUnitTestElementManager manager,
                        PsiModuleManager psiModuleManager,
                        CacheManager cacheManager,
#endif
                        UnitTestElementLocationConsumer consumer,
                        IFile file,
                        CheckForInterrupt interrupted)
    {
      if (file == null)
      {
        throw new ArgumentNullException("file");
      }

      if (provider == null)
      {
        throw new ArgumentNullException("provider");
      }

      _consumer = consumer;
      _file = file;
      _interrupted = interrupted;

      IProject project = file.GetSourceFile().ToProjectFile().GetProject();
      var projectEnvoy = new ProjectModelElementEnvoy(project);
      string assemblyPath = UnitTestManager.GetOutputAssemblyPath(project).FullPath;

      var cache = new ElementCache();

#if RESHARPER_61
      var contextFactory = new ContextFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, assemblyPath, cache);
      var contextSpecificationFactory = new ContextSpecificationFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, cache);
      var behaviorFactory = new BehaviorFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy, cache);
      var behaviorSpecificationFactory = new BehaviorSpecificationFactory(provider, manager, psiModuleManager, cacheManager, project, projectEnvoy);
#else
      var contextFactory = new ContextFactory(provider, project, projectEnvoy, assemblyPath, cache);
      var contextSpecificationFactory = new ContextSpecificationFactory(provider, project, projectEnvoy, cache);
      var behaviorFactory = new BehaviorFactory(provider, project, projectEnvoy, cache);
      var behaviorSpecificationFactory = new BehaviorSpecificationFactory(provider, project, projectEnvoy);
#endif

      _elementHandlers = new List<IElementHandler>
                         {
                           new ContextElementHandler(contextFactory),
                           new ContextSpecificationElementHandler(contextSpecificationFactory),
                           new BehaviorElementHandler(behaviorFactory, behaviorSpecificationFactory)
                         };
    }

    public bool InteriorShouldBeProcessed(ITreeNode element)
    {
      if (element is ITypeMemberDeclaration)
      {
        return element is ITypeDeclaration;
      }

      return true;
    }

    public void ProcessBeforeInterior(ITreeNode element)
    {
      var handler = _elementHandlers.FirstOrDefault(x => x.Accepts(element));
      if (handler == null)
      {
        return;
      }

      foreach (var elementDisposition in handler.AcceptElement(element, _file))
      {
        if (elementDisposition != null && elementDisposition.UnitTestElement != null)
        {
          _consumer(elementDisposition);
        }
      }
    }

    public void ProcessAfterInterior(ITreeNode element)
    {
      _elementHandlers
        .Where(x => x.Accepts(element))
        .Reverse()
        .ToList()
        .ForEach(x => x.Cleanup(element));
    }

    public bool ProcessingIsFinished
    {
      get
      {
        if (_interrupted())
        {
          throw new ProcessCancelledException();
        }

        return false;
      }
    }
  }
}