using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Machine.Specifications.ReSharper.Runner
{
    public class SpecificationRunnerAdapter : IReSharperSpecificationRunner
    {
        readonly object _mspecRunner;
        readonly Action<object, Assembly> _startRun;
        readonly Action<object, Assembly, Type> _runMember;
        readonly Action<object, Assembly> _endRun;

        public SpecificationRunnerAdapter(object mspecRunner)
        {
            _mspecRunner = mspecRunner;
            var methodInfo = mspecRunner.GetType().GetMethod("RunMember");
            var runnerParameter = Expression.Parameter(typeof(object), "runner");
            var runnerInstance = Expression.Convert(runnerParameter, mspecRunner.GetType());
            var contextAssemblyParameter = Expression.Parameter(typeof(Assembly), "contextAssembly");
            var contextClassParameter = Expression.Parameter(typeof(Type), "contextClass");
            var methodCallExpression = Expression.Call(runnerInstance, methodInfo, contextAssemblyParameter, contextClassParameter);
            _runMember = Expression.Lambda<Action<object, Assembly, Type>>(methodCallExpression, runnerParameter,
                contextAssemblyParameter,
                contextClassParameter).Compile();

            methodInfo = mspecRunner.GetType().GetMethod("StartRun");
            methodCallExpression = Expression.Call(runnerInstance, methodInfo, contextAssemblyParameter);
            _startRun = Expression.Lambda<Action<object, Assembly>>(methodCallExpression, runnerParameter, contextAssemblyParameter).Compile();

            methodInfo = mspecRunner.GetType().GetMethod("EndRun");
            methodCallExpression = Expression.Call(runnerInstance, methodInfo, contextAssemblyParameter);
            _endRun = Expression.Lambda<Action<object, Assembly>>(methodCallExpression, runnerParameter, contextAssemblyParameter).Compile();
        }

        public void StartRun(Assembly contextAssembly)
        {
            _startRun(_mspecRunner, contextAssembly);
        }

        public void RunMember(Assembly contextAssembly, Type contextClass)
        {
            _runMember(_mspecRunner, contextAssembly, contextClass);
        }

        public void EndRun(Assembly contextAssembly)
        {
            _endRun(_mspecRunner, contextAssembly);
        }
    }
}