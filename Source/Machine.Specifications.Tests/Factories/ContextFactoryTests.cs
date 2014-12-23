using System;
using System.Collections.Generic;
using System.Linq;

using Example.Random;

using FluentAssertions;

using Machine.Specifications.Model;

using NUnit.Framework;

namespace Machine.Specifications.Factories
{
  [TestFixture]
  public class ContextFactoryTests : With<ContextWithSingleSpecification>
  {
    [Test]
    public void ShouldSetType()
    {
      context.Type.Name.Should().Be("ContextWithSingleSpecification");
    }
  }

  [TestFixture]
  public class when_using_nested_contexts
  {
    public static IList<int> numbers;
    ContextFactory factory;
    Context newContext;

    public class top_of_hierarchy
    {
      Establish first = () => { numbers.Add(1); };

      public class inner
      {
        Establish second = () => { numbers.Add(2); };
      }
    }

    [SetUp]
    public void setup()
    {
      numbers = new List<int>();
      factory = new ContextFactory();
      newContext = factory.CreateContextFrom(new top_of_hierarchy.inner());
      newContext.EstablishContext();
    }

    [TearDown]
    public void teardown()
    {
      numbers.Clear();
    }

    [Test]
    public void establish_blocks_run_in_the_correct_order()
    {
      Assert.AreEqual(2, numbers.Count());
      Assert.AreEqual(1, numbers[0]);
      Assert.AreEqual(2, numbers[1]);
    }
  }

  [TestFixture]
  public class when_creating_a_nested_context
  {
    public static List<int> numbers;
    ContextFactory factory;
    Context newContext;

    public class top_of_hierarchy
    {
      Establish first = () =>
        numbers.Add(1);

      public class some_other_base
      {
        Establish c = () =>
          numbers.Add(2);
      }

      public class inner : some_other_base
      {
        Establish third = () =>
          numbers.Add(3);
      }
    }

    [SetUp]
    public void setup()
    {
      numbers = new List<int>();
      factory = new ContextFactory();
      newContext = factory.CreateContextFrom(new top_of_hierarchy.inner());
      newContext.EstablishContext();
    }

    [Test]
    public void establish_blocks_run_in_the_correct_order()
    {
      var results = numbers.Distinct().ToList();
      Assert.AreEqual(1, results[0]);
      Assert.AreEqual(2, results[1]);
      Assert.AreEqual(3, results[2]);
    }
  }

  [TestFixture]
  public class when_creating_a_context_that_is_nested_inside_of_a_generic_class
  {
    [TestFixture]
    public class and_the_nested_context_is_not_a_generic_type_definition
    {
      Context newContext;
      ContextFactory factory;

      [SetUp]
      public void setup()
      {
        factory = new ContextFactory();
      }

      [Test]
      public void should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic()
      {
        newContext = factory.CreateContextFrom(new generic_container<int>.nested_context());
        Assert.NotNull(newContext);
      }
    }

    [TestFixture]
    public class and_the_nested_context_is_a_generic_type_definition
    {
      Context newContext;
      ContextFactory factory;

      [SetUp]
      public void setup()
      {
        factory = new ContextFactory();
      }

      [Test]
      public void should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic()
      {
        newContext = factory.CreateContextFrom(new generic_container<int>.nested_generic_context<string>());
        Assert.NotNull(newContext);
      }
    }

    [TestFixture]
    public class and_there_are_multiple_generic_parents
    {
      [TestFixture]
      public class and_the_nested_context_is_not_generic
      {
        Context newContext;
        ContextFactory factory;

        [SetUp]
        public void setup()
        {
          factory = new ContextFactory();
          newContext =
            factory.CreateContextFrom(
                                      new generic_container<int>.nested_generic_context<string>.
                                        nested_nested_non_generic());
        }

        [Test]
        public void should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic()
        {
          Assert.NotNull(newContext);
        }
      }

      [TestFixture]
      public class and_the_nested_context_generic
      {
        Context newContext;
        ContextFactory factory;

        [SetUp]
        public void setup()
        {
          factory = new ContextFactory();
          newContext = factory.CreateContextFrom(
                                                 new generic_container<int>.nested_generic_context<string>.
                                                   nested_nested_generic
                                                   <bool>());
        }

        [Test]
        public void should_be_able_to_create_the_context_even_though_the_enclosing_class_is_generic()
        {
          Assert.NotNull(newContext);
        }
      }
    }
  }
}