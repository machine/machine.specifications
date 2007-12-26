using System;
using System.Collections.Generic;

namespace Machine.Validation
{
  public interface IValidatable
  {
    bool IsValid
    {
      get;
      set;
    }

    IValidationCallback ValidationCallback
    {
      get;
      set;
    }

    void CommitChanges();
  }
  public interface IValidationCallback
  {
    void RollbackChanges();
    void CommitChanges();
  }
}
