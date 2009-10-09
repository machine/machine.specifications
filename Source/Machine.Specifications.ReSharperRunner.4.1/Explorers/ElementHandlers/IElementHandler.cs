using System.Collections.Generic;

using JetBrains.ReSharper.Psi.Tree;
#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal interface IElementHandler
  {
    bool Accepts(IElement element);
    IEnumerable<UnitTestElementDisposition> AcceptElement(IElement element, IFile file);
  }
}