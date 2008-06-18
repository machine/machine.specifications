using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Model
{
  public class Context
  {
    readonly List<Specification> _specifications;
    readonly object _instance;
    readonly Concern _concern;
    IEnumerable<Establish> _beforeEachs;
    IEnumerable<Establish> _beforeAlls;
    IEnumerable<Cleanup> _afterEachs;
    IEnumerable<Cleanup> _afterAlls;
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

    public Concern Concern
    {
      get { return _concern; }
    }

    public Context(Type type, object instance, IEnumerable<Establish> beforeEachs,
      IEnumerable<Establish> beforeAlls, IEnumerable<Cleanup> afterEachs,
      IEnumerable<Cleanup> afterAlls, Concern concern)
    {
      Name = type.Name.ReplaceUnderscores();
      Type = type;
      _instance = instance;
      _afterAlls = afterAlls;
      _afterEachs = afterEachs;
      _beforeAlls = beforeAlls;
      _beforeEachs = beforeEachs;
      _specifications = new List<Specification>();
      _concern = concern;
    }

    public void AddSpecification(Specification specification)
    {
      _specifications.Add(specification);
    }

    public ContextVerificationResult Verify()
    {
      var verificationResults = VerifySpecifications();
      return new ContextVerificationResult(verificationResults);
    }

    IEnumerable<SpecificationVerificationResult> VerifySpecifications()
    {
      _beforeAlls.InvokeAll();
      var results = ExecuteSpecifications();
      _afterAlls.InvokeAll();

      return results;
    }

    IEnumerable<SpecificationVerificationResult> ExecuteSpecifications()
    {
      var results = new List<SpecificationVerificationResult>();
      foreach (Specification specification in _specifications)
      {
        var result = VerifySpecification(specification);
        results.Add(result);
      }

      return results;
    }

    public SpecificationVerificationResult VerifySpecification(Specification specification)
    {
      VerificationContext context = new VerificationContext(_instance);
      try
      {
        _beforeEachs.InvokeAll();
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
        return new SpecificationVerificationResult(err);
      }

      return result;
    }

    public void RunContextBeforeAll()
    {
      _beforeAlls.InvokeAll();
    }

    public void RunContextAfterAll()
    {
      _afterAlls.InvokeAll();
    }
  }
}