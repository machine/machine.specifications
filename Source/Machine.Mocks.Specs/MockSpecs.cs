using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Mocks.Exceptions;
using Machine.Specifications;

namespace Machine.Mocks.MockSpecs
{
  [Concern(typeof(Mock))]
  public class When_creating_a_mock
  {
    static IFoo mock;

    Establish context = () =>
    {
      mock = Mock.Of<IFoo>();
    };

    It should_not_be_null = () =>
    {
      mock.ShouldNotBeNull();
    };
  }

  [Concern(typeof(Mock))]
  public class When_creating_a_mock_of_an_interface_with_constructor_arguments
  {
    static IFoo mock;
    static Exception exception;

    Establish context =()=>
    {
      exception = Catch.Exception(()=>
        mock = Mock.Of<IFoo>(0)
      );
    };

    It should_throw_a_mock_exception = ()=>
    {
      exception.ShouldBeOfType<MockUsageException>();
    };
  }

  [Concern(typeof(Mock))]
  public class When_creating_a_mock_of_a_class
  {
    static Foo mock;

    Because a_mock_is_created = () =>
    {
      mock = Mock.Of<Foo>();
    };

    It should_not_be_null = () =>
    {
      mock.ShouldNotBeNull();
    };
  }

  [Concern(typeof(Mock))]
  public class When_verifying_a_command_on_a_mock_that_was_called
  {
    static IFoo mock;

    Establish context = () =>
    {
      mock = Mock.Of<IFoo>();
      mock.Command();
    };

    Because of = () =>
    {
      mock.Command();
      Should.HaveBeenCalled();
    };

    It should_not_throw = () =>
    {
    };
  }

  [Concern(typeof(Mock))]
  public class When_verifying_a_command_on_a_mock_that_was_not_called
  {
    static IFoo mock;
    static Exception exception;

    Establish context = () =>
    {
      mock = Mock.Of<IFoo>();
    };

    Because of = () =>
    {
      exception = Catch.Exception(()=>
      {
        mock.Command();
        Should.HaveBeenCalled();
      });
    };

    It should_throw_a_mock_verification_exception = ()=>
    {
      exception.ShouldBeOfType<MockVerificationException>();
    };
  }
}
