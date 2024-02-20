namespace Machine.Specifications.Reflection;

public class Node
{
    public Node(string name, object value, object expected)
    {
        Name = name;
        Value = value;
        Expected = expected;
    }

    public string Name { get; }

    public object Value { get; }

    public object Expected { get; }
}
