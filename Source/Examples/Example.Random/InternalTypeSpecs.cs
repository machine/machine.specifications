using FluentAssertions;

using Machine.Specifications;

namespace Example.Random.Internal
{
  [Subject("Internal types")]
  [Tags(tag.example)]
  class when_a_context_is_internal
  {
    It should_work =
      () => true.Should().BeTrue();
  }

  [Subject("Internal types")]
  [Tags(tag.example)]
  class when_a_context_is_internal_and_uses_internal_behaviors
  {
    protected static bool MSpecRocks = true;

    Because of = () => { MSpecRocks = true; };

    Behaves_like<WorkingSpecs> a_working_spec;
  }

  [Behaviors]
  class WorkingSpecs
  {
#pragma warning disable 0649
    protected static bool MSpecRocks;
#pragma warning restore 0649

    It should_work =
      () => MSpecRocks.Should().BeTrue();
  }
}