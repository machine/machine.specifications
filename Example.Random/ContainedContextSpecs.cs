using FluentAssertions;

using Machine.Specifications;

namespace Example.Random
{
  public static class StaticContainer
  {
    static readonly bool Foo;

    static StaticContainer()
    {
      Foo = true;
    }

    public class when_a_context_is_nested_inside_a_static_class
    {
      It should_be_run = () => Foo.Should().BeTrue();
    }
  }

  public class NonStaticContainer
  {
    static readonly bool Bar;

    static NonStaticContainer()
    {
      Bar = true;
    }

    public static class StaticContainer
    {
      static readonly bool Foo;

      static StaticContainer()
      {
        Foo = true;
      }

      public class when_a_context_is_nested_inside_a_static_class_that_is_nested_inside_a_class
      {
        It should_be_run = () =>
        {
          Foo.Should().BeTrue();
          Bar.Should().BeTrue();
        };
      }
    }
  }
}