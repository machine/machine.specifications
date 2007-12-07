using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace CodeWeaving.Matcher.Services.Impl
{
  [TestFixture]
  public class MemberFinderTests : TestFixture<MemberFinder>
  {
    public override MemberFinder Create()
    {
      return new MemberFinder();
    }
  }
}