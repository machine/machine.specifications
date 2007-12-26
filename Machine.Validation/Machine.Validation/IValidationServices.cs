using System;
using System.Collections.Generic;

namespace Machine.Validation
{
  public interface IValidationServices
  {
    T Wrap<T>(T validatable) where T : IValidatable;
  }
}
