using System.Collections.Generic;

namespace Machine.Specifications.Reflection;

public class CompareContext
{
    public HashSet<(object, object)> Visited { get; } = new();

    public List<SpecificationException> Exceptions { get; } = new();
}
