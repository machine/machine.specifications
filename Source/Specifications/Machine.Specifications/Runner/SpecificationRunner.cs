using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Machine.Specifications.Explorers;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner
{
  public class SpecificationRunner : ISpecificationRunner
  {
    readonly ISpecificationRunListener _listener;
    AssemblyExplorer _explorer;

    public SpecificationRunner(ISpecificationRunListener listener)
    {
      _listener = listener;
      _explorer = new AssemblyExplorer();
    }

    public void RunAssembly(Assembly assembly)
    {
      var contexts = _explorer.FindContextsIn(assembly);

      var assemblyContexts = new List<IAssemblyContext>(_explorer.FindAssemblyContextsIn(assembly));

      _listener.OnAssemblyStart(assembly.GetInfo());
      
      assemblyContexts.ForEach(assemblyContext=>
        assemblyContext.OnAssemblyStart());

      RunContexts(contexts);
      
      assemblyContexts.ForEach(assemblyContext=>
        assemblyContext.OnAssemblyComplete());
      
      _listener.OnAssemblyEnd(assembly.GetInfo());
    }

    public void RunAssembly(object assembly)
    {
      RunAssembly((Assembly) assembly);
    }

    public void RunContexts(IEnumerable<Model.Context> contexts)
    {
      _listener.OnRunStart();
      if (contexts.Count() == 0) return;

      foreach (var context in contexts)
      {
        if (context.Specifications.Count() == 0) continue;
        _listener.OnContextStart(context.GetInfo());
        context.RunContextBeforeAll();

        foreach (var specification in context.Specifications)
        {
          GetTestResult(context, specification);
        }

        context.RunContextAfterAll();
        _listener.OnContextEnd(context.GetInfo());
      }

      _listener.OnRunEnd();
    }

    void GetTestResult(Model.Context context, Specification specification)
    {
      _listener.OnSpecificationStart(specification.GetInfo());

      var result = context.VerifySpecification(specification);

      _listener.OnSpecificationEnd(specification.GetInfo(), result);
    }

    public void RunNamespace(Assembly assembly, string targetNamespace)
    {
      var contexts = _explorer.FindContextsIn(assembly, targetNamespace);

      RunContexts(contexts);
    }

    public void RunMember(Assembly assembly, MemberInfo member)
    {
      if (member.MemberType == MemberTypes.TypeInfo)
      {
        Type type = (Type)member;
        var description = _explorer.FindContexts(type);

        if (description == null)
        {
          return;
        }

        RunContexts(new[] {description});
      }
      else if (member.MemberType == MemberTypes.Field)
      {
        FieldInfo fieldInfo = (FieldInfo)member;
        var description = _explorer.FindContexts(fieldInfo);

        RunContexts(new[] {description});
      }
    }
  }
}