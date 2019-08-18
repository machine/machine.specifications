using System;
using System.Dynamic;
using System.Reflection;

namespace Machine.Specifications.Specs
{
    public class DynamicWrapper : DynamicObject
    {
        private readonly Type _type;
        private readonly object _value;

        public DynamicWrapper(Type type)
        {
            _type = type;
        }

        protected DynamicWrapper(object value)
        {
            _value = value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var field = _type.GetField(binder.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (field == null)
                throw new InvalidOperationException();

            result = new DynamicWrapper(field.GetValue(null));

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var field = _type.GetField(binder.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (field == null)
                throw new InvalidOperationException();

            field.SetValue(null, value);

            return true;
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

            if (binder.Name == "ShouldBeGreaterThan")
            {
                ((IComparable) _value).ShouldBeGreaterThan((IComparable) args[0]);

                return true;
            }

            var method = _type.GetMethod(binder.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if (method != null)
            {
                method.Invoke(null, new object[0]);

                return true;
            }

            throw new SpecificationException();
        }
    }
}
