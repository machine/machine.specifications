using System;
using System.Reflection;

namespace Machine.Specifications.ReSharper.Runner
{
    public interface IReSharperSpecificationRunner
    {
        void StartRun(Assembly contextAssembly);
        void RunMember(Assembly contextAssembly, Type contextClass);
        void EndRun(Assembly contextAssembly);
    }
}