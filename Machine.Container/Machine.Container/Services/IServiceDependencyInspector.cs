using System;

using Machine.Container.Model;

namespace Machine.Container.Services
{
  public interface IServiceDependencyInspector
  {
    ConstructorCandidate SelectConstructor(Type type);
  }
}
