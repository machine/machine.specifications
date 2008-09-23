using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Runner
{
  public class AppDomainRunner : ISpecificationRunner
  {
    readonly ISpecificationRunListener listener;
    readonly SpecificationRunner runner;

    public AppDomainRunner(ISpecificationRunListener listener)
    {
      this.listener = listener;
      runner = new SpecificationRunner(new RemoteRunListener(listener));
    }

    public void RunAssembly(Assembly assembly)
    {
      runner.RunAssembly(assembly);
    }

    public void RunContexts(IEnumerable<Context> contexts)
    {
      throw new System.NotImplementedException();
    }

    public void RunNamespace(Assembly assembly, string targetNamespace)
    {
      throw new System.NotImplementedException();
    }

    public void RunMember(Assembly assembly, MemberInfo member)
    {
      throw new System.NotImplementedException();
    }
  }
}
