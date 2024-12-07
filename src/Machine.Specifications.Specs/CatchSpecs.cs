using System;
using System.Threading.Tasks;

using FluentAssertions;

namespace Machine.Specifications.Specs
{
  [Subject(typeof(Catch))]
  public class when_calling_Catch_Exception_with_an_Action
  {
    [Subject(typeof(Catch))]
    public class with_a_throwing_Action
    {
      static ArgumentException AnException;
      static Exception Result;

      Establish context = () => { AnException = new ArgumentException(); };

      Because of = () => { Result = Catch.Exception(() => { throw AnException; }); };

      It should_return_the_same_exception =
        () => Result.Should().BeSameAs(AnException);
    }

    [Subject(typeof(Catch))]
    public class with_a_non_throwing_Action
    {
      static string ActionSideEffect;
      static Exception Result;

      Because of = () => { Result = Catch.Exception(() => { ActionSideEffect = "hi"; }); };

      It should_access_the_propety =
        () => ActionSideEffect.Should().Be("hi");

      It should_return_null =
        () => Result.Should().BeNull();
    }
  }

  [Subject(typeof(Catch))]
  public class when_calling_Catch_Exception_with_a_Func
  {
    class Dummy
    {
      public static readonly ArgumentException AnException = new ArgumentException();

      public static string ThrowingProperty
      {
        get { throw AnException; }
      }

      public static string NonThrowingProperty
      {
        get { return "hi"; }
      }
    }

    [Subject(typeof(Catch))]
    public class with_a_throwing_Func
    {
      static Exception Result;

      Because of = () => { Result = Catch.Exception(() => Dummy.ThrowingProperty); };

      It should_return_the_same_exception =
        () => Result.Should().BeSameAs(Dummy.AnException);
    }

    [Subject(typeof(Catch))]
    public class with_a_non_throwing_Func
    {
      static Exception Result;
      static string PropertyValue;

      Because of = () => { Result = Catch.Exception(() => PropertyValue = Dummy.NonThrowingProperty); };

      It should_access_the_propety =
        () => PropertyValue.Should().Be("hi");

      It should_return_null =
        () => Result.Should().BeNull();
    }
  }

  [Subject(typeof(Catch))]
  public class when_calling_Catch_Only_with_an_Action
  {
    [Subject(typeof(Catch))]
    public class with_a_throwing_Action_which_matches_exception_to_be_caught
    {
      static ArgumentException AnException;
      static Exception Result;

      Establish context = () => { AnException = new ArgumentException(); };

      Because of = () => { Result = Catch.Only<ArgumentException>(() => { throw AnException; }); };

      It should_return_the_same_exception =
        () => Result.Should().BeSameAs(AnException);
    }

    [Subject(typeof(Catch))]
    public class with_a_throwing_Action_which_doesnt_match_exception_to_be_caught
    {
      static ArgumentException AnException;
      static Exception Result;

      Establish context = () => { AnException = new ArgumentException(); };

      Because of = () => { Result = Catch.Exception(() => Catch.Only<InvalidOperationException>(() => { throw AnException; })); };

      It should_return_the_same_exception =
        () => Result.Should().BeSameAs(AnException);
    }

    [Subject(typeof(Catch))]
    public class with_a_non_throwing_Action
    {
      static string ActionSideEffect;
      static Exception Result;

      Because of = () => { Result = Catch.Only<ArgumentException>((() => { ActionSideEffect = "hi"; })); };

      It should_access_the_propety =
        () => ActionSideEffect.Should().Be("hi");

      It should_return_null =
          () => Result.Should().BeNull();
        }
    }

  [Subject(typeof(Catch))]
  public class when_calling_Catch_OnlyAsync_with_an_Async_Func
  {
      [Subject(typeof(Catch))]
      public class with_a_throwing_Action_which_matches_exception_to_be_caught
      {
          static ArgumentException AnException;
          static Exception Result;
          static Task TestAsync() => Task.Run(() => throw AnException);

          Establish context = () => { AnException = new ArgumentException(); };

          Because of = async () => { Result = await Catch.OnlyAsync<ArgumentException>(TestAsync); };

          It should_return_the_same_exception =
              () => Result.Should().BeSameAs(AnException);
      }

      [Subject(typeof(Catch))]
      public class with_a_throwing_Action_which_doesnt_match_exception_to_be_caught
      {
          static ArgumentException AnException;
          static Exception Result;
          static Task TestAsync() => Task.Run(() => throw AnException);

          Establish context = () => { AnException = new ArgumentException(); };

          Because of = async () => { Result = await Catch.ExceptionAsync(async () => await Catch.OnlyAsync<InvalidOperationException>(TestAsync)); };

          It should_return_the_same_exception =
              () => Result.Should().BeSameAs(AnException);
      }

      [Subject(typeof(Catch))]
      public class with_a_non_throwing_Action
      {
          static string ActionSideEffect;
          static Exception Result;

          static Task TestAsync() => Task.Run(() => { ActionSideEffect = "hi"; });

          Because of = async () => { Result = await Catch.OnlyAsync<ArgumentException>(TestAsync); };

          It should_access_the_propety =
              () => ActionSideEffect.Should().Be("hi");

          It should_return_null =
              () => Result.Should().BeNull();
      }
  }

    [Subject(typeof(Catch))]
    public class when_calling_catch_with_async_methods
    {
        static Exception exception;

        [Subject(typeof(Catch))]
        public class with_a_non_throwing_action
        {
            static Task Test() => Task.Run(() => { });

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_return_null = () =>
                exception.Should().BeNull();
        }

        [Subject(typeof(Catch))]
        public class with_a_throwing_action
        {
            static Task Test() => Task.Run(() => throw new ArgumentNullException());

            Because of = async () =>
                exception = await Catch.ExceptionAsync(Test);

            It should_return_exception = () =>
                exception.Should().BeOfType<ArgumentNullException>();
        }

        [Subject(typeof(Catch))]
        public class calling_wrong_catch_method
        {
            static Task Test() => Task.Run(() => throw new ArgumentNullException());

            Because of = () =>
                exception = Catch.Exception(() => Catch.Exception(Test));

            It should_return_exception = () =>
                exception.Should().BeOfType<InvalidOperationException>();

            It should_contain_message = () =>
                exception.Message.Should().Be("You must use Catch.ExceptionAsync for async methods");
        }
    }
}
