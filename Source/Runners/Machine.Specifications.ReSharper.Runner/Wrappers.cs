using System;

// TODO: Sort out namespace - this is just to keep other code compiling in all projects
using Machine.Specifications.ReSharper.Runner;

// ReSharper disable once EmptyNamespace
namespace Machine.Specifications.Runner
{
}

namespace Machine.Specifications
{
    public class AssemblyInfo
    {
        static readonly Accessor<string> GetLocationAccessor = new Accessor<string>("Location"); 

        readonly Func<string> _getLocation;

        public AssemblyInfo(object inner)
        {
            _getLocation = GetLocationAccessor.MemoizeGetter(inner);
        }

        public string Location { get { return _getLocation(); } }
    }

    public class ContextInfo
    {
        static readonly Accessor<string> GetTypeNameAccessor = new Accessor<string>("TypeName");

        readonly Func<string> _getTypeName;

        public ContextInfo(object inner)
        {
            _getTypeName = GetTypeNameAccessor.MemoizeGetter(inner);
        }

        public string TypeName { get { return _getTypeName(); } }
    }

    public class ExceptionResult
    {
        static readonly Accessor<string> GetFullTypeNameAccessor = new Accessor<string>("FullTypeName");
        static readonly Accessor<string> GetMessageAccessor = new Accessor<string>("Message");
        static readonly Accessor<string> GetStackTraceAccessor = new Accessor<string>("StackTrace");
        static readonly Accessor<object> GetInnerExceptionResultAccessor = new Accessor<object>("InnerExceptionResult"); 

        readonly Func<string> _getFullTypeName;
        readonly Func<string> _getMessage;
        readonly Func<string> _getStackTrace;
        readonly Func<object> _getInnerExceptionResult;


        public ExceptionResult(object inner)
        {
            _getFullTypeName = GetFullTypeNameAccessor.MemoizeGetter(inner);
            _getMessage = GetMessageAccessor.MemoizeGetter(inner);
            _getStackTrace = GetStackTraceAccessor.MemoizeGetter(inner);
            _getInnerExceptionResult = GetInnerExceptionResultAccessor.MemoizeGetter(inner);
        }

        public string FullTypeName { get { return _getFullTypeName(); }}
        public string Message { get { return _getMessage(); } }
        public string StackTrace { get { return _getStackTrace(); } }

        public ExceptionResult InnerExceptionResult
        {
            get
            {
                var result = _getInnerExceptionResult();
                return result != null ? new ExceptionResult(result) : null;
            }
        }
    }

    public class Result
    {
        // TODO: Ugh enum. Might be nicer to do strings
        static readonly Accessor<Status> GetStatusAccessor = new EnumAccessor<Status>("Status");
        static readonly Accessor<object> GetExceptionAccessor = new Accessor<object>("Exception");

        readonly Func<Status> _getStatus;
        readonly Func<object> _getException;

        public Result(object inner)
        {
            _getStatus = GetStatusAccessor.MemoizeGetter(inner);
            _getException = GetExceptionAccessor.MemoizeGetter(inner);
        }

        public Status Status { get { return _getStatus(); } }

        public ExceptionResult Exception
        {
            get
            {
                var exception = _getException();
                return exception != null ? new ExceptionResult(exception) : null;
            }
        }
    }

    public class SpecificationInfo
    {
        static readonly Accessor<string> GetContainingTypeAccessor = new Accessor<string>("ContainingType");
        static readonly Accessor<string> GetFieldNameAccessor = new Accessor<string>("FieldName");

        readonly Func<string> _getContainingType; 
        readonly Func<string> _getFieldName; 

        public SpecificationInfo(object inner)
        {
            _getContainingType = GetContainingTypeAccessor.MemoizeGetter(inner);
            _getFieldName = GetFieldNameAccessor.MemoizeGetter(inner);
        }

        public string ContainingType { get { return _getContainingType(); } }
        public string FieldName { get { return _getFieldName(); }}
    }

    public enum Status
    {
        Failing,
        Passing,
        NotImplemented,
        Ignored
    }
}