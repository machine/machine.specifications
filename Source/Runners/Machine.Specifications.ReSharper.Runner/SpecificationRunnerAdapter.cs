using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Machine.Specifications.ReSharper.Runner
{
    public class SpecificationRunnerAdapter : IReSharperSpecificationRunner
    {
        readonly object _mspecRunner;
        readonly object _runOptions;
        readonly Action<object, Assembly> _startRun = (_, __) => { };
        readonly Action<object, object, Assembly, Type> _runMember;
        readonly Action<object, Assembly> _endRun = (_, __) => { };

        public SpecificationRunnerAdapter(object mspecRunner, object runOptions)
        {
            _mspecRunner = mspecRunner;
            _runOptions = runOptions;

            var runnerParameter = Expression.Parameter(typeof(object), "runner");
            var contextAssemblyParameter = Expression.Parameter(typeof(Assembly), "contextAssembly");

            var runnerInstance = Expression.Convert(runnerParameter, mspecRunner.GetType());

            _runMember = CompileRunMemberAction(mspecRunner, runOptions, runnerInstance, runnerParameter, contextAssemblyParameter);

            // StartRun and EndRun added 19/07/2013 (10dcac2f39f000a14326ebfe8e6cf699c43b9e82)
            var methodInfo = mspecRunner.GetType().GetMethod("StartRun");
            if (methodInfo != null)
            {
                var methodCallExpression2 = Expression.Call(runnerInstance, methodInfo, contextAssemblyParameter);
                _startRun = Expression.Lambda<Action<object, Assembly>>(methodCallExpression2, runnerParameter, contextAssemblyParameter).Compile();
            }

            methodInfo = mspecRunner.GetType().GetMethod("EndRun");
            if (methodInfo != null)
            {
                var methodCallExpression2 = Expression.Call(runnerInstance, methodInfo, contextAssemblyParameter);
                _endRun = Expression.Lambda<Action<object, Assembly>>(methodCallExpression2, runnerParameter, contextAssemblyParameter).Compile();
            }
        }

        Action<object, object, Assembly, Type> CompileRunMemberAction(object mspecRunner, object runOptions,
            Expression runnerInstance,
            ParameterExpression runnerParameter,
            ParameterExpression contextAssemblyParameter)
        {
            // Create an action to call AppDomainRunner.RunMember. Pass in the implicit
            // "this" object of the mspecRunner (late bound). Also pass in the late bound
            // runOptions object, and the early bound context assembly and context class
            // The generated lambda casts the late bound objects to their proper types,
            // then calls RunMember on the runner instance, passing in the required parameters
            var methodInfo = mspecRunner.GetType().GetMethod("RunMember");

            var runOptionsParameter = Expression.Parameter(typeof(object), "runOptions");
            var contextClassParameter = Expression.Parameter(typeof(Type), "contextClass");

            // AppDomainRunner.RunMember removed RunOptions argument 29/09/2008 (218cb8233058e1327ca612b79786289354a0b50b)
            // TODO: Was added the commit before! Might not have made it to public API
            // TODO: RunMember was only added in 27/09/2008 (15efd28df8b72e6dfda9e9cc29fb1c36cae20d0d) so can't support anything before then!
            // nuget.org goes back to 0.3.0 which is the 26/01/2011
            Expression[] arguments;
            if (methodInfo.GetParameters().Any(pi => pi.ParameterType.Name == "RunOptions"))
            {
                var runOptionsInstance = Expression.Convert(runOptionsParameter, runOptions.GetType());
                arguments = new Expression[] { contextAssemblyParameter, contextClassParameter, runOptionsInstance };
            }
            else
            {
                arguments = new Expression[] {contextAssemblyParameter, contextClassParameter};
            }

            var methodCallExpression = Expression.Call(runnerInstance, methodInfo, arguments);

            return Expression.Lambda<Action<object, object, Assembly, Type>>(methodCallExpression,
                runnerParameter,
                runOptionsParameter,
                contextAssemblyParameter,
                contextClassParameter).Compile();
        }

        public void StartRun(Assembly contextAssembly)
        {
            _startRun(_mspecRunner, contextAssembly);
        }

        public void RunMember(Assembly contextAssembly, Type contextClass)
        {
            _runMember(_mspecRunner, _runOptions, contextAssembly, contextClass);
        }

        public void EndRun(Assembly contextAssembly)
        {
            _endRun(_mspecRunner, contextAssembly);
        }
    }
}