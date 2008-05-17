using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Machine.Mocks.Specs
{
  public class Creating_a_mock_interface
  {
    static IFoo mock;

    When a_mock_is_created =()=>
    {
      mock = Mock.Of<IFoo>();
    };

    It should_not_be_null = ()=>
    {
      mock.ShouldNotBeNull();
    };
  }

  public class Creating_a_mock_interface_with_constructor_arguments
  {
    static IFoo mock;

    When a_mock_is_created_with_constructor_arguments =()=>
    {
      mock = Mock.Of<IFoo>(0);
    };

    It_should_throw a_mock_exception =exception=>
    {
      exception.ShouldBeOfType<MocksException>();
    };
  }

  public class Creating_a_mock_class
  {
    static Foo mock;

    When a_mock_is_created =()=>
    {
      mock = Mock.Of<Foo>();
    };

    It should_not_be_null = ()=>
    {
      mock.ShouldNotBeNull();
    };
  }

  public class Verifying_a_command_on_a_mock
  {
    static IFoo mock;

    Context before_each = ()=>
    {
      mock = Mock.Of<IFoo>();
      mock.Command();
    };

    When command_call_is_verified =()=>
    {
      mock.Command(); 
      Should.HaveBeenCalled();
    };

    It should_not_throw = ()=>
    {
    };
  }

  public class Foo
  {
    
  }

  public interface IFoo
  {
    bool Query();
    int Query(int i);
    string Query(string s);
    void Command(int i);
    void Command(string i);
    void Command();
  }
}
