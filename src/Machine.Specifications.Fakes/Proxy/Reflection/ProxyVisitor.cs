using System;
using System.Reflection;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public abstract class ProxyVisitor
    {
        public virtual void Visit(MemberInfo memberInfo)
        {
            memberInfo.Accept(this);
        }

        public virtual void VisitType(Type type)
        {
        }

        public virtual void VisitConstructor(ConstructorInfo constructorInfo)
        {
        }

        public virtual void VisitProperty(PropertyInfo propertyInfo)
        {
        }

        public virtual void VisitEvent(EventInfo eventInfo)
        {
        }

        public virtual void VisitMethod(MethodInfo methodInfo)
        {
        }
    }
}
