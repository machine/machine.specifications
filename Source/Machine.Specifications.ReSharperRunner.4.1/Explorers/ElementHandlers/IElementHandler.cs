using System.Collections.Generic;

using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER_5 || RESHARPER_6
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal interface IElementHandler
  {
#if RESHARPER_6
    bool Accepts(ITreeNode element);
    IEnumerable<UnitTestElementDisposition> AcceptElement(ITreeNode element, IFile file);
#else
    bool Accepts(IElement element);
    IEnumerable<UnitTestElementDisposition> AcceptElement(IElement element, IFile file);
#endif
  }
}