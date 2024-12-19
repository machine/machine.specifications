using System;

namespace Machine.Specifications.Runner.VisualStudio.Navigation
{
    public interface INavigationSession : IDisposable
    {
        NavigationData GetNavigationData(string typeName, string fieldName);
    }
}
