using System;
using System.Linq;
using System.Text;

using JetBrains.Metadata.Reader.API;

namespace Machine.Specifications.ReSharperRunner
{
  internal static partial class MetadataExtensions
  {
    public static string FullyQualifiedName(this IMetadataClassType classType)
    {
      return FullyQualifiedName(classType, false);
    }

    static string FullyQualifiedName(this IMetadataClassType classType, bool appendAssembly)
    {
      var fullyQualifiedName = new StringBuilder();

      fullyQualifiedName.Append(classType.Type.FullyQualifiedName);

      if (classType.Arguments.Length > 0)
      {
        fullyQualifiedName.Append("[");
        fullyQualifiedName.Append(
          String.Join(",",
                      classType.Arguments
                        .Select(x => x as IMetadataClassType)
                        .Where(x => x != null)
                        .Select(x => "[" + x.FullyQualifiedName(true) + "]")
                        .ToArray()));
        fullyQualifiedName.Append("]");
      }

      if (appendAssembly)
      {
        fullyQualifiedName.Append(classType.AssemblyQualification);
      }

      return fullyQualifiedName.ToString();
    }
  }
}