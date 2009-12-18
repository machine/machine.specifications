using System;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Specs
{
  public class ReportSpecs
  {
    protected static Run Run(params Assembly[] assemblies)
    {
      var run = new Run(assemblies);

      new ISpecificationVisitor[]
      {
        new SpecificationIdGenerator(),
        new FailedSpecificationLinker(),
        new NotImplementedSpecificationLinker()
      }
        .Each(x => x.Visit(run));

      return run;
    }

    protected static Assembly Assembly(string name, params Concern[] concerns)
    {
      return new Assembly(name, concerns);
    }

    protected static Concern Concern(string name, params Context[] contexts)
    {
      return new Concern(name, contexts);
    }

    protected static Context Context(string name, params Specification[] specifications)
    {
      return new Context(name, specifications);
    }

    protected static Specification Spec(string name, Result result)
    {
      return new Specification(name, result);
    }

    protected static Exception PrepareException()
    {
      try
      {
        try
        {
          SomeAction();
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException("something bad happened", ex);
        }
      }
      catch (Exception ex)
      {
        return ex;
      }

      return null;
    }

    static void SomeAction()
    {
      throw new NotImplementedException();
    }
  }
}