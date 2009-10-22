using System.Collections.Generic;
using System.Diagnostics;

namespace Machine.Specifications.Runner.Impl
{
  public class TimingRunListener : ISpecificationRunListener
  {
    readonly Stopwatch _contextTimer = new Stopwatch();
    readonly Stopwatch _specificationTimer = new Stopwatch();
    readonly Stopwatch _assemblyTimer = new Stopwatch();
    readonly Stopwatch _runTimer = new Stopwatch();
    readonly Dictionary<SpecificationInfo, long> _specificationTimes = new Dictionary<SpecificationInfo, long>();
    readonly Dictionary<ContextInfo, long> _contextTimes = new Dictionary<ContextInfo, long>();
    readonly Dictionary<AssemblyInfo, long> _assemblyTimes = new Dictionary<AssemblyInfo, long>();

    public long GetSpecificationTime(SpecificationInfo specificationInfo)
    {
      if (_specificationTimes.ContainsKey(specificationInfo))
      {
        return _specificationTimes[specificationInfo];
      }

      return -1;
    }

    public long GetContextTime(ContextInfo contextInfo)
    {
      if (_contextTimes.ContainsKey(contextInfo))
      {
        return _contextTimes[contextInfo];
      }

      return -1;
    }

    public long GetAssemblyTime(AssemblyInfo assemblyInfo)
    {
      if (_assemblyTimes.ContainsKey(assemblyInfo))
      {
        return _assemblyTimes[assemblyInfo];
      }

      return -1;
    }

    public long GetRunTime()
    {
      return _runTimer.ElapsedMilliseconds;
    }

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      _assemblyTimer.Restart();
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      _assemblyTimer.Stop();
      _assemblyTimes[assembly] = _assemblyTimer.ElapsedMilliseconds;
    }

    public void OnRunStart()
    {
      _runTimer.Restart();
    }

    public void OnRunEnd()
    {
      _runTimer.Stop();
    }

    public void OnContextStart(ContextInfo context)
    {
      _contextTimer.Restart();
      _specificationTimer.Restart();
    }

    public void OnContextEnd(ContextInfo context)
    {
      _contextTimer.Stop();
      _contextTimes[context] = _contextTimer.ElapsedMilliseconds;
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
      if (!_specificationTimer.IsRunning)
      {
        _specificationTimer.Restart();
      }
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      _specificationTimer.Stop();
      _specificationTimes[specification] = _specificationTimer.ElapsedMilliseconds;
    }

    public void OnFatalError(ExceptionResult exception)
    {
    }
  }

  public static class TimerExtensions
  {
    public static void Restart(this Stopwatch timer)
    {
      timer.Reset();
      timer.Start();
    }
  }
}