using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Explorers;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
  public class AssemblyRunner
  {
    readonly ISpecificationRunListener _listener;
    readonly RunOptions _options;

    public AssemblyRunner(ISpecificationRunListener listener, RunOptions options)
    {
      _listener = listener;
      _options = options;
    }

    public void Run(Assembly assembly, IEnumerable<Context> contexts)
    {
      bool hasExecutableSpecifications = contexts.Where(x => x.HasExecutableSpecifications).Any();

      var explorer = new AssemblyExplorer();
      var assemblyContexts = new List<IAssemblyContext>(explorer.FindAssemblyContextsIn(assembly));
      var globalCleanups = new List<ICleanupAfterEveryContextInAssembly>(explorer.FindAssemblyWideContextCleanupsIn(assembly));
      var specificationSupplements = new List<ISupplementSpecificationResults>(explorer.FindSpecificationSupplementsIn(assembly));

      _listener.OnAssemblyStart(assembly.GetInfo());

      var executedAssemblyContexts = new List<IAssemblyContext>();

      try
      {
        if (hasExecutableSpecifications)
        {
          assemblyContexts.FilteredBy(_options).ForEach(assemblyContext =>
          {
            assemblyContext.OnAssemblyStart();
            executedAssemblyContexts.Add(assemblyContext);
          });
        }

        foreach (var context in contexts)
        {
          RunContext(context, globalCleanups, specificationSupplements);
        }
      }
      catch (Exception err)
      {
        _listener.OnFatalError(new ExceptionResult(err));
      }
      finally
      {
        if (hasExecutableSpecifications)
        {
          try
          {
            executedAssemblyContexts.Reverse();
            executedAssemblyContexts.ForEach(assemblyContext => assemblyContext.OnAssemblyComplete());
          }
          catch (Exception err)
          {
            _listener.OnFatalError(new ExceptionResult(err));
            throw;
          }
        }
      }

      _listener.OnAssemblyEnd(assembly.GetInfo());
    }

    void RunContext(Context context, IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups, List<ISupplementSpecificationResults> supplements)
    {
      IContextRunner runner = ContextRunnerFactory.GetContextRunnerFor(context);
      runner.Run(context, _listener, _options, globalCleanups, supplements);
    }
  }

  public static class EnumerableExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
      foreach (var item in list)
      {
        action(item);
      }
    }
  }
}
