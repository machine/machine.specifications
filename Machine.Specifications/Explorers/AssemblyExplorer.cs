using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Explorers
{
  public class AssemblyExplorer
  {
    public IEnumerable<Specification> FindSpecifications(Assembly assembly)
    {
      foreach (Type type in assembly.GetExportedTypes())
      {
        if (type.IsDefined(typeof(SpecificationAttribute), false))
        {
          yield return CreateSpecificationFrom(type);
        }
      }
    }

    private Specification CreateSpecificationFrom(Type type)
    {
      FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
      string whenClause = "";
      List<FieldInfo> itFieldInfos = new List<FieldInfo>();

      foreach (FieldInfo info in fieldInfos)
      {
        if (info.FieldType == typeof (When))
        {
          whenClause = info.Name.ReplaceUnderscores();
        }
        if (info.FieldType == typeof(It))
        {
          itFieldInfos.Add(info);
        }
      }

      var requirements = itFieldInfos.Select(x => new Requirement() {ItClause = x.Name.ReplaceUnderscores()});

      var specification = new Specification(requirements) { 
            Name = type.Name.ReplaceUnderscores(),
            WhenClause = whenClause
          };

      return specification;
    }
  }
}
