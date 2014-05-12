using System.Collections.Generic;
using System.Diagnostics;

namespace Machine.Specifications.Runner.Utility
{
    public class TimingRunListener : ISpecificationRunListener
    {
        readonly Stopwatch _assemblyTimer = new Stopwatch();
        readonly Dictionary<AssemblyInfo, long> _assemblyTimes = new Dictionary<AssemblyInfo, long>();
        readonly Stopwatch _contextTimer = new Stopwatch();
        readonly Dictionary<ContextInfo, long> _contextTimes = new Dictionary<ContextInfo, long>();
        readonly Stopwatch _runTimer = new Stopwatch();
        readonly Stopwatch _specificationTimer = new Stopwatch();
        readonly Dictionary<SpecificationInfo, long> _specificationTimes = new Dictionary<SpecificationInfo, long>();

        public void OnRunStart()
        {
            this._runTimer.Restart();
        }

        public void OnRunEnd()
        {
            this._runTimer.Stop();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            this._assemblyTimer.Restart();
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            this._assemblyTimer.Stop();
            this._assemblyTimes[assembly] = this._assemblyTimer.ElapsedMilliseconds;
        }

        public void OnContextStart(ContextInfo context)
        {
            this._contextTimer.Restart();
            this._specificationTimer.Restart();
        }

        public void OnContextEnd(ContextInfo context)
        {
            this._contextTimer.Stop();
            this._contextTimes[context] = this._contextTimer.ElapsedMilliseconds;
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            if (!this._specificationTimer.IsRunning)
            {
                this._specificationTimer.Restart();
            }
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            this._specificationTimer.Stop();
            this._specificationTimes[specification] = this._specificationTimer.ElapsedMilliseconds;
        }

        public void OnFatalError(ExceptionResult exception)
        {
        }

        public long GetSpecificationTime(SpecificationInfo specificationInfo)
        {
            if (this._specificationTimes.ContainsKey(specificationInfo))
            {
                return this._specificationTimes[specificationInfo];
            }

            return -1;
        }

        public long GetContextTime(ContextInfo contextInfo)
        {
            if (this._contextTimes.ContainsKey(contextInfo))
            {
                return this._contextTimes[contextInfo];
            }

            return -1;
        }

        public long GetAssemblyTime(AssemblyInfo assemblyInfo)
        {
            if (this._assemblyTimes.ContainsKey(assemblyInfo))
            {
                return this._assemblyTimes[assemblyInfo];
            }

            return -1;
        }

        public long GetRunTime()
        {
            return this._runTimer.ElapsedMilliseconds;
        }
    }

    static class TimerExtensions
    {
        public static void Restart(this Stopwatch timer)
        {
            timer.Reset();
            timer.Start();
        }
    }
}