using System.Reflection;

namespace Machine.Specifications.Execution;

public class TestCaseDiscoverer(Assembly assembly)
{
    public IEnumerable<TestContext> GetTests()
    {
        return assembly.GetTypes()
            .SelectMany(GetTestsFromType);
    }

    private IEnumerable<TestContext> GetTestsFromType(Type type)
    {
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(It))
            {
                yield return new TestContext
                {
                    TestId = $"{field.DeclaringType}.{field.Name}",
                    Context = field.DeclaringType!
                };
            }
        }
    }
}
