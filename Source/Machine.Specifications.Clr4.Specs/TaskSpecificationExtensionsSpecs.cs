using System;
using System.Threading;
using System.Threading.Tasks;

namespace Machine.Specifications.Clr4.Specs
{
  public static class Delayed
  {
    static readonly TimeSpan WaitTimeout = TimeSpan.FromMilliseconds(500);

    public static Task<string> Echo(string toEcho)
    {
      return Task.Factory.StartNew(() =>
      {
        Thread.Sleep(WaitTimeout);

        return toEcho;
      });
    }

    public static Task Fail()
    {
      return Task.Factory.StartNew(() =>
      {
        Thread.Sleep(WaitTimeout);

        throw new InvalidOperationException("something went wrong");

      });
    }

    public static Task MultipleFails()
    {
      return Task.Factory.StartNew(() => Task.WaitAll(Fail(), Fail()));
    }
  }

  [Subject(typeof(TaskSpecificationExtensions))]
  public class when_an_async_operation_runs_without_await
  {
    static Task<string> Result;

    Because of = () => { Result = Delayed.Echo("result"); };

    It should_not_wait_for_completion =
      () => Result.IsCompleted.ShouldBeFalse();
  }

  [Subject(typeof(TaskSpecificationExtensions))]
  public class when_an_async_operation_runs_with_await
  {
    static string Result;

    Because of = () => { Result = Delayed.Echo("result").Await(); };

    It should_wait_for_completion =
      () => Result.ShouldEqual("result");
  }

  [Subject(typeof(TaskSpecificationExtensions), "exception")]
  public class when_an_async_operation_fails_without_await
  {
    static Exception Exception;

    Because of = () => Exception = Catch.Exception(() => Delayed.Fail());

    It should_not_capture_the_exception =
      () => Exception.ShouldBeNull();
  }

  [Subject(typeof(TaskSpecificationExtensions), "exception")]
  public class when_a_single_async_operation_fails_with_await
  {
    static Exception Exception;

    Because of = () => Exception = Catch.Exception(() => Delayed.Fail().Await());

    It should_capture_the_first_exception =
      () => Exception.ShouldBeOfType<InvalidOperationException>();
  }

  [Subject(typeof(TaskSpecificationExtensions), "exception")]
  public class when_multiple_async_operations_fail_with_await
  {
    static Exception Exception;

    Because of = () => Exception = Catch.Exception(() => Delayed.MultipleFails().Await());

    It should_capture_the_aggregate_exception =
      () => Exception.ShouldBeOfType<AggregateException>();

    It should_capture_all_inner_exceptions =
      () => ((AggregateException) Exception).InnerExceptions.Count.ShouldEqual(2);
  }
}