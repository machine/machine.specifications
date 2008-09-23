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
    public void RunAssembly(Assembly assembly)
    {
      throw new System.NotImplementedException();
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
