using System.Collections.Generic;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
  public interface IContextRunner
  {
    IEnumerable<Result> Run(Context context, ISpecificationRunListener listener, RunOptions options);
  }

  public class SetupOnceContextRunner : IContextRunner
  {
    public IEnumerable<Result> Run(Context context, ISpecificationRunListener listener, RunOptions options)
    {
      var results = new List<Result>();
      listener.OnContextStart(context.GetInfo());

      if (context.HasExecutableSpecifications)
      {
        context.EstablishContext();
      }

      foreach (var specification in context.Specifications)
      {
        var runner = new SpecificationRunner(listener, options);
        var result = runner.Run(specification);

        results.Add(result);
      }

      if (context.HasExecutableSpecifications)
      {
        context.Cleanup();
      }

      listener.OnContextEnd(context.GetInfo());

      return results;
    }
  }

  public class SetupForEachContextRunner : IContextRunner
  {
    public IEnumerable<Result> Run(Context context, ISpecificationRunListener listener, RunOptions options)
    {
      var results = new List<Result>();
      listener.OnContextStart(context.GetInfo());

      foreach (var specification in context.Specifications)
      {
        if (specification.IsExecutable)
        {
          context.EstablishContext();
        }

        var runner = new SpecificationRunner(listener, options);
        var result = runner.Run(specification);

        results.Add(result);

        if (specification.IsExecutable)
        {
          context.Cleanup();
        }
      }

      listener.OnContextEnd(context.GetInfo());

      return results;
    }
  }
}