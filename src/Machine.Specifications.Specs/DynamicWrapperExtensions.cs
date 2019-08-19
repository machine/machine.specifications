using System;

namespace Machine.Specifications.Specs
{
    public static class DynamicWrapperExtensions
    {
        public static dynamic ToDynamic(this Type type)
        {
            return new DynamicWrapper(type);
        }
    }
}
