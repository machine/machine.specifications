namespace Machine.Specifications.Specs.Internal
{
  [Subject("Internal types")]
  [Tags(tag.example)]
  class when_a_context_is_internal
  {
    It should_work =
      () => true.ShouldBeTrue();
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
    protected static bool MSpecRocks;

    It should_work =
      () => MSpecRocks.ShouldBeTrue();
  }
}