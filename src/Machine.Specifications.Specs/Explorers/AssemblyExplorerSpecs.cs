using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Example.Random;

using FluentAssertions;

using Machine.Specifications.Explorers;
using Machine.Specifications.Model;

namespace Machine.Specifications.Specs.Explorers
{
  [Subject(typeof(AssemblyExplorer))]
  public class When_inspecting_internal_types_for_contexts
  {
    static AssemblyExplorer Explorer;
    static IEnumerable<Context> Contexts;
    Establish context = () => { Explorer = new AssemblyExplorer(); };

    Because of =
      () => { Contexts = Explorer.FindContextsIn(typeof(tag).GetTypeInfo().Assembly, "Example.Random.Internal"); };

    It should_find_two_contexts =
      () => Contexts.Count().Should().Be(2);
  }
}