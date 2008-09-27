using System.Collections.Generic;
using System.Reflection;

namespace Machine.Specifications.Runner
{
  public interface ISpecificationRunner
  {
    void RunAssembly(Assembly assembly, RunOptions options);
    void RunAssemblies(IEnumerable<Assembly> assemblies, RunOptions options);
    void RunNamespace(Assembly assembly, string targetNamespace, RunOptions options);
    void RunMember(Assembly assembly, MemberInfo member, RunOptions options);
  }
}