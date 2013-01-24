using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Tasks;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class UnitTestTaskFactory
  {
    readonly string _providerId;

    public UnitTestTaskFactory(string providerId)
    {
      _providerId = providerId;
    }

    public UnitTestTask CreateAssemblyLoadTask(ContextElement context)
    {
      return new UnitTestTask(null,
                              new AssemblyLoadTask(context.AssemblyLocation));
    }

    public UnitTestTask CreateContextTask(ContextElement context, bool isExplicit)
    {
      return new UnitTestTask(context,
                              new ContextTask(_providerId,
                                              context.AssemblyLocation,
                                              context.GetTypeClrName().FullName,
                                              false));
    }

    public UnitTestTask CreateContextSpecificationTask(ContextElement context,
                                                       ContextSpecificationElement contextSpecification,
                                                       bool isExplicit)
    {
      return new UnitTestTask(contextSpecification,
                              new ContextSpecificationTask(_providerId,
                                                           context.AssemblyLocation,
                                                           context.GetTypeClrName().FullName,
                                                           contextSpecification.FieldName,
                                                           false));
    }

    public UnitTestTask CreateBehaviorTask(ContextElement context, BehaviorElement behavior, bool isExplicit)
    {
      return new UnitTestTask(behavior,
                              new BehaviorTask(_providerId,
                                               context.AssemblyLocation,
                                               context.GetTypeClrName().FullName,
                                               behavior.FullyQualifiedTypeName,
                                               behavior.FieldName,
                                               false));
    }

    public UnitTestTask CreateBehaviorSpecificationTask(ContextElement context,
                                                        BehaviorSpecificationElement behaviorSpecification,
                                                        bool isExplicit)
    {
      return new UnitTestTask(behaviorSpecification,
                              new BehaviorSpecificationTask(_providerId,
                                                            context.AssemblyLocation,
                                                            context.GetTypeClrName().FullName,
                                                            behaviorSpecification.Behavior.FieldName,
                                                            behaviorSpecification.FieldName,
                                                            behaviorSpecification.GetTypeClrName().FullName,
                                                            false));
    }
  }
}