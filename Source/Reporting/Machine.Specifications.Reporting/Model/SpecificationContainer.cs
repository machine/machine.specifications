using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Reporting.Model
{
  public abstract class SpecificationContainer
  {
    readonly int _totalSpecifications;
    readonly int _passingSpecifications;
    readonly int _failingSpecifications;
    readonly int _notImplementedSpecifications;
    readonly int _ignoredSpecifications;

    protected SpecificationContainer(IEnumerable<Specification> specifications)
    {
      _totalSpecifications = specifications.Count();
      _passingSpecifications = specifications.Where(x => x.Status == Status.Passing).Count();
      _failingSpecifications = specifications.Where(x => x.Status == Status.Failing).Count();
      _notImplementedSpecifications = specifications.Where(x => x.Status == Status.NotImplemented).Count();
      _ignoredSpecifications = specifications.Where(x => x.Status == Status.Ignored).Count();
    }
    
    protected SpecificationContainer(IEnumerable<SpecificationContainer> specificationContainers)
    {
      _totalSpecifications = specificationContainers.Sum(x => x.TotalSpecifications);
      _passingSpecifications = specificationContainers.Sum(x => x.PassingSpecifications);
      _failingSpecifications = specificationContainers.Sum(x => x.FailingSpecifications);
      _notImplementedSpecifications = specificationContainers.Sum(x => x.NotImplementedSpecifications);
      _ignoredSpecifications = specificationContainers.Sum(x => x.IgnoredSpecifications);
    }
    
    public int TotalSpecifications
    {
      get { return _totalSpecifications; }
    }

    public int PassingSpecifications
    {
      get { return _passingSpecifications; }
    }

    public int FailingSpecifications
    {
      get { return _failingSpecifications; }
    }

    public int NotImplementedSpecifications
    {
      get { return _notImplementedSpecifications; }
    }
    
    public int IgnoredSpecifications
    {
      get { return _ignoredSpecifications; }
    }
  }
}