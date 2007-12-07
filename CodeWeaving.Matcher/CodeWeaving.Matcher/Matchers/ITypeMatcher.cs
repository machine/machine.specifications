using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace CodeWeaving.Matcher.Matchers
{
  public interface ITypeMatcher
  {
    bool Includes(TypeDefinition type);
  }
}
