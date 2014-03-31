using Machine.Specifications.Runner;

namespace Machine.Specifications.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class RemoteToInternalSpecificationRunListenerAdapter : ISpecificationRunListener
    {
        private static readonly object locker = new object();

        private static Type runnerType;

        private static readonly Dictionary<string, Delegate> methodCache = new Dictionary<string, Delegate>();

        private readonly object _listener;

        protected RemoteToInternalSpecificationRunListenerAdapter(object listener)
        {
            _listener = listener;
        }

        public void OnAssemblyStart(AssemblyInfo assemblyInfo)
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnAssemblyStart");

            methodToInvoke.DynamicInvoke(this._listener, assemblyInfo.ToXml());
        }

        public void OnAssemblyEnd(AssemblyInfo assemblyInfo)
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnAssemblyEnd");

            methodToInvoke.DynamicInvoke(this._listener, assemblyInfo.ToXml());
        }

        public void OnRunStart()
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnRunStart");

            methodToInvoke.DynamicInvoke(this._listener);
        }

        public void OnRunEnd()
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnRunEnd");

            methodToInvoke.DynamicInvoke(this._listener);
        }

        public void OnContextStart(ContextInfo context)
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnContextStart");

            methodToInvoke.DynamicInvoke(this._listener, context.ToXml());
        }

        public void OnContextEnd(ContextInfo context)
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnContextEnd");

            methodToInvoke.DynamicInvoke(this._listener, context.ToXml());
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnSpecificationStart");

            methodToInvoke.DynamicInvoke(this._listener, specification.ToXml());
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnSpecificationEnd");

            methodToInvoke.DynamicInvoke(this._listener, specification.ToXml(), result.ToXml());
        }

        public void OnFatalError(ExceptionResult exception)
        {
            var methodToInvoke = this.GetOrCacheDelegate("OnFatalError");

            methodToInvoke.DynamicInvoke(this._listener, exception.ToXml());
        }

        private Delegate GetOrCacheDelegate(string methodName)
        {
            Delegate methodToInvoke;
            if (!methodCache.TryGetValue(methodName, out methodToInvoke))
            {
                if (runnerType == null)
                {
                    runnerType = this._listener.GetType().GetInterfaces().Single(t => t.Name == "IRemoteSpecificationRunListener");
                }

                MethodInfo method = runnerType.GetMethod(methodName);

                int parameterCount = 0;
                var instanceParameter = Expression.Parameter(runnerType, "instance");
                var parameterExpressions = new List<ParameterExpression> { instanceParameter };
                var callExpressions = new List<Expression>();
                foreach (var parameter in method.GetParameters())
                {
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(string), string.Format("arg{0}", parameterCount++));
                    parameterExpressions.Add(parameterExpression);
                    callExpressions.Add(Expression.Convert(parameterExpression, parameter.ParameterType));
                }
                var methodCall = Expression.Call(instanceParameter, method, callExpressions);
                var action = Expression.Lambda(methodCall, parameterExpressions.ToArray()).Compile();

                methodToInvoke = action;

                lock (locker)
                {
                    methodCache[methodName] = action;
                }
            }
            return methodToInvoke;
        }
    }
}