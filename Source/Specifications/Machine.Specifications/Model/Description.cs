using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Model
{
  public class Description
  {
    private List<Specification> _specifications;
    private object _instance;
    private IEnumerable<Context> _beforeEachs;
    private IEnumerable<Context> _beforeAlls;
    private IEnumerable<Context> _afterEachs;
    private IEnumerable<Context> _afterAlls;
    private When _when;
    public string Name { get; private set; }
    public string WhenClause { get; set; }
    public object Instance
    {
      get { return _instance; }
    }

    public IEnumerable<Specification> Specifications
    {
      get { return _specifications; }
    }

    public Type Type
    {
      get; private set;
    }

    public Description(Type type, object instance, IEnumerable<Context> beforeEachs, IEnumerable<Context> beforeAlls, IEnumerable<Context> afterEachs, IEnumerable<Context> afterAlls, When when)
    {
      Name = type.Name.ReplaceUnderscores();
      Type = type;
      _instance = instance;
      _when = when;
      _afterAlls = afterAlls;
      _afterEachs = afterEachs;
      _beforeAlls = beforeAlls;
      _beforeEachs = beforeEachs;
      _specifications = new List<Specification>();
    }

    public void AddSpecification(Specification specification)
    {
      _specifications.Add(specification);
    }

    public DescriptionVerificationResult Verify()
    {
      var verificationResults = VerifySpecifications();
      return new DescriptionVerificationResult(verificationResults);
    }

    private IEnumerable<SpecificationVerificationResult> VerifySpecifications()
    {
      _beforeAlls.InvokeAll();
      var results = ExecuteSpecifications();
      _afterAlls.InvokeAll();

      return results;
    }

    private IEnumerable<SpecificationVerificationResult> ExecuteSpecifications()
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
      VerificationContext context = new VerificationContext();
      _beforeEachs.InvokeAll();
      if (_when != null)
      {
        try
        {
          _when();
        }
        catch (Exception exception)
        {
          context.ThrownException = exception;
        }
      }
      var result = specification.Verify(context);
      _afterEachs.InvokeAll();

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
