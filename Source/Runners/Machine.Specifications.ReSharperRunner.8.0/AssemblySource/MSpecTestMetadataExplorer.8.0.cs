using System.Threading;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace Machine.Specifications.ReSharperRunner.Explorers
{
  public partial class MSpecTestMetadataExplorer
  {
    public void ExploreAssembly(IProject project,
                                IMetadataAssembly assembly,
                                UnitTestElementConsumer consumer,
                                ManualResetEvent exitEvent)
    {
      ExploreAssembly(project, assembly, consumer);
    }
  }
}