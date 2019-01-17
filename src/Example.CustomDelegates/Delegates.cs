// At the moment all runners are not supporting custom delegates.
// TestDriven.Net and the console runners are expected to work.

using Machine.Specifications;

namespace Example.CustomDelegates
{
  [SetupDelegate]
  public delegate void Given();

  [ActDelegate]
  public delegate void When();

  [AssertDelegate]
  public delegate void Then();
}