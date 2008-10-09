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
      var filteredContexts = contexts.FilteredBy(_options);

      if (!filteredContexts.Any()) return;

      var explorer = new AssemblyExplorer();
      var assemblyContexts = new List<IAssemblyContext>(explorer.FindAssemblyContextsIn(assembly));

      _listener.OnAssemblyStart(assembly.GetInfo());
        
      assemblyContexts.ForEach(assemblyContext => assemblyContext.OnAssemblyStart());

      RunContexts(contexts.FilteredBy(_options));
        
      assemblyContexts.ForEach(assemblyContext => assemblyContext.OnAssemblyComplete());
        
      _listener.OnAssemblyEnd(assembly.GetInfo());
      
    }

    private void RunContexts(IEnumerable<Context> contexts)
    {
      foreach (var context in contexts)
      {
        if (!context.HasRunnableSpecifications) continue;

        RunContext(context);
      }
    }

    void RunContext(Context context)
    {
      var runner = new ContextRunner(_listener, _options);
      runner.Run(context);
    }
  }
}
