// At the moment all runners are not supporting custom delegates.
// TestDriven.Net and the console runners are expected to work.
namespace Machine.Specifications.Example.CustomDelegates
{
  [DelegateUsage(DelegateUsage.Setup)]
  public delegate void Given();

  [DelegateUsage(DelegateUsage.Act)]
  public delegate void When();

  [DelegateUsage(DelegateUsage.Assert)]
  public delegate void Then();
}