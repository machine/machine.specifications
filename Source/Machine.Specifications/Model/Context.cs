using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Model
{
  public class Context
  {
    readonly List<Specification> _specifications;
    readonly object _instance;
    readonly Subject _subject;
    readonly IEnumerable<Establish> _contextClauses;
    readonly Because _becauseClause;
    readonly IEnumerable<Cleanup> _cleanupClauses;
    readonly IEnumerable<Tag> _tags;

    public string Name { get; private set; }
    public bool IsIgnored { get; private set; }
    public bool IsSetupForEachSpec { get; set; }
    public IEnumerable<Tag> Tags { get { return _tags; } }
    public object Instance { get { return _instance; } }
    public IEnumerable<Specification> Specifications { get { return _specifications; } }
    public Type Type { get; private set; }
    public Subject Subject { get { return _subject; } }
    public bool HasBecauseClause { get { return _becauseClause != null; } }

    public Context(Type type, object instance, IEnumerable<Establish> contextClauses, Because becauseClause, IEnumerable<Cleanup> cleanupClauses, Subject subject, bool isIgnored, IEnumerable<Tag> tags, bool isSetupForEachSpec)
    {
      Name = type.Name.ToFormat();
      Type = type;
      _instance = instance;
      _cleanupClauses = cleanupClauses;
      _contextClauses = contextClauses;
      _becauseClause = becauseClause;
      _specifications = new List<Specification>();
      _subject = subject;
      IsIgnored = isIgnored;
      _tags = tags;
      IsSetupForEachSpec = isSetupForEachSpec;
    }

    public void AddSpecification(Specification specification)
    {
      _specifications.Add(specification);
    }

    public Result EstablishContext()
    {
      Result result = Result.Pass();

      try
      {
        _contextClauses.InvokeAll();
        _becauseClause.InvokeIfNotNull();
      }
      catch (Exception err)
      {
        result = Result.ContextFailure(err);
      }

      return result;
    }

    public Result Cleanup()
    {
      Result result = Result.Pass();

      try
      {
        _cleanupClauses.InvokeAll();
      }
      catch (Exception err)
      {
        result = Result.ContextFailure(err);
      }

      return result;
    }

    // TODO: Rename to Name
    public string FullName
    {
      get
      {
        string line = "";

        if (Subject != null)
        {
          line += Subject.FullConcern + ", ";
        }

        return line + Name;
      }
    }

    public bool HasExecutableSpecifications
    {
      get { return Specifications.Where(x => x.IsExecutable).Any(); }
    }
  }

  public class SpecificationVerificationIteration
  {
    public Specification Next { get; private set; }
    public Specification Current { get; private set; }
    public Result Result { get; private set; }

    public SpecificationVerificationIteration(Specification current, Result result, Specification next)
    {
      Next = next;
      Current = current;
      Result = result;
    }
  }
}