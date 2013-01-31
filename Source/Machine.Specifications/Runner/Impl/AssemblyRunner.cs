using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
      var hasExecutableSpecifications = contexts.Where(x => x.HasExecutableSpecifications).Any();

      var explorer = new AssemblyExplorer();
      var globalCleanups = new List<ICleanupAfterEveryContextInAssembly>(explorer.FindAssemblyWideContextCleanupsIn(assembly));
      var specificationSupplements = new List<ISupplementSpecificationResults>(explorer.FindSpecificationSupplementsIn(assembly));

      try
      {
        if (hasExecutableSpecifications)
        {
          _listener.OnAssemblyStart(assembly.GetInfo());
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
        try
        {
          if (hasExecutableSpecifications)
          {
            _listener.OnAssemblyEnd(assembly.GetInfo());
          }
        }
        catch (Exception err)
        {
          _listener.OnFatalError(new ExceptionResult(err));
          throw;
        }
      }
    }

    void RunContext(Context context,
                    IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups,
                    IEnumerable<ISupplementSpecificationResults> supplements)
    {
      var runner = ContextRunnerFactory.GetContextRunnerFor(context);
      runner.Run(context, _listener, _options, globalCleanups, supplements);
    }
  }
}
