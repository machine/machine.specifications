using System;

namespace Machine.Specifications
{
    public class PrerequisiteNotMetException : Exception
    {
        readonly Exception _innerException;

        public PrerequisiteNotMetException()
        {
        }

        public PrerequisiteNotMetException(string message) : base(message)
        {

        }

        public PrerequisiteNotMetException(string message, Exception innerException) : base(message)
        {
            this._innerException = innerException;
        }
       
        public override string StackTrace => this._innerException?.StackTrace;
    }
}