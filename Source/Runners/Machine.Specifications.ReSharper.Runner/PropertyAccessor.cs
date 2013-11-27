using System;
using System.Linq.Expressions;

namespace Machine.Specifications.ReSharper.Runner
{
    class Accessor<T>
    {
        Func<object, T> _getAccessor;

        public Accessor(string propertyName)
        {
            _getAccessor = o =>
            {
                _getAccessor = GetPropertyAccessor(o.GetType(), propertyName);
                return _getAccessor(o);
            };
        }

        public Func<T> MemoizeGetter(object o)
        {
            return Functional.Memoize(() => _getAccessor(o));
        }

        protected virtual Func<object, T> GetPropertyAccessor(Type objectType, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(object), "o");
            var cast = Expression.Convert(parameter, objectType);
            var property = Expression.Property(cast, propertyName);
            return Expression.Lambda<Func<object, T>>(property, parameter).Compile();
        }
    }

    class EnumAccessor<TEnum> : Accessor<TEnum>
    {
        public EnumAccessor(string propertyName)
            : base(propertyName)
        {
        }

        protected override Func<object, TEnum> GetPropertyAccessor(Type objectType, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(object), "o");
            var cast = Expression.Convert(parameter, objectType);
            var property = Expression.Property(cast, propertyName);
            var stringValue = Expression.Call(property, "ToString", null);
            var enumValueObject = Expression.Call(typeof(Enum), "Parse", null, Expression.Constant(typeof(TEnum), typeof(Type)), stringValue);
            var enumValue = Expression.Convert(enumValueObject, typeof(TEnum));
            return Expression.Lambda<Func<object, TEnum>>(enumValue, parameter).Compile();
        }
    }
}