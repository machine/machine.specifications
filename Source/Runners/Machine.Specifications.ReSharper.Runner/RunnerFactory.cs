using System;
using System.Linq;
using System.Reflection;

using Castle.DynamicProxy;

using JetBrains.ReSharper.TaskRunnerFramework;

using Machine.Specifications.ReSharperRunner.Runners.Notifications;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharper.Runner
{
    class RunnerFactory
    {
        static readonly RemoteTaskNotificationFactory TaskNotificationFactory = new RemoteTaskNotificationFactory();

        readonly IRemoteTaskServer _server;
        readonly Assembly _mspecAssembly;
        readonly TaskExecutionNode _node;

        public RunnerFactory(IRemoteTaskServer server, Assembly mspecAssembly, TaskExecutionNode node)
        {
            _server = server;
            _mspecAssembly = mspecAssembly;
            _node = node;
        }

        public IReSharperSpecificationRunner CreateRunner()
        {
            object runOptions = GetDefaultRunOptions();
            object mspecAppDomainRunner = GetAppDomainRunner(runOptions);
            return new SpecificationRunnerAdapter(mspecAppDomainRunner, runOptions);
        }

        object GetAppDomainRunner(object runOptions)
        {
            object listener = GetPerAssemblyRunListener();
            return GetAppDomainRunner(listener, runOptions);
        }

        object GetPerAssemblyRunListener()
        {
            var interceptor = CreateListenerInterceptor();

            var listenerType = _mspecAssembly.GetType("Machine.Specifications.Runner.ISpecificationRunListener");
            var generator = new ProxyGenerator();
            return generator.CreateInterfaceProxyWithoutTarget(listenerType, interceptor);
        }

        PerAssemblyRunListenerInterceptor CreateListenerInterceptor()
        {
            var interceptor = new PerAssemblyRunListenerInterceptor(_server, (RunAssemblyTask) _node.RemoteTask);
            foreach (var child in _node.Flatten(x => x.Children))
            {
                interceptor.RegisterTaskNotification(TaskNotificationFactory.CreateTaskNotification(child));
            }
            return interceptor;
        }

        object GetAppDomainRunner(object listener, object runOptions)
        {
            // AppDomainRunner moved namespace 09/10/2008 (4d716e9dc2574423264d7a38b3609837deba854b) 
            var appDomainRunnerType = _mspecAssembly.GetType("Machine.Specifications.Runner.Impl.AppDomainRunner");
            if (appDomainRunnerType == null)
                appDomainRunnerType = _mspecAssembly.GetType("Machine.Specifications.Runner.AppDomainRunner");

            // AppDomainRunner .ctor added RunOptions 29/09/2008 (218cb8233058e1327ca612b79786289354a0b50b)
            var args = appDomainRunnerType.GetConstructors().Any(HasRunOptionsParameter) ? new[] {listener, runOptions} : new[] {listener};
            return Activator.CreateInstance(appDomainRunnerType, args);
        }

        static bool HasRunOptionsParameter(MethodBase methodInfo)
        {
            return methodInfo.GetParameters().Any(pi => pi.ParameterType.Name == "RunOptions");
        }

        object GetDefaultRunOptions()
        {
            // RunOptions was added 27/09/2008 (3c83f099a11d9acfbf1e9a95aee7b1a053658b24)
            var runOptionsType = _mspecAssembly.GetType("Machine.Specifications.Runner.RunOptions");
            var propertyInfo = runOptionsType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
            return propertyInfo.GetValue(null, null);
        }
    }
}