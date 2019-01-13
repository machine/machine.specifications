namespace Machine.Specifications
{
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