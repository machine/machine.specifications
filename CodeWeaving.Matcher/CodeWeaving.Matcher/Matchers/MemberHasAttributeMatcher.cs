using System;
using Mono.Cecil;

namespace CodeWeaving.Matcher.Matchers
{
  public class MemberHasAttributeMatcher : IMemberMatcher
  {
    #region Member Data
    private readonly string _attributeName;
    #endregion

    #region MemberHasAttributeMatcher()
    public MemberHasAttributeMatcher(string attributeName)
    {
      _attributeName = attributeName;
    }
    #endregion

    #region IMemberMatcher Members
    public bool Includes(MemberReference memberReference)
    {
      ICustomAttributeProvider customAttributeProvider = memberReference as ICustomAttributeProvider;
      if (customAttributeProvider == null)
      {
        return false;
      }
      foreach (CustomAttribute attribute in customAttributeProvider.CustomAttributes)
      {
        if (attribute.Constructor.DeclaringType.FullName == _attributeName)
        {
          return true;
        }
      }
      return false;
    }
    #endregion
  }
}