using System;
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
    public string Name { get; private set; }

    public object Instance
    {
      get { return _instance; }
    }

    public IEnumerable<Specification> Specifications
    {
      get { return _specifications; }
    }

    public Type Type { get; private set; }

    public Subject Subject
    {
      get { return _subject; }
    }

    public bool HasBecauseClause
    {
      get { return _because != null; }
    }

    public Context(Type type, object instance, IEnumerable<Establish> beforeEachs,
      IEnumerable<Establish> beforeAlls, Because because, IEnumerable<Cleanup> afterEachs,
      IEnumerable<Cleanup> afterAlls, Subject subject)
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
      
    }

    public void AddSpecification(Specification specification)
    {
      _specifications.Add(specification);
    }

    public IEnumerable<SpecificationVerificationResult> VerifyAllSpecifications()
    {
      return EnumerateAndVerifyAllSpecifications().Where(x => x.Result != null).Select(x => x.Result).ToList();
    }

    public IEnumerable<SpecificationVerificationIteration> EnumerateAndVerifyAllSpecifications()
    {
      if (!Specifications.Any()) yield break;

      bool hasRunnableSpecifications = Specifications.Where(x => !x.IsIgnored).Any();

      if (hasRunnableSpecifications)
      {
        try
        {
          _beforeAlls.InvokeAll();
        }
        catch (Exception err)
        {
          
          throw;
        }
      }

      SpecificationVerificationResult result = null;
      Specification current = null;
      foreach (var next in Specifications)
      {
        yield return new SpecificationVerificationIteration(current, result, next);
        current = next;
        result = VerifyOrIgnoreSpecification(current);
      }

      yield return new SpecificationVerificationIteration(current, result, null);

      if (hasRunnableSpecifications)
      {
        _afterAlls.InvokeAll();
      }
    }

    private SpecificationVerificationResult VerifyOrIgnoreSpecification(Specification specification)
    {
      if (specification.IsIgnored)
      {
        return SpecificationVerificationResult.Ignored;
      }
      else
      {
        return VerifySpecification(specification);
      }
    }
    
    public SpecificationVerificationResult VerifySpecification(Specification specification)
    {
      VerificationContext context = new VerificationContext(_instance);
      try
      {
        _beforeEachs.InvokeAll();
        _because.InvokeIfNotNull();
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
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
          return new SpecificationVerificationResult(err);
        }
        return result;
      }

      return result;
    }

    public SpecificationVerificationResult RunContextBeforeAll()
    {
      try
      {
        _beforeAlls.InvokeAll();
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
      }
      return null;
    }

    public SpecificationVerificationResult RunContextAfterAll()
    {
      try
      {
        _afterAlls.InvokeAll();
      }
      catch (Exception err)
      {
        return new SpecificationVerificationResult(err);
      }
      return null;
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
  }

  public class SpecificationVerificationIteration
  {
    public Specification Next { get; private set; }
    public Specification Current { get; private set; }
    public SpecificationVerificationResult Result { get; private set; }

    public SpecificationVerificationIteration(Specification current, SpecificationVerificationResult result, Specification next)
    {
      Next = next;
      Current = current;
      Result = result;
    }
  }
}