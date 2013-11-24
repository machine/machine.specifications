using System;
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
            var mspecAppDomainRunner = GetAppDomainRunner();
            return new SpecificationRunnerAdapter(mspecAppDomainRunner);
        }

        object GetAppDomainRunner()
        {
            object listener = GetPerAssemblyRunListener();
            object runOptions = GetDefaultRunOptions();
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
            var appDomainRunnerType = _mspecAssembly.GetType("Machine.Specifications.Runner.Impl.AppDomainRunner");
            return Activator.CreateInstance(appDomainRunnerType, new[] { listener, runOptions });
        }

        object GetDefaultRunOptions()
        {
            var runOptionsType = _mspecAssembly.GetType("Machine.Specifications.Runner.RunOptions");
            var propertyInfo = runOptionsType.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
            return propertyInfo.GetValue(null, null);
        }
    }
}