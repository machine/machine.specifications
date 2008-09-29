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
    readonly RunOptions _options;
    readonly AssemblyExplorer _explorer;

    public SpecificationRunner(ISpecificationRunListener listener, RunOptions options)
    {
      _listener = listener;
      _options = options;
      _explorer = new AssemblyExplorer();
    }

    public void RunAssembly(Assembly assembly)
    {
      var contexts = _explorer.FindContextsIn(assembly);
      var map = CreateMap(assembly, contexts);

      StartRun(map);
    }

    public void RunAssemblies(IEnumerable<Assembly> assemblies)
    {
      var map = new Dictionary<Assembly, IEnumerable<Context>>();

      assemblies.Each(assembly => map.Add(assembly, _explorer.FindContextsIn(assembly)));

      StartRun(map);
    }

    public void RunNamespace(Assembly assembly, string targetNamespace)
    {
      var contexts = _explorer.FindContextsIn(assembly, targetNamespace);

      StartRun(CreateMap(assembly, contexts));
    }

    public void RunMember(Assembly assembly, MemberInfo member)
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

    static Dictionary<Assembly, IEnumerable<Context>> CreateMap(Assembly assembly, IEnumerable<Context> contexts)
    {
      var map = new Dictionary<Assembly, IEnumerable<Context>>();
      map[assembly] = contexts;
      return map;
    }

    void StartRun(IDictionary<Assembly, IEnumerable<Context>> contextMap)
    {
      _listener.OnRunStart();

      foreach (var pair in contextMap)
      {
        var assembly = pair.Key;
        var contexts = pair.Value;

        StartAssemblyRun(assembly, contexts);
      }

      _listener.OnRunEnd();
    }

    void StartAssemblyRun(Assembly assembly, IEnumerable<Context> contexts)
    {
      var assemblyContexts = new List<IAssemblyContext>(_explorer.FindAssemblyContextsIn(assembly));

      _listener.OnAssemblyStart(assembly.GetInfo());
        
      assemblyContexts.ForEach(assemblyContext => assemblyContext.OnAssemblyStart());

      RunContexts(contexts.FilteredBy(_options));
        
      assemblyContexts.ForEach(assemblyContext => assemblyContext.OnAssemblyComplete());
        
      _listener.OnAssemblyEnd(assembly.GetInfo());
    }

    private void RunContexts(IEnumerable<Context> contexts)
    {
      foreach (var context in contexts)
      {
        if (context.Specifications.Count() == 0) continue;

        RunContext(context);
      }
    }

    void RunContext(Context context)
    {
      _listener.OnContextStart(context.GetInfo());

      foreach (var specification in context.EnumerateSpecificationsForVerification())
      {
        _listener.OnSpecificationStart(specification.GetInfo());
        var result = context.VerifyOrIgnoreSpecification(specification);
        _listener.OnSpecificationEnd(specification.GetInfo(), result);
      }

      _listener.OnContextEnd(context.GetInfo());
    }
  }

  public static class ContextFilteringExtensions
  {
    public static IEnumerable<Context> FilteredBy(this IEnumerable<Context> contexts, RunOptions options)
    {
      var results = contexts;

      if (options.IncludeTags.Any())
      {
        var tags = options.IncludeTags.Select(tag => new Tag(tag));

        results = results.Where(x => x.Tags.Intersect(tags).Any());
      }

      if (options.ExcludeTags.Any())
      {
        var tags = options.ExcludeTags.Select(tag => new Tag(tag));
        results = results.Where(x => !x.Tags.Intersect(tags).Any());
      }

      return results;
    }
  }
}