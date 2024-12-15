namespace Machine.Specifications.Fakes.Proxy
{
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }
}
