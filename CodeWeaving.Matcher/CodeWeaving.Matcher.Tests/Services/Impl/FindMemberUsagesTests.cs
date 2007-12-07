using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace CodeWeaving.Matcher.Services.Impl
{
  [TestFixture]
  public class FindMemberUsagesTests : TestFixture<FindMemberUsages>
  {
    public override FindMemberUsages Create()
    {
      return new FindMemberUsages();
    }
  }
}