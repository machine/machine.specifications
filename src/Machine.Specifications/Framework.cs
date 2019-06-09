namespace Machine.Specifications
{
#if ASYNC
    using System.Threading.Tasks;

    [SetupDelegate]
    public delegate Task EstablishAsync();

    [ActDelegate]
    public delegate Task BecauseAsync();

    [AssertDelegate]
    public delegate Task ItAsync();

    [CleanupDelegate]
    public delegate Task CleanupAsync();
#endif

    [SetupDelegate]
    public delegate void Establish();

    [ActDelegate]
    public delegate void Because();

    [AssertDelegate]
    public delegate void It();

    [BehaviorDelegate]
    public delegate void Behaves_like<TBehavior>();

    [CleanupDelegate]
    public delegate void Cleanup();
}
