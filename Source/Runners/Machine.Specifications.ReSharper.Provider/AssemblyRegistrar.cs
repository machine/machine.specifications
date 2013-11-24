using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Runners;

namespace Machine.Specifications.ReSharper.Provider
{
    [SolutionComponent]
    public class AssemblyRegistrar
    {
        public AssemblyRegistrar(UnitTestingAssemblyLoader loader)
        {
            loader.RegisterAssembly(typeof(RecursiveMSpecTaskRunner).Assembly);
        }
    }
}