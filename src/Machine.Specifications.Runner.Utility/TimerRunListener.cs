using System.Collections.Generic;
using System.Diagnostics;

namespace Machine.Specifications.Runner.Utility
{
    public class TimingRunListener : ISpecificationRunListener
    {
        private readonly Stopwatch assemblyTimer = new Stopwatch();

        private readonly Dictionary<AssemblyInfo, long> assemblyTimes = new Dictionary<AssemblyInfo, long>();

        private readonly Stopwatch contextTimer = new Stopwatch();

        private readonly Dictionary<ContextInfo, long> contextTimes = new Dictionary<ContextInfo, long>();

        private readonly Stopwatch runTimer = new Stopwatch();

        private readonly Stopwatch specificationTimer = new Stopwatch();

        private readonly Dictionary<SpecificationInfo, long> specificationTimes = new Dictionary<SpecificationInfo, long>();

        public void OnRunStart()
        {
            runTimer.Restart();
        }

        public void OnRunEnd()
        {
            runTimer.Stop();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            assemblyTimer.Restart();
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            assemblyTimer.Stop();
            assemblyTimes[assembly] = assemblyTimer.ElapsedMilliseconds;
        }

        public void OnContextStart(ContextInfo context)
        {
            contextTimer.Restart();
            specificationTimer.Restart();
        }

        public void OnContextEnd(ContextInfo context)
        {
            contextTimer.Stop();
            contextTimes[context] = contextTimer.ElapsedMilliseconds;
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            if (!specificationTimer.IsRunning)
            {
                specificationTimer.Restart();
            }
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            specificationTimer.Stop();
            specificationTimes[specification] = specificationTimer.ElapsedMilliseconds;
        }

        public void OnFatalError(ExceptionResult exception)
        {
        }

        public long GetSpecificationTime(SpecificationInfo specificationInfo)
        {
            if (specificationTimes.ContainsKey(specificationInfo))
            {
                return specificationTimes[specificationInfo];
            }

            return -1;
        }

        public long GetContextTime(ContextInfo contextInfo)
        {
            if (contextTimes.ContainsKey(contextInfo))
            {
                return contextTimes[contextInfo];
            }

            return -1;
        }

        public long GetAssemblyTime(AssemblyInfo assemblyInfo)
        {
            if (assemblyTimes.ContainsKey(assemblyInfo))
            {
                return assemblyTimes[assemblyInfo];
            }

            return -1;
        }

        public long GetRunTime()
        {
            return runTimer.ElapsedMilliseconds;
        }
    }
}
