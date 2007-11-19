using System;
using System.Collections.Generic;

using Castle.Core;
using Castle.MicroKernel;

namespace Castle.Facilities.DeferredServiceResolution
{
  public interface IComponentModelAndKernelSource
  {
    List<ComponentModel> Models
    {
      get;
    }
    IKernel Kernel
    {
      get;
    }
  }
}
