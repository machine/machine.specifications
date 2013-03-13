namespace Machine.Specifications.Reporting.Model
{
  public interface ILinkTarget
  {
    string Id
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
  
  public interface ILinkToIgnored
  {
    ILinkTarget NextIgnored
    {
      get;
      set;
    }

    ILinkTarget PreviousIgnored
    {
      get;
      set;
    }
  }
}