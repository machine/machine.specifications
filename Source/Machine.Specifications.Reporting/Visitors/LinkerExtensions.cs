using Machine.Specifications.Reporting.Model;

namespace Machine.Specifications.Reporting.Visitors
{
  static class LinkerExtensions
  {
    public static Specification LinkFailureTo(this Specification specification, Specification linkTo)
    {
      if (linkTo != null)
      {
        specification.PreviousFailed = linkTo;
        linkTo.NextFailed = specification;
      }

      return specification;
    }

    public static Specification LinkNotImplementedTo(this Specification specification, Specification linkTo)
    {
      if (linkTo != null)
      {
        specification.PreviousNotImplemented = linkTo;
        linkTo.NextNotImplemented = specification;
      }

      return specification;
    } 
    
    public static Specification LinkIgnoredTo(this Specification specification, Specification linkTo)
    {
      if (linkTo != null)
      {
        specification.PreviousIgnored = linkTo;
        linkTo.NextIgnored = specification;
      }

      return specification;
    }
  }
}