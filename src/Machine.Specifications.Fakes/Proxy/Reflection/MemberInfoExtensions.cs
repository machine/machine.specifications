using System;
using System.Reflection;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public static class MemberInfoExtensions
    {
        public static void Accept(this MemberInfo member, ProxyVisitor visitor)
        {
            switch (member)
            {
                case Type type:
                    visitor.VisitType(type);
                    break;

                case ConstructorInfo constructorInfo:
                    visitor.VisitConstructor(constructorInfo);
                    break;

                case PropertyInfo propertyInfo:
                    visitor.VisitProperty(propertyInfo);
                    break;

                case EventInfo eventInfo:
                    visitor.VisitEvent(eventInfo);
                    break;

                case MethodInfo methodInfo:
                    visitor.VisitMethod(methodInfo);
                    break;
            }
        }
    }
}
