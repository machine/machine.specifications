using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Model
{
  public class Context
  {
    readonly List<Specification> _specifications;
    readonly object _instance;
    readonly Subject _subject;
    readonly IEnumerable<Establish> _beforeEachs;
    readonly IEnumerable<Establish> _beforeAlls;
    readonly Because _because;
    readonly IEnumerable<Cleanup> _afterEachs;
    readonly IEnumerable<Cleanup> _afterAlls;
    readonly IEnumerable<Tag> _tags;
    bool _isGlobalContextEstablished;

    public string Name { get; private set; }
    public bool IsIgnored { get; private set; }
    public IEnumerable<Tag> Tags { get { return _tags; } }
    public object Instance { get { return _instance; } }
    public IEnumerable<Specification> Specifications { get { return _specifications; } }
    public Type Type { get; private set; }
    public Subject Subject { get { return _subject; } }
    public bool HasBecauseClause { get { return _because != null; } }
    public Result CriticalContextFailure { get; private set; }

    public Context(Type type, object instance, IEnumerable<Establish> beforeEachs,
      IEnumerable<Establish> beforeAlls, Because because, IEnumerable<Cleanup> afterEachs,
      IEnumerable<Cleanup> afterAlls, Subject subject, bool isIgnored, IEnumerable<Tag> tags)
    {
      Name = type.Name.ReplaceUnderscores();
      Type = type;
      _instance = instance;
      _afterAlls = afterAlls;
      _afterEachs = afterEachs;
      _beforeAlls = beforeAlls;
      _because = because;
      _beforeEachs = beforeEachs;
      _specifications = new List<Specification>();
      _subject = subject;
      IsIgnored = isIgnored;
      _tags = tags;
    }

    public void AddSpecification(Specification specification)
    {
      _specifications.Add(specification);
    }

    public IEnumerable<Result> VerifyAllSpecifications()
    {
      var results = new List<Result>();
      foreach (var specification in EnumerateSpecificationsForVerification())
      {
        var result = VerifyOrIgnoreSpecification(specification);
        results.Add(result);
      }

      return results;
    }

    public Result VerifyOrIgnoreSpecification(Specification specification)
    {
      if (specification.IsIgnored)
      {
        return Result.Ignored();
      }
      else if (!specification.IsDefined)
      {
        return Result.NotImplemented();
      }
      else if (CriticalContextFailure != null)
      {
        return CriticalContextFailure;
      }
      else
      {
        return InternalVerifySpecification(specification);
      }
    }

    public Result VerifySpecification(Specification specification)
    {
      if (specification.IsIgnored)
      {
        return Result.Ignored();
      }

      if (!specification.IsDefined)
      {
        return Result.NotImplemented();
      }

      if (!_isGlobalContextEstablished)
      {
        RunContextBeforeAll();
      }

      if (CriticalContextFailure != null)
      {
        return CriticalContextFailure;
      }

      var result = InternalVerifySpecification(specification);

      if (!_isGlobalContextEstablished)
      {
        RunContextAfterAll();
      }

      return result;
    }
    
    private Result InternalVerifySpecification(Specification specification)
    {
      VerificationContext context = new VerificationContext(_instance);
      try
      {
        _beforeEachs.InvokeAll();
        _because.InvokeIfNotNull();
      }
      catch (Exception err)
      {
        return Result.ContextFailure(err);
      }

      var result = specification.Verify(context);

      try
      {
        _afterEachs.InvokeAll();
      }
      catch (Exception err)
      {
        if (result.Passed)
        {
          return Result.ContextFailure(err);
        }
        return result;
      }

      return result;
    }

    public void RunContextBeforeAll()
    {
      try
      {
        _beforeAlls.InvokeAll();
      }
      catch (Exception err)
      {
        CriticalContextFailure = Result.ContextFailure(err);
      }
    }

    public void RunContextAfterAll()
    {
      try
      {
        _afterAlls.InvokeAll();
      }
      catch (Exception err)
      {
        CriticalContextFailure = Result.ContextFailure(err);
      }
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

    public IEnumerable<Specification> EnumerateSpecificationsForVerification()
    {
      if (!Specifications.Any()) yield break;

      bool hasRunnableSpecifications = Specifications.Where(x => !x.IsIgnored && x.IsDefined).Any();

      if (hasRunnableSpecifications)
      {
        RunContextBeforeAll();
      }
      _isGlobalContextEstablished = true;

      foreach (var specification in Specifications)
      {
        yield return specification;
      }

      if (hasRunnableSpecifications)
      {
        RunContextAfterAll();
      }

      _isGlobalContextEstablished = false;
      CriticalContextFailure = null;
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