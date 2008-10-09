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

      _listener.OnAssemblyStart(assembly.GetInfo());
        
      if (hasExecutableSpecifications)
      {
        assemblyContexts.ForEach(assemblyContext => assemblyContext.OnAssemblyStart());
      }

      foreach (var context in contexts)
      {
        RunContext(context);
      }

      if (hasExecutableSpecifications)
      {
        assemblyContexts.ForEach(assemblyContext => assemblyContext.OnAssemblyComplete());
      }

      _listener.OnAssemblyEnd(assembly.GetInfo());
      
    }

    void RunContext(Context context)
    {
      IContextRunner runner = ContextRunnerFactory.GetContextRunnerFor(context);
      runner.Run(context, _listener, _options);
    }
  }
}
