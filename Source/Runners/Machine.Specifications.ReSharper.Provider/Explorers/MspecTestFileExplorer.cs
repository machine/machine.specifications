namespace Machine.Specifications.ReSharperProvider.Explorers
{
    using System;

    using JetBrains.Application;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Factories;

    [FileUnitTestExplorer]
    public class MspecTestFileExplorer : IUnitTestFileExplorer
    {
        readonly ElementFactories _factories;
        readonly MSpecUnitTestProvider _provider;

        public MspecTestFileExplorer(MSpecUnitTestProvider provider,
                                     ElementFactories factories)
        {
            this._provider = provider;
            this._factories = factories;
        }

#if RESHARPER_81
    public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, Func<bool> interrupted)
    {
      if ((psiFile.Language.Name == "CSHARP") || (psiFile.Language.Name == "VBASIC"))
      {
        psiFile.ProcessDescendants(new FileExplorer(_provider, _factories, psiFile, consumer, interrupted));
      }
    }
#else
        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            Func<bool> interruptedFunc = () => interrupted();

            if ((psiFile.Language.Name == "CSHARP") || (psiFile.Language.Name == "VBASIC"))
            {
                psiFile.ProcessDescendants(new FileExplorer(this._provider, this._factories, psiFile, consumer, interruptedFunc));
            }
        }
#endif

        public IUnitTestProvider Provider
        {
            get { return this._provider; }
        }
    }
}