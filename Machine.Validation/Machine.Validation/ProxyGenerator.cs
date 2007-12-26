using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Machine.Validation
{
  public class ProxyGenerator
  {
    private Dictionary<string, Assembly> _assemblies;
    private AssemblyBuilder _assemblyBuilder;
    private ModuleBuilder _moduleBuilder;

    public Type GenerateProxy(Type type)
    {
      if (_assemblyBuilder == null)
      {
        AppDomain appDomain = Thread.GetDomain();
        string generatedAssemblyName = type.Name + "SuperDuperGenerated";
        AssemblyName assemblyName = new AssemblyName(generatedAssemblyName);
        _assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
        _moduleBuilder = _assemblyBuilder.DefineDynamicModule(generatedAssemblyName, generatedAssemblyName + ".dll");
      }
      TypeBuilder typeBuilder = _moduleBuilder.DefineType("SuperDuperGeneratedType", TypeAttributes.Class  | TypeAttributes.BeforeFieldInit | TypeAttributes.Public, type);

      foreach (PropertyInfo propertyInfo in type.GetProperties())
      {
      }

      foreach (MethodInfo method in type.GetMethods())
      {
        if (method.IsVirtual)
        {
          List<Type> parameterTypes = new List<Type>();
          foreach (ParameterInfo parameterInfo in method.GetParameters())
          {
            parameterTypes.Add(parameterInfo.ParameterType);
          }
          MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, method.Attributes, method.ReturnType, parameterTypes.ToArray());
          ILGenerator il = methodBuilder.GetILGenerator();
          il.Emit(OpCodes.Nop);
          il.Emit(OpCodes.Ldstr, "HI");
          il.Emit(OpCodes.Throw);
          //typeBuilder.DefineMethodOverride(methodBuilder, method);
        }
      }
      Type generatedType = typeBuilder.CreateType();

      _assemblyBuilder.Save(@"Test.dll");
      return generatedType;
    }
  }
}
