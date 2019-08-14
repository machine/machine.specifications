using System;
using System.Dynamic;
using System.Reflection;

namespace Machine.Specifications.Specs
{
    public class DynamicWrapper : DynamicObject
    {
        private readonly Type _type;

        public DynamicWrapper(Type type)
        {
            _type = type;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var field = _type.GetField(binder.Name, BindingFlags.Static | BindingFlags.Public);

            if (field == null)
                throw new InvalidOperationException();

            result = new DynamicWrapperExecutor(field.GetValue(null));

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var field = _type.GetField(binder.Name, BindingFlags.Static | BindingFlags.Public);

            if (field == null)
                throw new InvalidOperationException();

            field.SetValue(null, value);

            return true;
        }
    }
}
