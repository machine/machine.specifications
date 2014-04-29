namespace Machine.Specifications.ReSharperProvider
{
    using System;
    using System.Collections.Generic;

    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Presentation;

    class MSpecUnitTestElementComparer : Comparer<IUnitTestElement>
    {
        readonly Type[] _typeOrder;

        public MSpecUnitTestElementComparer()
        {
            this._typeOrder = new[]
                   {
                     typeof(ContextElement),
                     typeof(BehaviorElement),
                     typeof(BehaviorSpecificationElement),
                     typeof(ContextSpecificationElement)
                   };
        }

        public override int Compare(IUnitTestElement x, IUnitTestElement y)
        {
            var different = Array.IndexOf(this._typeOrder, x.GetType()) - Array.IndexOf(this._typeOrder, y.GetType());
            if (different != 0)
            {
                return different;
            }
            return x.ShortName.CompareTo(y.ShortName);
        }
    }
}