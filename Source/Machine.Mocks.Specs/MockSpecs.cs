using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Mocks.Exceptions;
using Machine.Specifications;

namespace Machine.Mocks.MockSpecs
{
  public class Creating_mock_interfaces
  {
    static IFoo mock;

    When a_mock_is_created = () =>
    {
      mock = Mock.Of<IFoo>();
    };

    It should_not_be_null = () =>
    {
      mock.ShouldNotBeNull();
    };

    When a_mock_of_an_interface_is_created_with_constructor_arguments =()=>
    {
      mock = Mock.Of<IFoo>(0);
    };

    It_should_throw a_mock_exception = exception =>
    {
      exception.ShouldBeOfType<MockUsageException>();
    };
  }

  public class Creating_mock_classes
  {
    static Foo mock;

    When a_mock_is_created = () =>
    {
      mock = Mock.Of<Foo>();
    };

    It should_not_be_null = () =>
    {
      mock.ShouldNotBeNull();
    };
  }

  public class Verifying_a_command_on_a_mock_that_was_called
  {
    static IFoo mock;

    Context before_each = () =>
    {
      mock = Mock.Of<IFoo>();
      mock.Command();
    };

    When command_call_is_verified = () =>
    {
      mock.Command();
      Should.HaveBeenCalled();
    };

    It should_not_throw = () =>
    {
    };
  }

  public class Verifying_a_command_on_a_mock_that_was_not_called
  {
    static IFoo mock;

    Context before_each = () =>
    {
      mock = Mock.Of<IFoo>();
    };

    When command_call_is_verified = () =>
    {
      mock.Command();
      Should.HaveBeenCalled();
    };

    It_should_throw a_mock_verification_exception = exception =>
    {
      exception.ShouldBeOfType<MockVerificationException>();
    };
  }
}
