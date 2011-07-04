using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  [FileUnitTestExplorer] 
  public class MspecTestFileExplorer : IUnitTestFileExplorer
  {
    MSpecUnitTestProvider _provider;

    public MspecTestFileExplorer(MSpecUnitTestProvider provider)
    {
      _provider = provider;
    }

    public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
    {
      if ((psiFile.Language.Name == "CSHARP") || (psiFile.Language.Name == "VBASIC"))
      {
        psiFile.ProcessDescendants(new FileExplorer(_provider, consumer, psiFile, interrupted));
      }
    }

    public IUnitTestProvider Provider { get { return _provider; }}
  }
}