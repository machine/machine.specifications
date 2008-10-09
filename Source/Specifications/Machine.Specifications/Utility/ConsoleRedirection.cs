using System;
using System.IO;

namespace Machine.Specifications.Utility
{
  internal class ConsoleRedirection : IDisposable
  {
    public ConsoleRedirection()
    {
      ConsoleOut = new StringWriter();
      ConsoleError = new StringWriter();
      Streams = new ConsoleStreams();
    }

    TextWriter DefaultConsoleOut
    {
      get;
      set;
    }

    public ConsoleStreams Streams
    {
      get;
      private set;
    }

    StringWriter ConsoleOut
    {
      get;
      set;
    }

    StringWriter ConsoleError
    {
      get;
      set;
    }

    TextWriter DefaultConsoleError
    {
      get;
      set;
    }

    #region IDisposable Members
    public void Dispose()
    {
      Streams.Out = RestoreConsoleOut(DefaultConsoleOut, ConsoleOut);
      Streams.Error = RestoreConsoleError(DefaultConsoleError, ConsoleError);
    }
    #endregion

    static TextWriter RedirectConsoleOutTo(TextWriter writer)
    {
      if (writer == null)
      {
        throw new ArgumentNullException("writer");
      }

      var defaultWriter = Console.Out;

      Console.SetOut(writer);

      return defaultWriter;
    }

    static TextWriter RedirectConsoleErrorTo(TextWriter writer)
    {
      if (writer == null)
      {
        throw new ArgumentNullException("writer");
      }

      var defaultWriter = Console.Error;

      Console.SetError(writer);

      return defaultWriter;
    }

    static string RestoreConsoleOut(TextWriter defaultWriter, TextWriter capture)
    {
      if (defaultWriter == null)
      {
        throw new ArgumentNullException("defaultWriter");
      }

      if (capture == null)
      {
        throw new ArgumentNullException("capture");
      }

      Console.SetOut(defaultWriter);

      try
      {
        capture.Flush();

        return capture.ToString();
      }
      finally
      {
        capture.Dispose();
      }
    }

    static string RestoreConsoleError(TextWriter defaultWriter, TextWriter capture)
    {
      if (defaultWriter == null)
      {
        throw new ArgumentNullException("defaultWriter");
      }

      if (capture == null)
      {
        throw new ArgumentNullException("capture");
      }

      Console.SetError(defaultWriter);

      try
      {
        capture.Flush();

        return capture.ToString();
      }
      finally
      {
        capture.Dispose();
      }
    }

    internal static ConsoleRedirection RedirectConsoleStreams()
    {
      ConsoleRedirection result = new ConsoleRedirection();

      result.DefaultConsoleOut = RedirectConsoleOutTo(result.ConsoleOut);
      result.DefaultConsoleError = RedirectConsoleErrorTo(result.ConsoleError);

      return result;
    }
  }
}