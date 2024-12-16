using System;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public interface IFrameworkLogger
    {
        void SendErrorMessage(string message, Exception exception);
    }
}
