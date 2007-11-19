using System;
using System.Collections.Generic;

namespace ObjectFactories.Services
{
  public interface IObjectFactory<TType>
  {
    TType Create();
  }
}