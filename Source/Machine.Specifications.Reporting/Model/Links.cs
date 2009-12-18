using System;

namespace Machine.Specifications.Reporting.Model
{
  public interface ILinkTarget
  {
    Guid Id
    {
      get;
    }
  }

  public interface ILinkToCanFail
  {
    ILinkTarget NextFailed
    {
      get;
      set;
    }

    ILinkTarget PreviousFailed
    {
      get;
      set;
    }
  }

  public interface ILinkToNotImplemented
  {
    ILinkTarget NextNotImplemented
    {
      get;
      set;
    }

    ILinkTarget PreviousNotImplemented
    {
      get;
      set;
    }
  }
}