using System;
using System.Collections.Generic;
using System.Reflection;

using Mono.Cecil;
using Mono.Cecil.Cil;

using MethodAttributes = Mono.Cecil.MethodAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

namespace ObjectFactories.Services.Impl
{
  public class FactoryMapSerializer : IFactoryMapSerializer
  {
    public const string FactoryMapTypeName = "FactoryMap";
    public const string FactoryMapTypeNamespace = "ObjectFactories.Runtime";
    public const string FactoryMapTypeFullName = FactoryMapTypeNamespace + "." + FactoryMapTypeName;
    public const string FactoryMapMethodName = "GetTypes";

    public void StoreFactoryMap(IAssembly assembly, FactoryMap factoryMap)
    {
      MethodInfo getTypeFromHandleReflect = typeof (Type).GetMethod("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) });
      MethodReference getTypeFromHandle = assembly.Import(getTypeFromHandleReflect);
      TypeReference systemType = assembly.Import(typeof(Type));
      TypeReference arrayOfTypes = assembly.Import(typeof(Type[]));
      TypeReference arrayOfArrayOfTypes = assembly.Import(typeof(Type[][]));

      // Now we create the actual type and stuff...
      TypeDefinition mapType = GetFactoryMapType(assembly);
      MethodDefinition getFactoriesMethod = new MethodDefinition(FactoryMapMethodName, MethodAttributes.Public, arrayOfArrayOfTypes);
      getFactoriesMethod.IsVirtual = true;
      mapType.Methods.Add(getFactoriesMethod);

      getFactoriesMethod.Body.Variables.Add(new VariableDefinition("map", 0, getFactoriesMethod, arrayOfArrayOfTypes));
      getFactoriesMethod.Body.Variables.Add(new VariableDefinition("row", 1, getFactoriesMethod, arrayOfTypes));
      getFactoriesMethod.Body.InitLocals = true;

      CilWorker worker = getFactoriesMethod.Body.CilWorker;
      List<Factory> factories = new List<Factory>(factoryMap.Factories);
      worker.Append(worker.Create(OpCodes.Nop));
      worker.Append(worker.Create(OpCodes.Ldc_I4, factories.Count));
      worker.Append(worker.Create(OpCodes.Newarr, arrayOfTypes));
      worker.Append(worker.Create(OpCodes.Stloc_0));
      worker.Append(worker.Create(OpCodes.Ldloc_0));

      int index = 0;
      foreach (Factory factory in factories)
      {
        worker.Append(worker.Create(OpCodes.Ldc_I4, index));

        worker.Append(worker.Create(OpCodes.Ldc_I4, 2));
        worker.Append(worker.Create(OpCodes.Newarr, systemType));
        worker.Append(worker.Create(OpCodes.Stloc_1));

        worker.Append(worker.Create(OpCodes.Ldloc_1));
        worker.Append(worker.Create(OpCodes.Ldc_I4_0));
        worker.Append(worker.Create(OpCodes.Ldtoken, factory.ObjectType));
        worker.Append(worker.Create(OpCodes.Call, getTypeFromHandle));
        worker.Append(worker.Create(OpCodes.Stelem_Ref));

        worker.Append(worker.Create(OpCodes.Ldloc_1));
        worker.Append(worker.Create(OpCodes.Ldc_I4_1));
        worker.Append(worker.Create(OpCodes.Ldtoken, factory.FactoryType));
        worker.Append(worker.Create(OpCodes.Call, getTypeFromHandle));
        worker.Append(worker.Create(OpCodes.Stelem_Ref));

        worker.Append(worker.Create(OpCodes.Ldloc_1));
        worker.Append(worker.Create(OpCodes.Stelem_Ref));
        index++;
        worker.Append(worker.Create(OpCodes.Ldloc_0));
      }

      worker.Append(worker.Create(OpCodes.Ret));
      assembly.Inject(mapType);
    }

    private static TypeDefinition GetFactoryMapType(IAssembly assembly)
    {
      foreach (TypeDefinition type in assembly.Types)
      {
        if (type.FullName == FactoryMapTypeNamespace + '.' + FactoryMapTypeName)
        {
          assembly.Types.Remove(type);
          break;
        }
      }

      // Now we create the actual type and stuff...
      TypeReference objectType = assembly.Import(typeof(Object));
      MethodReference objectCtor = assembly.Import(typeof(Object).GetConstructor(new Type[0]));

      TypeDefinition mapType = new TypeDefinition(FactoryMapTypeName, FactoryMapTypeNamespace, TypeAttributes.Public, objectType);
      mapType.Interfaces.Add(assembly.Import(typeof(IRuntimeFactoryInformation)));

      MethodDefinition defaultCtor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, assembly.Import(typeof(void)));
      CilWorker worker = defaultCtor.Body.CilWorker;
      worker.Append(worker.Create(OpCodes.Ldarg_0));
      worker.Append(worker.Create(OpCodes.Call, objectCtor));
      worker.Append(worker.Create(OpCodes.Ret));
      mapType.Constructors.Add(defaultCtor);
      return mapType;
    }
  }
}
