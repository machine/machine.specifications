using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class SpecificationTask : RemoteTask
  {
    public SpecificationTask(XmlElement element) : base(element)
    {
    }

    public SpecificationTask(string runnerId) : base(runnerId)
    {
    }

    public SpecificationTask(string providerId, string contextTypeName, string fieldName, bool @explicit)
      : base(providerId)
    {
      throw new NotImplementedException();
    }
  }
}