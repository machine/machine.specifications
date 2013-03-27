using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [FileUnitTestExplorer]
  public class MspecTestFileExplorer : IUnitTestFileExplorer
  {
    readonly FileExplorer _fileExplorer;
    readonly MSpecUnitTestProvider _provider;

    public MspecTestFileExplorer(MSpecUnitTestProvider provider,
                                 FileExplorer fileExplorer)
    {
      _provider = provider;
      _fileExplorer = fileExplorer;
    }

    public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
    {
      if ((psiFile.Language.Name == "CSHARP") || (psiFile.Language.Name == "VBASIC"))
      {
        psiFile.ProcessDescendants(_fileExplorer);
      }
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}