using System;
using System.Collections.Generic;
using Machine.Specifications.Model;
using System.Linq;

namespace Machine.Specifications.Runner.Impl
{
  public interface IContextRunner
  {
    IEnumerable<Result> Run(Context context, ISpecificationRunListener listener, RunOptions options, IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups, IEnumerable<ISupplementSpecificationResults> resultSupplementers);
  }

  public class SetupOnceContextRunner : IContextRunner
  {
    public IEnumerable<Result> Run(Context context, ISpecificationRunListener listener, RunOptions options, IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups, IEnumerable<ISupplementSpecificationResults> resultSupplementers)
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
        results = RunSpecifications(context, listener, options, resultSupplementers);
      }
      else
      {
        results = FailSpecifications(context, listener, options, result, resultSupplementers);
      }

      if (context.HasExecutableSpecifications)
      {
        result = context.Cleanup();
        foreach (var cleanup in globalCleanups)
        {
          cleanup.AfterContextCleanup();
        }
      }

      listener.OnContextEnd(context.GetInfo());

      return results;
    }

    private static IEnumerable<Result> RunSpecifications(Context context, ISpecificationRunListener listener, RunOptions options, IEnumerable<ISupplementSpecificationResults> resultSupplementers)
    {
      var results = new List<Result>();
      foreach (var specification in context.Specifications)
      {
        var runner = new SpecificationRunner(listener, options, resultSupplementers);
        var result = runner.Run(specification);

        results.Add(result);
      }

      return results;
    }

    private static IEnumerable<Result> FailSpecifications(Context context, ISpecificationRunListener listener, RunOptions options, Result result, IEnumerable<ISupplementSpecificationResults> resultSupplementers)
    {
      result = resultSupplementers.Aggregate(result, (r, supplement) => supplement.SupplementResult(r));

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
    public IEnumerable<Result> Run(Context context, ISpecificationRunListener listener, RunOptions options, IEnumerable<ICleanupAfterEveryContextInAssembly> globalCleanups, IEnumerable<ISupplementSpecificationResults> resultSupplementers)
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
          var runner = new SpecificationRunner(listener, options, resultSupplementers);
          result = runner.Run(specification);
        }
        else
        {
            results = FailSpecification(listener, specification, result);
        }

        if (specification.IsExecutable)
        {
          var cleanupResult = context.Cleanup();

          if (result.Passed && !cleanupResult.Passed)
          {
            result = cleanupResult;
          }

          foreach (var cleanup in globalCleanups)
          {
            cleanup.AfterContextCleanup();
          }
        }

        results.Add(result);
      }

      listener.OnContextEnd(context.GetInfo());

      return results;
    }

    static List<Result> FailSpecification(ISpecificationRunListener listener, Specification specification, Result result)
    {
      listener.OnSpecificationStart(specification.GetInfo());
      listener.OnSpecificationEnd(specification.GetInfo(), result);
      return new List<Result> { result };
    }
  }
}