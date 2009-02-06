using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class ContextTask : RemoteTask
  {
    public ContextTask(XmlElement element) : base(element)
    {
    }

    public ContextTask(string runnerId) : base(runnerId)
    {
    }

    public ContextTask(string providerId, string assemblyLocation, string contextTypeName, bool @explicit)
      : base(providerId)
    {
      throw new NotImplementedException();
    }
  }
}