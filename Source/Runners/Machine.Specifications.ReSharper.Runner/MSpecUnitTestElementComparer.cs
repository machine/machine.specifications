using System;
using System.Collections.Generic;

using Machine.Specifications.ReSharperRunner.Presentation;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner
{
  class MSpecUnitTestElementComparer : Comparer<IUnitTestElement>
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

    public override int Compare(IUnitTestElement x, IUnitTestElement y)
    {
      var different = Array.IndexOf(_typeOrder, x.GetType()) - Array.IndexOf(_typeOrder, y.GetType());
      if (different != 0)
      {
        return different;
      }
      return x.ShortName.CompareTo(y.ShortName);
    }
  }
}