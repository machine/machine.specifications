using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner
{
  public class SpecificationRunner : ISpecificationRunner
  {
    readonly ISpecificationRunListener _listener;
    readonly AssemblyExplorer _explorer;

    public SpecificationRunner(ISpecificationRunListener listener)
    {
      _listener = listener;
      _explorer = new AssemblyExplorer();
    }

    public void RunAssembly(Assembly assembly, RunOptions options)
    {
      var contexts = _explorer.FindContextsIn(assembly);
      var map = CreateMap(assembly, contexts);

      StartRun(map);
    }

    public void RunAssemblies(IEnumerable<Assembly> assemblies, RunOptions options)
    {
      var map = new Dictionary<Assembly, IEnumerable<Context>>();

      assemblies.Each(assembly => map.Add(assembly, _explorer.FindContextsIn(assembly)));

      StartRun(map);
    }

    public void RunNamespace(Assembly assembly, string targetNamespace, RunOptions options)
    {
      var contexts = _explorer.FindContextsIn(assembly, targetNamespace);

      StartRun(CreateMap(assembly, contexts));
    }

    public void RunMember(Assembly assembly, MemberInfo member, RunOptions options)
    {
      if (member.MemberType == MemberTypes.TypeInfo)
      {
        RunClass(member, assembly);
      }
      else if (member.MemberType == MemberTypes.Field)
      {
        RunField(member, assembly);
      }
    }

    void RunField(MemberInfo member, Assembly assembly)
    {
      FieldInfo fieldInfo = (FieldInfo)member;
      var context = _explorer.FindContexts(fieldInfo);

      StartRun(CreateMap(assembly, new[] {context}));
    }

    void RunClass(MemberInfo member, Assembly assembly)
    {
      Type type = (Type)member;
      var context = _explorer.FindContexts(type);

      if (context == null)
      {
        return;
      }

      StartRun(CreateMap(assembly, new[] {context}));
    }

    Dictionary<Assembly, IEnumerable<Context>> CreateMap(Assembly assembly, IEnumerable<Context> contexts)
    {
      var map = new Dictionary<Assembly, IEnumerable<Context>>();
      map[assembly] = contexts;
      return map;
    }

    private void StartRun(IDictionary<Assembly, IEnumerable<Context>> contextMap)
    {
      _listener.OnRunStart();

      foreach (var pair in contextMap)
      {
        var assembly = pair.Key;
        var contexts = pair.Value;

        var assemblyContexts = new List<IAssemblyContext>(_explorer.FindAssemblyContextsIn(assembly));

        _listener.OnAssemblyStart(assembly.GetInfo());
        
        assemblyContexts.ForEach(assemblyContext=>
          assemblyContext.OnAssemblyStart());

        RunContexts(contexts);
        
        assemblyContexts.ForEach(assemblyContext=>
          assemblyContext.OnAssemblyComplete());
        
        _listener.OnAssemblyEnd(assembly.GetInfo());
      }

      _listener.OnRunEnd();
    }

    private void RunContexts(IEnumerable<Context> contexts)
    {
      if (contexts.Count() == 0) return;

      foreach (var context in contexts)
      {
        if (context.Specifications.Count() == 0) continue;

        _listener.OnContextStart(context.GetInfo());

        foreach (var iteration in context.EnumerateAndVerifyAllSpecifications())
        {
          if (iteration.Current != null)
          {
            _listener.OnSpecificationEnd(iteration.Current.GetInfo(), iteration.Result);
          }

          if (iteration.Next != null)
          {
            _listener.OnSpecificationStart(iteration.Next.GetInfo());
          }
        }

        _listener.OnContextEnd(context.GetInfo());
      }
    }

  }
}