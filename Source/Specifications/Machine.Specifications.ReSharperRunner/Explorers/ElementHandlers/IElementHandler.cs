using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestExplorer;

namespace Machine.Specifications.ReSharperRunner.Explorers.ElementHandlers
{
  internal interface IElementHandler
  {
    bool Accepts(IElement element);
    UnitTestElementDisposition AcceptElement(IElement element, IFile file);
  }
}