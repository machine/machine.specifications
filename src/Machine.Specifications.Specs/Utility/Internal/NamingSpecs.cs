using FluentAssertions;

using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications.Specs.Utility.Internal
{
  [Subject(typeof(Naming))]
  public class when_formatting_specification_elements_with_underscores
  {
    static string Name;

    Because of = () => Name = "when_something_is_specified".ToFormat();

    It should_replace_single_underscores_with_spaces =
      () => Name.Should().Be("when something is specified");
  }

  [Subject(typeof(Naming))]
  public class when_formatting_specification_elements_with_double_underscores
  {
    static string Name;

    Because of = () => Name = "when__something__is_specified".ToFormat();

    It should_replace_double_underscores_with_quotes =
      () => Name.Should().Be("when \"something\" is specified");
  }

  [Subject(typeof(Naming))]
  public class when_formatting_specification_elements_with_double_underscores_and_only_a_single_double_underscore_is_given
  {
    static string Name;

    Because of = () => Name = "when__something_is_specified".ToFormat();

    It should_replace_the_double_underscore_with_two_spaces =
      () => Name.Should().Be("when  something is specified");
  }

  [Subject(typeof(Naming))]
  public class when_formatting_specification_elements_with_an_underscore_followed_by_an__s__and_another_underscore
  {
    static string Name;

    Because of = () => Name = "when_something_s_specified".ToFormat();

    It should_convert__underscore_s__to_possessive_s =
      () => Name.Should().Be("when something's specified");
  }

  [Subject(typeof(Naming))]
  public class when_formatting_specification_elements_with_an__s__enclosed_with_double_underscores
  {
    static string Name;

    Because of = () => Name = "a_single__s__for_fun".ToFormat();

    It should_replace_double_underscores_with_quotes =
      () => Name.Should().Be("a single \"s\" for fun");
  }
}