using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.Reporting.Generation
{
  public static class SpecificationTreeMapping
  {
    public static Assembly ToNode(this AssemblyInfo assemblyInfo, IEnumerable<Concern> concerns)
    {
      return new Assembly(assemblyInfo.Name, concerns);
    }
    
    public static Context ToNode(this ContextInfo contextInfo, IEnumerable<Specification> specifications)
    {
      return new Context(contextInfo.Name, specifications);
    }

    public static Specification ToNode(this SpecificationInfo specification, Result result)
    {
      return new Specification(specification.Name, result);
    }
  }

  public class SpecificationTreeListener : ISpecificationRunListener
  {
    Run _run;
    int _nextId;
    readonly List<Assembly> _assemblies = new List<Assembly>();
    Dictionary<string, List<Context>> _concernsToContexts = new Dictionary<string, List<Context>>();
    List<Specification> _specifications = new List<Specification>();

    public Run Run
    {
      get { return _run; }
    }

    public void OnRunStart()
    {
      _nextId = 1;
    }

    public virtual void OnRunEnd()
    {
      _run = new Run(_assemblies);
    }

    public void OnAssemblyStart(AssemblyInfo assembly)
    {
      _concernsToContexts = new Dictionary<string, List<Context>>();
    }

    public void OnAssemblyEnd(AssemblyInfo assembly)
    {
      var concerns = CreateConcerns();

      _assemblies.Add(assembly.ToNode(concerns));
    }

    IEnumerable<Concern> CreateConcerns()
    {
      return _concernsToContexts.Select(x => new Concern(x.Key, x.Value));
    }

    public void OnContextStart(ContextInfo context)
    {
      _specifications = new List<Specification>();
    }

    public void OnContextEnd(ContextInfo context)
    {
      if (!_concernsToContexts.ContainsKey(context.Concern))
      {
        _concernsToContexts[context.Concern] = new List<Context>();
      }

      _concernsToContexts[context.Concern].Add(context.ToNode(_specifications));
    }

    public void OnSpecificationStart(SpecificationInfo specification)
    {
    }

    public void OnSpecificationEnd(SpecificationInfo specification, Result result)
    {
      _specifications.Add(AssignId(specification.ToNode(result)));
    }

    public void OnFatalError(ExceptionResult exception)
    {
    }

    Specification AssignId(Specification node)
    {
      node.Id = _nextId.ToString(CultureInfo.InvariantCulture);
      _nextId++;
      return node;
    }
  }
}


