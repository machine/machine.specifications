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
      IEnumerable<Result> results;
      listener.OnContextStart(context.GetInfo());
      Result result = Result.Pass();

      if (context.HasExecutableSpecifications)
      {
        result = context.EstablishContext();
      }

      if (result.Passed)
      {
        results = RunSpecifications(context, listener, options);
      }
      else
      {
        results = FailSpecifications(context, listener, options, result);
      }

      if (context.HasExecutableSpecifications)
      {
        result = context.Cleanup();
      }

      listener.OnContextEnd(context.GetInfo());

      return results;
    }

    private IEnumerable<Result> RunSpecifications(Context context, ISpecificationRunListener listener, RunOptions options)
    {
      var results = new List<Result>();
      foreach (var specification in context.Specifications)
      {
        var runner = new SpecificationRunner(listener, options);
        var result = runner.Run(specification);

        results.Add(result);
      }

      return results;
    }

    private IEnumerable<Result> FailSpecifications(Context context, ISpecificationRunListener listener, RunOptions options, Result result)
    {
      var results = new List<Result>();
      foreach (var specification in context.Specifications)
      {
        listener.OnSpecificationStart(specification.GetInfo());
        listener.OnSpecificationEnd(specification.GetInfo(), result);
        results.Add(result);
      }

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
        Result result = Result.Pass();

        if (specification.IsExecutable)
        {
          result = context.EstablishContext();
        }

        if (result.Passed)
        {
          var runner = new SpecificationRunner(listener, options);
          result = runner.Run(specification);
        }

        if (specification.IsExecutable)
        {
          var cleanupResult = context.Cleanup();

          if (result.Passed && !cleanupResult.Passed)
          {
            result = cleanupResult;
          }
        }

        results.Add(result);
      }

      listener.OnContextEnd(context.GetInfo());

      return results;
    }
  }
}