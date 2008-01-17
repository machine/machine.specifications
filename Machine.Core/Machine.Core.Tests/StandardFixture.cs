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
    protected Dictionary<Type, object> _dynamicMocks;
    protected Dictionary<Type, object> _normalMocks;

    [SetUp]
    public virtual void Setup()
    {
      _dynamicMocks = new Dictionary<Type, object>();
      _normalMocks = new Dictionary<Type, object>();
      _mocks = new MockRepository();
      _target = Create();
    }

    public virtual TType Create()
    {
      throw new InvalidOperationException();
    }

    public TMock GetNormal<TMock>()
    {
      if (_dynamicMocks.ContainsKey(typeof(TMock)))
      {
        throw new InvalidOperationException();
      }
      if (!_normalMocks.ContainsKey(typeof(TMock)))
      {
        _normalMocks[typeof(TMock)] = _mocks.CreateMock<TMock>();
      }
      return (TMock)_normalMocks[typeof(TMock)];
    }

    public TMock Get<TMock>()
    {
      if (_normalMocks.ContainsKey(typeof(TMock)))
      {
        return (TMock)_normalMocks[typeof(TMock)];
      }
      if (!_dynamicMocks.ContainsKey(typeof(TMock)))
      {
        _dynamicMocks[typeof(TMock)] = _mocks.DynamicMock<TMock>();
      }
      return (TMock)_dynamicMocks[typeof(TMock)];
    }
  }
}
