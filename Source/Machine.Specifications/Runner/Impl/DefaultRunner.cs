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
    readonly AssemblyRunner _assemblyRunner;
    Action _runStart;
    Action _runEnd;

    public DefaultRunner(ISpecificationRunListener listener, RunOptions options)
    {
      _listener = listener;
      _options = options;
      _assemblyRunner = new AssemblyRunner(_listener, _options);

      _explorer = new AssemblyExplorer();

      _runStart = () => _listener.OnRunStart();
      _runEnd = () => _listener.OnRunEnd();
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
      if (member.MemberType == MemberTypes.TypeInfo ||
          member.MemberType == MemberTypes.NestedType)
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
      var fieldInfo = (FieldInfo)member;
      var context = _explorer.FindContexts(fieldInfo);

      StartRun(CreateMap(assembly, new[] {context}));
    }

    void RunClass(MemberInfo member, Assembly assembly)
    {
      var type = (Type)member;
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
      _runStart();

      foreach (var pair in contextMap)
      {
        var assembly = pair.Key;
        // TODO: move this filtering to a more sensible place
        var contexts = pair.Value.FilteredBy(_options);

        if (contexts.Any())
        {
          StartAssemblyRun(assembly, contexts);
        }
      }

      _runEnd();
    }

    void StartAssemblyRun(Assembly assembly, IEnumerable<Context> contexts)
    {
      _assemblyRunner.Run(assembly, contexts.FilteredBy(_options));
    }

    public void StartRun(Assembly assembly)
    {
      _runStart = () => {};
      _runEnd = () => {};

      _listener.OnRunStart();
      _assemblyRunner.StartExplicitRunScope(assembly);
    }

    public void EndRun(Assembly assembly)
    {
      _assemblyRunner.EndExplicitRunScope(assembly);
      _listener.OnRunEnd();
    }
  }

  public static class TagFilteringExtensions
  {
    public static IEnumerable<Context> FilteredBy(this IEnumerable<Context> contexts, RunOptions options)
    {
      var results = contexts;

      if (options.Filters.Any())
      {
        var includeFilters = options.Filters;

        results = results.Where(x => includeFilters.Any(filter => StringComparer.OrdinalIgnoreCase.Equals(filter, x.Type.FullName)));
      }

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