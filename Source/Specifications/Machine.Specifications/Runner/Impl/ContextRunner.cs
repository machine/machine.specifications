using Machine.Specifications.Model;

namespace Machine.Specifications.Runner.Impl
{
  public class ContextRunner
  {
    readonly ISpecificationRunListener _listener;
    readonly RunOptions _options;

    public ContextRunner(ISpecificationRunListener listener, RunOptions options)
    {
      _listener = listener;
      _options = options;
    }

    public void Run(Context context)
    {
      _listener.OnContextStart(context.GetInfo());

      foreach (var specification in context.EnumerateSpecificationsForVerification())
      {
        _listener.OnSpecificationStart(specification.GetInfo());
        var result = context.VerifySpecification(specification);
        _listener.OnSpecificationEnd(specification.GetInfo(), result);
      }

      _listener.OnContextEnd(context.GetInfo());
    }
  }
}