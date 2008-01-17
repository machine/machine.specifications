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
    protected Dictionary<Type, object> _gottenMocks;

    [SetUp]
    public virtual void Setup()
    {
      _gottenMocks = new Dictionary<Type, object>();
      _mocks = new MockRepository();
      _target = Create();
    }

    public virtual TType Create()
    {
      throw new InvalidOperationException();
    }

    public TMock Get<TMock>()
    {
      if (!_gottenMocks.ContainsKey(typeof(TMock)))
      {
        _gottenMocks[typeof(TMock)] = _mocks.DynamicMock<TMock>();
      }
      return (TMock)_gottenMocks[typeof (TMock)];
    }
  }
}
