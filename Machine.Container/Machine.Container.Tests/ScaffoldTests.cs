using System;
using System.Collections.Generic;
using System.Reflection;

namespace Machine.Container
{
  public class ScaffoldTests<TObject> : MachineContainerTestsFixture
  {
    #region Member Data
    protected delegate void Block();
    protected TObject _target;
    #endregion

    #region Methods
    public override void Setup()
    {
      base.Setup();
      _target = Create();
    }

    protected void Run(Block block)
    {
      using (_mocks.Record())
      {
        if (block != null) block();
      }
    }

    protected void Run()
    {
      Run(null);
    }

    protected virtual TObject Create()
    {
      return Create<TObject>();
    }
    #endregion
  }
}
