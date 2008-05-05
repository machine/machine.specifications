using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;

namespace Machine.Specifications.Explorers
{
  public class AssemblyExplorer
  {
    private SpecificationFactory _specificationFactory;

    public AssemblyExplorer()
    {
      _specificationFactory = new SpecificationFactory();
    }

    public IEnumerable<Specification> FindSpecificationsIn(Assembly assembly)
    {
      foreach (Type type in assembly.GetExportedTypes())
      {
        if (type.IsDefined(typeof(DescriptionAttribute), false))
        {
          object instance = Activator.CreateInstance(type);
          yield return _specificationFactory.CreateSpecificationFrom(instance);
        }
      }
    }
  }
}
