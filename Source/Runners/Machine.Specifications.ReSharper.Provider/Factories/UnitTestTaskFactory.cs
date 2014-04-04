namespace Machine.Specifications.ReSharperProvider.Factories
{
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Presentation;
    using Machine.Specifications.ReSharperRunner.Tasks;

    class UnitTestTaskFactory
    {
        readonly string _providerId;

        public UnitTestTaskFactory(string providerId)
        {
            this._providerId = providerId;
        }

        public UnitTestTask CreateRunAssemblyTask(ContextElement context)
        {
            return new UnitTestTask(null,
                                    new RunAssemblyTask(this._providerId, context.AssemblyLocation));
        }

        public UnitTestTask CreateContextTask(ContextElement context)
        {
            return new UnitTestTask(context,
                                    new ContextTask(this._providerId,
                                                    context.AssemblyLocation,
                                                    context.GetTypeClrName().FullName));
        }

        public UnitTestTask CreateContextSpecificationTask(ContextElement context,
                                                           ContextSpecificationElement contextSpecification)
        {
            return new UnitTestTask(contextSpecification,
                                    new ContextSpecificationTask(this._providerId,
                                                                 context.AssemblyLocation,
                                                                 context.GetTypeClrName().FullName,
                                                                 contextSpecification.FieldName));
        }

        public UnitTestTask CreateBehaviorSpecificationTask(ContextElement context,
                                                            BehaviorSpecificationElement behaviorSpecification)
        {
            return new UnitTestTask(behaviorSpecification,
                                    new BehaviorSpecificationTask(this._providerId,
                                                                  context.AssemblyLocation,
                                                                  context.GetTypeClrName().FullName,
                                                                  behaviorSpecification.Behavior.FieldName,
                                                                  behaviorSpecification.FieldName,
                                                                  behaviorSpecification.Behavior.FieldType));
        }
    }
}