namespace Machine.Specifications.Runner.Utility
{
    public class ReflectionRunListener : Runner.ISpecificationRunListener
    {
        private readonly ISpecificationRunListener listener;

        public ReflectionRunListener(ISpecificationRunListener listener)
        {
            this.listener = listener;
        }

        public void OnAssemblyStart(Runner.AssemblyInfo assembly)
        {
            listener.OnAssemblyStart(new AssemblyInfo(assembly.Name, assembly.Location));
        }

        public void OnAssemblyEnd(Runner.AssemblyInfo assembly)
        {
            listener.OnAssemblyEnd(new AssemblyInfo(assembly.Name, assembly.Location));
        }

        public void OnRunStart()
        {
            listener.OnRunStart();
        }

        public void OnRunEnd()
        {
            listener.OnRunEnd();
        }

        public void OnContextStart(Runner.ContextInfo context)
        {
            listener.OnContextStart(new ContextInfo(context.Name, context.Concern, context.TypeName, context.Namespace, context.AssemblyName));
        }

        public void OnContextEnd(Runner.ContextInfo context)
        {
            listener.OnContextEnd(new ContextInfo(context.Name, context.Concern, context.TypeName, context.Namespace, context.AssemblyName));
        }

        public void OnSpecificationStart(Runner.SpecificationInfo specification)
        {
            listener.OnSpecificationStart(new SpecificationInfo(specification.Leader, specification.Name, specification.ContainingType, specification.FieldName));
        }

        public void OnSpecificationEnd(Runner.SpecificationInfo specification, Specifications.Result result)
        {
            Result utilityResult;

            if (result.Status == Specifications.Status.Ignored)
            {
                utilityResult = Result.Ignored();
            }
            else if (result.Status == Specifications.Status.NotImplemented)
            {
                utilityResult = Result.NotImplemented();
            }
            else if (result.Status == Specifications.Status.Failing)
            {
                utilityResult = Result.Failure(GetExceptionResult(result.Exception));
            }
            else
            {
                utilityResult = Result.Pass();
            }

            foreach (var key in result.Supplements.Keys)
            {
                utilityResult.Supplements[key] = result.Supplements[key];
            }

            listener.OnSpecificationEnd(new SpecificationInfo(specification.Leader, specification.Name, specification.ContainingType, specification.FieldName), utilityResult);
        }

        public void OnFatalError(Specifications.ExceptionResult exception)
        {
            listener.OnFatalError(GetExceptionResult(exception));
        }

        private ExceptionResult GetExceptionResult(Specifications.ExceptionResult result)
        {
            if (result == null)
            {
                return null;
            }

            return new ExceptionResult(result.FullTypeName, result.TypeName, result.Message, result.StackTrace, GetExceptionResult(result.InnerExceptionResult));
        }
    }
}
