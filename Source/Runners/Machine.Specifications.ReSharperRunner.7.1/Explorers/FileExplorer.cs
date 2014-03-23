using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers;
using Machine.Specifications.ReSharperRunner.Factories;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  public class FileExplorer : IRecursiveElementProcessor
  {
    readonly UnitTestElementLocationConsumer _consumer;
    readonly IEnumerable<IElementHandler> _elementHandlers;
    readonly IFile _file;
    readonly Func<bool> _interrupted;
    readonly string _assemblyPath;

    public FileExplorer(MSpecUnitTestProvider provider,
                        ElementFactories factories,
                        IFile file,
                        UnitTestElementLocationConsumer consumer,
                        Func<bool> interrupted)
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

      var project = file.GetSourceFile().ToProjectFile().GetProject();
        
#if !RESHARPER_8
      _assemblyPath = UnitTestManager.GetOutputAssemblyPath(project).FullPath;
#else
      _assemblyPath = project.GetOutputFilePath().FullPath;
#endif

      _elementHandlers = new List<IElementHandler>
                         {
                           new ContextElementHandler(factories),
                           new ContextSpecificationElementHandler(factories),
                           new BehaviorElementHandler(factories)
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

      foreach (var elementDisposition in handler.AcceptElement(_assemblyPath, _file, element))
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