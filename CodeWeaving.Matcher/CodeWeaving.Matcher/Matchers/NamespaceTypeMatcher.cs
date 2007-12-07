using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace CodeWeaving.Matcher.Matchers
{
  public class NamespaceTypeMatcher : ITypeMatcher
  {
    #region Member Data
    private readonly string _namespace;
    #endregion

    #region NamespaceTypeMatcher()
    public NamespaceTypeMatcher(string ns)
    {
      _namespace = ns;
    }
    #endregion

    #region ITypeMatcher Members
    public bool Includes(TypeDefinition type)
    {
      return String.Equals(type.Namespace, _namespace, StringComparison.InvariantCultureIgnoreCase);
    }
    #endregion
  }
}
