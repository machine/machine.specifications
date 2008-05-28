using System.Collections.Generic;
using System.Reflection;

namespace Machine.Specifications.Runner
{
  public interface ISpecificationRunner
  {
    void RunAssembly(Assembly assembly);
    void RunContexts(IEnumerable<Model.Context> contexts);
    void RunNamespace(Assembly assembly, string targetNamespace);
    void RunMember(Assembly assembly, MemberInfo member);
  }
}