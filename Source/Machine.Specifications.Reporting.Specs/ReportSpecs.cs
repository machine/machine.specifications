using System;

using Machine.Specifications.Reporting.Model;

namespace Machine.Specifications.Reporting.Specs
{
  public class ReportSpecs
  {
    protected static Run Run(params Assembly[] assemblies)
    {
      return new Run(assemblies);
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

    protected static Specification Spec(string leader, string name, Result result)
    {
      return new Specification(leader, name, result) { Id = Guid.NewGuid().ToString() };
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