using System;
using System.Collections.Generic;

using Mono.Cecil;

namespace CodeWeaving.Matcher.Matchers
{
  public interface IMemberMatcher
  {
    bool Includes(MemberReference memberReference);
  }
}
