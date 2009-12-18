using System;

namespace Machine.Specifications.Reporting.Model
{
  public interface ILinkToCanFail
  {
    ICanFail Next
    {
      get;
      set;
    }

    ICanFail Previous
    {
      get;
      set;
    }
  }

  public interface ICanFail : ILinkToCanFail
  {
    Guid Id
    {
      get;
    }
  }

  public interface ILinkToNotImplemented
  {
    ICanBeNotImplemented Next
    {
      get;
      set;
    }

    ICanBeNotImplemented Previous
    {
      get;
      set;
    }
  }

  public interface ICanBeNotImplemented : ILinkToNotImplemented
  {
    Guid Id
    {
      get;
    }
  }
}