using System;
using System.Collections.Generic;

#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif
#if RESHARPER_6
using System.Xml;
#endif 
using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner
{
#if RESHARPER_6
  internal class UnitTestElementComparer : Comparer<XmlElement>
#else
  internal class UnitTestElementComparer : Comparer<UnitTestElement>
#endif
  {

#if RESHARPER_6
    public override int Compare(XmlElement x, XmlElement y)
#else
    public override int Compare(UnitTestElement x, UnitTestElement y)
#endif
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
      return StringComparer.CurrentCultureIgnoreCase.Compare(x.InnerText, y.InnerText);
#else
      return StringComparer.CurrentCultureIgnoreCase.Compare(x.GetTitle(), y.GetTitle());
#endif
    }

  }
}