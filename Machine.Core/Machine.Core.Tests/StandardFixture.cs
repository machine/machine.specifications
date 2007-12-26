using System;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Core
{
  public class StandardFixture<TType>
  {
    protected TType _target;
    protected MockRepository _mocks;

    [SetUp]
    public virtual void Setup()
    {
      _mocks = new MockRepository();
      _target = Create();
    }

    public virtual TType Create()
    {
      throw new InvalidOperationException();
    }
  }
}
