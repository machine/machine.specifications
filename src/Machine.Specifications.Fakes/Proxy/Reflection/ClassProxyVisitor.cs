namespace Machine.Specifications.Fakes.Proxy.Reflection
{
    public class ClassProxyVisitor : ProxyVisitor
    {
        private ITypeEmitter typeEmitter;

        public ClassProxyVisitor(ITypeEmitter typeEmitter)
        {
            this.typeEmitter = typeEmitter;
        }
    }
}
