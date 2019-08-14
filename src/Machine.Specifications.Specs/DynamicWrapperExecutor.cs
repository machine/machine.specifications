using System;
using System.Dynamic;

namespace Machine.Specifications.Specs
{
    public class DynamicWrapperExecutor : DynamicObject
    {
        private readonly object _value;

        public DynamicWrapperExecutor(object value)
        {
            _value = value;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;

            if (binder.Name == "ShouldBeTrue")
            {
                ((bool) _value).ShouldBeTrue();

                return true;
            }

            if (binder.Name == "ShouldBeFalse")
            {
                ((bool) _value).ShouldBeFalse();

                return true;
            }

            if (binder.Name == "ShouldEqual")
            {
                _value.ShouldEqual(args[0]);

                return true;
            }

            throw new SpecificationException();
        }
    }
}
