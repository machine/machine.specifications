#if !NET35

using System;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

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
  public class when_an_async_operation_runs_with_await : AsyncSpecs
  {
    static string Result;

    Because of = () => { Result = Delayed.Echo("result").Await(); };

    It should_wait_for_completion =
      () => Result.Should().Be("result");
  }

  [Subject(typeof(TaskSpecificationExtensions), "exception")]
  public class when_an_async_operation_fails_without_await : AsyncSpecs
  {
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => Delayed.Fail());

    It should_not_capture_the_exception =
      () => exception.Should().BeNull();
  }

  [Subject(typeof(TaskSpecificationExtensions), "exception")]
  public class when_a_single_async_operation_fails_with_await : AsyncSpecs
  {
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => Delayed.Fail().Await());

    It should_capture_the_first_exception =
      () => exception.Should().BeOfType<InvalidOperationException>();
  }

  [Subject(typeof(TaskSpecificationExtensions), "exception")]
  public class when_multiple_async_operations_fail_with_await : AsyncSpecs
  {
    static Exception exception;

    Because of = () => exception = Catch.Exception(() => Delayed.MultipleFails().Await());

    It should_capture_the_aggregate_exception =
      () => exception.Should().BeOfType<AggregateException>();

    It should_capture_all_inner_exceptions =
      () => ((AggregateException)exception).InnerExceptions.Count.Should().Be(2);
  }

  public class AsyncSpecs
  {
  }
}
#endif