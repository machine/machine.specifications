using System;

namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public class InterfaceProxyVisitor : ProxyVisitor
    {
        public InterfaceProxyVisitor(Type type)
        {
        }

        public override void VisitType(Type type)
        {
            base.VisitType(type);
        }
    }
}
