namespace Machine.Specifications.Execution;

public class TestContext
{
    public string TestId { get; set; } = string.Empty;

    public Type Context { get; set; } = null!;

    public string GetDisplayName()
    {
        return Context.FullName!;
    }
}
