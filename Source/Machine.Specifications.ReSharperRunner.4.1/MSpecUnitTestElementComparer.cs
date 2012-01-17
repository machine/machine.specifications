using System;
using System.Collections.Generic;

using Machine.Specifications.ReSharperRunner.Presentation;
#if RESHARPER_5 || RESHARPER_6
using JetBrains.ReSharper.UnitTestFramework;
#else
using JetBrains.ReSharper.UnitTestExplorer;
#endif

namespace Machine.Specifications.ReSharperRunner
{
#if RESHARPER_6
  class MSpecUnitTestElementComparer : Comparer<IUnitTestElement>
#else
  class MSpecUnitTestElementComparer : Comparer<UnitTestElement>
#endif
  {
    readonly Type[] _typeOrder;

    public MSpecUnitTestElementComparer()
    {
      _typeOrder = new[]
                   {
                     typeof(ContextElement),
                     typeof(BehaviorElement),
                     typeof(BehaviorSpecificationElement),
                     typeof(ContextSpecificationElement)
                   };
    }

#if RESHARPER_6
    public override int Compare(IUnitTestElement x, IUnitTestElement y)
#else
    public override int Compare(UnitTestElement x, UnitTestElement y)
#endif
    {
      var different = Array.IndexOf(_typeOrder, x.GetType()) - Array.IndexOf(_typeOrder, y.GetType());
      if (different != 0)
      {
        return different;
      }
#if RESHARPER_6 
      return x.ShortName.CompareTo(y.ShortName);
#else
      return x.GetTitle().CompareTo(y.GetTitle());
#endif
    }
  }
}