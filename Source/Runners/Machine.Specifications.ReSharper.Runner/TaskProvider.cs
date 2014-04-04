namespace Machine.Specifications.ReSharperRunner
{
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.ReSharper.TaskRunnerFramework;
    using JetBrains.Util;

    using Machine.Specifications.ReSharperRunner.Tasks;

    public class TaskProvider
    {
        private readonly IDictionary<string, ContextTask> _contextTasks = new Dictionary<string, ContextTask>();
        //private readonly IDictionary<string, IList<ContextSpecificationTask>> contextSpecTasks = new Dictionary<string, IList<ContextSpecificationTask>>();
        //private readonly IDictionary<string, IList<BehaviorSpecificationTask>> contextBehaviorTasks = new Dictionary<string, IList<BehaviorSpecificationTask>>();
        private readonly IDictionary<string, IList<Task>> _specTasks = new Dictionary<string, IList<Task>>();
        private readonly RemoteTaskServer _server;

        private TaskProvider(RemoteTaskServer server)
        {
            this._server = server;
        }

        public IEnumerable<string> ContextNames { get { return this._contextTasks.Keys; } }
        private IEnumerable<string> SpecificationTypeNames { get { return this._specTasks.Keys; } }

        public static TaskProvider Create(RemoteTaskServer server, TaskExecutionNode assemblyNode)
        {
            var taskProvider = new TaskProvider(server);

            foreach (var contextNode in assemblyNode.Children)
            {
                var contextTask = (ContextTask)contextNode.RemoteTask;
                var behaviorTypeNames = taskProvider.GetBehaviorTypeNames(contextNode);
                taskProvider.AddContext(contextTask, behaviorTypeNames);

                foreach (var specNode in contextNode.Children)
                {
                    var specTask = (Task)specNode.RemoteTask;
                    taskProvider.AddSpecificationTask(specTask);
                    //foreach (var theoryNode in methodNode.Children)
                    //  taskProvider.AddTheory(methodTask, (XunitTestTheoryTask)theoryNode.RemoteTask);
                }
            }

            return taskProvider;
        }

        public ContextTask GetContextTask(string type)
        {
            return this._contextTasks[type];
        }

        public RemoteTask GetSpecificationTask(string type, string fieldName)
        {
            Task specTask = null;

            if (this._specTasks.ContainsKey(type))
            {
                var specList = this._specTasks[type];
                specTask = specList.FirstOrDefault(m => m.SpecificationFieldName == fieldName);
            }

            return specTask;
        }

        private IEnumerable<string> GetBehaviorTypeNames(TaskExecutionNode node)
        {
            var nodes = node.Children.Where(x => x.RemoteTask is BehaviorSpecificationTask);
            return nodes.Select(n => (BehaviorSpecificationTask)n.RemoteTask).Select(n => n.BehaviorTypeName);
        }

        private void AddContext(ContextTask contextTask, IEnumerable<string> behaviorTypeNames)
        {
            this._contextTasks.Add(contextTask.ContextTypeName, contextTask);

            this._specTasks.Add(contextTask.ContextTypeName, new List<Task>());

            foreach (var name in behaviorTypeNames.Where(name => !this.SpecificationTypeNames.Contains(name)))
            {
                this._specTasks.Add(name, new List<Task>());
            }
        }

        private void AddSpecificationTask(Task specTask)
        {
            if (!specTask.BehaviorTypeName.IsEmpty())
            {
                this._specTasks[specTask.BehaviorTypeName].Add(specTask);
            }
            else
            {
                this._specTasks[specTask.ContextTypeName].Add(specTask);
            }
        }
    }
}