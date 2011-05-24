using System;
using System.Collections.Generic;

#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif
using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner
{
  internal class UnitTestElementComparer : Comparer<UnitTestElement>
  {

    public override int Compare(UnitTestElement x, UnitTestElement y)
    {
      if (Equals(x, y))
      {
        return 0;
      }

      if ((x is ContextSpecificationElement || x is BehaviorElement) && y is ContextElement)
      {
        return -1;
      }

      if (x is ContextElement && (y is ContextSpecificationElement || y is BehaviorElement))
      {
        return 1;
      }

      if (x is ContextSpecificationElement && y is BehaviorElement)
      {
        return 1;
      }

      if (x is BehaviorElement && y is ContextSpecificationElement)
      {
        return -1;
      }
#if RESHARPER_6
      return StringComparer.CurrentCultureIgnoreCase.Compare(x.ShortName, y.ShortName);
#else
      return StringComparer.CurrentCultureIgnoreCase.Compare(x.GetTitle(), y.GetTitle());
#endif
    }

  }
}