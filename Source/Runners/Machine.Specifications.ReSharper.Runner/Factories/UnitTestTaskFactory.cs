using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  class UnitTestTaskFactory
  {
    readonly string _providerId;

    public UnitTestTaskFactory(string providerId)
    {
      _providerId = providerId;
    }

    public UnitTestTask CreateRunAssemblyTask(ContextElement context)
    {
      return new UnitTestTask(null,
                              new RunAssemblyTask(_providerId, context.AssemblyLocation));
    }

    public UnitTestTask CreateContextTask(ContextElement context)
    {
      return new UnitTestTask(context,
                              new ContextTask(_providerId,
                                              context.AssemblyLocation,
                                              context.GetTypeClrName().FullName));
    }

    public UnitTestTask CreateContextSpecificationTask(ContextElement context,
                                                       ContextSpecificationElement contextSpecification)
    {
      return new UnitTestTask(contextSpecification,
                              new ContextSpecificationTask(_providerId,
                                                           context.AssemblyLocation,
                                                           context.GetTypeClrName().FullName,
                                                           contextSpecification.FieldName));
    }

    public UnitTestTask CreateBehaviorSpecificationTask(ContextElement context,
                                                        BehaviorSpecificationElement behaviorSpecification)
    {
      return new UnitTestTask(behaviorSpecification,
                              new BehaviorSpecificationTask(_providerId,
                                                            context.AssemblyLocation,
                                                            context.GetTypeClrName().FullName,
                                                            behaviorSpecification.Behavior.FieldName,
                                                            behaviorSpecification.FieldName,
                                                            behaviorSpecification.Behavior.FieldType));
    }
  }
}