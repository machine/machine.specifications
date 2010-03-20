using System;
using System.Collections.Generic;

namespace Machine.Specifications
{
  public enum Status
  {
    Failing,
    Passing,
    NotImplemented,
    Ignored
  }

  [Serializable]
  public class ExceptionResult
  {
    readonly string _toString;

    public string FullTypeName { get; private set; }
    public string TypeName { get; private set; }
    public string Message { get; private set; }
    public string StackTrace { get; private set; }
    public ExceptionResult InnerExceptionResult { get; private set; }

    public ExceptionResult(Exception exception)
    {
      FullTypeName = exception.GetType().FullName;
      TypeName = exception.GetType().Name;
      Message = exception.Message;
      StackTrace = FilterStackTrace( exception.StackTrace);

      if (exception.InnerException != null)
      {
        InnerExceptionResult = new ExceptionResult(exception.InnerException);
      }

      _toString = exception.ToString();
    }

    public override string ToString()
    {
      return _toString;
    }

    #region Borrowed from XUnit to clean up the stack trace, licened under MS-PL

#if CLEAN_EXCEPTION_STACK_TRACE
    /// <summary>
    /// Filters the stack trace to remove all lines that occur within the testing framework.
    /// </summary>
    /// <param name="stackTrace">The original stack trace</param>
    /// <returns>The filtered stack trace</returns>
    static string FilterStackTrace(string stackTrace)
    {
      if (stackTrace == null)
        return null;      

      List<string> results = new List<string>();

      foreach (string line in SplitLines(stackTrace))
      {
        string trimmedLine = line.TrimStart();
        if (!IsFrameworkStackFrame(trimmedLine))
          results.Add(line);
      }

      return string.Join(Environment.NewLine, results.ToArray());
    }

    static bool IsFrameworkStackFrame(string trimmedLine)
    {
      // Anything in the Machine.Specifications namespace
      return trimmedLine.StartsWith("at Machine.Specifications.");
    }

    // Our own custom String.Split because Silverlight/CoreCLR doesn't support the version we were using
    static IEnumerable<string> SplitLines(string input)
    {
      while (true)
      {
        int index = input.IndexOf(Environment.NewLine);

        if (index < 0)
        {
          yield return input;
          break;
        }

        yield return input.Substring(0, index);
        input = input.Substring(index + Environment.NewLine.Length);
      }
    }
#else
    // Do not change the line at all if you are not going to clean it
    static string FilterStackTrace(string stackTrace)
    {
      return stackTrace;
    }
#endif

    #endregion
  }

  [Serializable]
  public class Result
  {
    readonly Status _status;
    readonly IDictionary<string, IDictionary<string, string>> _supplements = new Dictionary<string, IDictionary<string, string>>();

    public IDictionary<string, IDictionary<string, string>> Supplements
    {
      get { return _supplements; }
    }

    public bool Passed
    {
      get { return _status == Status.Passing; }
    }

    public ExceptionResult Exception { get; private set; }

    public Status Status
    {
      get { return _status; }
    }

    public string ConsoleOut
    {
      get;
      internal set;
    }

    public string ConsoleError
    {
      get;
      internal set;
    }

    public bool HasSupplement(string name)
    {
      return _supplements.ContainsKey(name);
    }

    public IDictionary<string, string> GetSupplement(string name)
    {
      return _supplements[name];
    }

    private Result(Exception exception)
    {
      _status = Status.Failing;
      this.Exception = new ExceptionResult(exception);
    }

    private Result(Status status)
    {
      _status = status;
    }

    private Result(Result result, string supplementName, IDictionary<string, string> supplement)
    {
      _status = result.Status;
      this.Exception = result.Exception;

      foreach (var pair in result._supplements)
      {
        _supplements.Add(pair);
      }

      if (HasSupplement(supplementName))
      {
        throw new ArgumentException("Result already has supplement named: " + supplementName, "supplementName");
      }

      _supplements.Add(supplementName, supplement);
      this.ConsoleOut = result.ConsoleOut;
      this.ConsoleError = result.ConsoleError;
    }

    public static Result Pass()
    {
      return new Result(Status.Passing);
    }

    public static Result Ignored()
    {
      return new Result(Status.Ignored);
    }

    public static Result NotImplemented()
    {
      return new Result(Status.NotImplemented);
    }

    public static Result Failure(Exception exception)
    {
      return new Result(exception);
    }

    public static Result ContextFailure(Exception exception)
    {
      return new Result(exception);
    }

    public static Result Supplement(Result result, string supplementName, IDictionary<string, string> supplement)
    {
      return new Result(result, supplementName, supplement);
    }
  }
}
