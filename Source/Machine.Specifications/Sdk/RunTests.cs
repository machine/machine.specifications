using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Machine.Specifications.Runner;
using Machine.Specifications.Runner.Impl;

namespace Machine.Specifications.Sdk
{
  public class RunSpecs : RemoteToInternalSpecificationRunListenerAdapter
  {
    public RunSpecs(object listener, IEnumerable<string> contextList, Assembly testAssembly)
      : base(listener)
    {
      var runner = new AppDomainRunner(this, RunOptions.Default);
      runner.StartRun(testAssembly);

      foreach (var contextName in contextList)
      {
        var contextClass = testAssembly.GetType(contextName);
        runner.RunMember(testAssembly, contextClass);
      }

      runner.EndRun(testAssembly);
    }

    public RunSpecs(object listener, IEnumerable<string> includeTags, IEnumerable<string> excludeTags, IEnumerable<string> filters, Assembly assembly)
      : base(listener)
    {
      var runOptions = new RunOptions(includeTags, excludeTags, filters);
      var runner = new AppDomainRunner(this, runOptions);
      runner.RunAssembly(assembly);
    }

    public RunSpecs(object listener, IEnumerable<string> includeTags, IEnumerable<string> excludeTags, IEnumerable<string> filters, IEnumerable<Assembly> assemblies)
      : base(listener)
    {
      var runOptions = new RunOptions(includeTags, excludeTags, filters);
      var runner = new AppDomainRunner(this, runOptions);
      runner.RunAssemblies(assemblies);
    }
  }

}