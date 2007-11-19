using System;
using System.Collections.Generic;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

using ObjectFactories.Model;

namespace ObjectFactories.Services.Impl
{
  public class FactoryCallWeaver : IFactoryCallWeaver
  {
    #region Member Data
    private readonly ILog _log;
    #endregion

    #region FactoryCallWeaver()
    public FactoryCallWeaver(ILog log)
    {
      _log = log;
    }
    #endregion

    #region IFactoryCallWeaver Members
    public void WeaveConstructorCalls(IEnumerable<ConstructorCallWeave> weaves, FactoryMap factories)
    {
      MethodInfo getFactoryMethod = typeof(Factories).GetMethod("Create");
      foreach (ConstructorCallWeave weave in weaves)
      {
        Factory factory = factories.GetForObjectType(weave.Constructor.DeclaringType);
        _log.Log("Weaving {0} in {1}", factory.ObjectType, weave.ParentMethod);

        MethodReference getObjectMethodReference = weave.ParentAssembly.Import(getFactoryMethod);
        GenericInstanceMethod methodCall = new GenericInstanceMethod(getObjectMethodReference);
        methodCall.GenericArguments.Add(factory.ObjectType);

        CilWorker worker = weave.ParentMethod.Body.CilWorker;
        Instruction callGetFactory = worker.Create(OpCodes.Call, methodCall);
        worker.Replace(weave.NewInstruction, callGetFactory);
      }
    }
    #endregion
  }
}
