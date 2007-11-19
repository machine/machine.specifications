using System;
using System.Collections.Generic;

namespace ObjectFactories.Services
{
  public interface ISuperFactory
  {
    IObjectFactory<TType> CreateFactory<TType>();
  }
}