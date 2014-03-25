namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public class TimingRunListener : IRemoteSpecificationRunListener
    {
        readonly Stopwatch _assemblyTimer = new Stopwatch();
        readonly Dictionary<RemoteAssemblyInfo, long> _assemblyTimes = new Dictionary<RemoteAssemblyInfo, long>();
        readonly Stopwatch _contextTimer = new Stopwatch();
        readonly Dictionary<RemoteContextInfo, long> _contextTimes = new Dictionary<RemoteContextInfo, long>();
        readonly Stopwatch _runTimer = new Stopwatch();
        readonly Stopwatch _specificationTimer = new Stopwatch();
        readonly Dictionary<RemoteSpecificationInfo, long> _specificationTimes = new Dictionary<RemoteSpecificationInfo, long>();

        public void OnRunStart()
        {
            this._runTimer.Restart();
        }

        public void OnRunEnd()
        {
            this._runTimer.Stop();
        }

        public void OnAssemblyStart(RemoteAssemblyInfo assembly)
        {
            this._assemblyTimer.Restart();
        }

        public void OnAssemblyEnd(RemoteAssemblyInfo assembly)
        {
            this._assemblyTimer.Stop();
            this._assemblyTimes[assembly] = this._assemblyTimer.ElapsedMilliseconds;
        }

        public void OnContextStart(RemoteContextInfo context)
        {
            this._contextTimer.Restart();
            this._specificationTimer.Restart();
        }

        public void OnContextEnd(RemoteContextInfo context)
        {
            this._contextTimer.Stop();
            this._contextTimes[context] = this._contextTimer.ElapsedMilliseconds;
        }

        public void OnSpecificationStart(RemoteSpecificationInfo specification)
        {
            if (!this._specificationTimer.IsRunning)
            {
                this._specificationTimer.Restart();
            }
        }

        public void OnSpecificationEnd(RemoteSpecificationInfo specification, RemoteResult result)
        {
            this._specificationTimer.Stop();
            this._specificationTimes[specification] = this._specificationTimer.ElapsedMilliseconds;
        }

        public void OnFatalError(RemoteExceptionResult exception)
        {
        }

        public long GetSpecificationTime(RemoteSpecificationInfo specificationInfo)
        {
            if (this._specificationTimes.ContainsKey(specificationInfo))
            {
                return this._specificationTimes[specificationInfo];
            }

            return -1;
        }

        public long GetContextTime(RemoteContextInfo contextInfo)
        {
            if (this._contextTimes.ContainsKey(contextInfo))
            {
                return this._contextTimes[contextInfo];
            }

            return -1;
        }

        public long GetAssemblyTime(RemoteAssemblyInfo assemblyInfo)
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