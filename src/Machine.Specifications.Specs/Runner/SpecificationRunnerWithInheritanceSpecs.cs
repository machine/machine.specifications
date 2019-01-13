using Example.Random;

using FluentAssertions;

namespace Machine.Specifications.Specs.Runner
{
  [Subject("Specification Runner")]
  public class when_running_a_context_with_inherited_specifications
    : RunnerSpecs
  {
    Establish context = () =>
      {
        context_that_inherits.BaseEstablishRunCount = 0;
        context_with_inherited_specifications.BecauseClauseRunCount = 0;
        context_with_inherited_specifications.EstablishRunCount = 0;
      };

    Because of = Run<context_with_inherited_specifications>;

    It should_establish_the_context_once = () =>
                                           context_with_inherited_specifications.BecauseClauseRunCount.
                                             Should().Be(1);

    It should_invoke_the_because_clause_once = () =>
                                               context_with_inherited_specifications.EstablishRunCount.
                                                 Should().Be(1);

    It should_invoke_the_because_clause_from_the_base_class_once = () =>
                                                                   context_that_inherits.BaseEstablishRunCount.
                                                                     Should().Be(1);

    It should_detect_two_specs = () =>
                                 testListener.SpecCount.Should().Be(2);
  }

  [Subject("Specification Runner")]
  public class when_running_a_context_with_inherited_specifications_and_setup_once_per_attribute
    : RunnerSpecs
  {
    Establish context = () =>
      {
        context_that_inherits.BaseEstablishRunCount = 0;
        context_with_inherited_specifications_and_setup_for_each.BecauseClauseRunCount = 0;
        context_with_inherited_specifications_and_setup_for_each.EstablishRunCount = 0;
      };

    Because of = Run<context_with_inherited_specifications_and_setup_for_each>;

    It should_establish_the_context_twice = () =>
                                            context_with_inherited_specifications_and_setup_for_each.
                                              BecauseClauseRunCount.Should().Be(2);

    It should_invoke_the_because_clause_twice = () =>
                                                context_with_inherited_specifications_and_setup_for_each.
                                                  EstablishRunCount.Should().Be(2);

    It should_invoke_the_because_clause_from_the_base_class_twice = () =>
                                                                    context_that_inherits.BaseEstablishRunCount.
                                                                      Should().Be(2);

    It should_detect_two_specs = () =>
                                 testListener.SpecCount.Should().Be(2);
  }
}