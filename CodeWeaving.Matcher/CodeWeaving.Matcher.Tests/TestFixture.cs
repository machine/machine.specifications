using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;

using NUnit.Framework;

namespace CodeWeaving.Matcher
{
  public abstract class TestFixture<TType>
  {
    protected TType _target;
    protected CilWorker _cilWorker;

    [SetUp]
    public virtual void Setup()
    {
      _target = Create();
    }

    public abstract TType Create();

    public CilWorker CilWorker
    {
      get
      {
        if (_cilWorker == null)
        {
          MethodBody body = new MethodBody(new MethodDefinition("Test", MethodAttributes.Public, null));
          _cilWorker = body.CilWorker;
        }
        return _cilWorker;
      }
    }
  }
}
