using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Machine.Specifications.Explorers;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl
{
  public class DefaultRunner : ISpecificationRunner
  {
    readonly ISpecificationRunListener _listener;
    readonly RunOptions _options;
    readonly AssemblyExplorer _explorer;

    public DefaultRunner(ISpecificationRunListener listener, RunOptions options)
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
      var runner = new AssemblyRunner(_listener, _options);
      runner.Run(assembly, contexts);
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