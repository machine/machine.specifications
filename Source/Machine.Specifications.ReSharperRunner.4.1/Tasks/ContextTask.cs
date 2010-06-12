using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal partial class ContextTask : Task, IEquatable<ContextTask>
  {
    public ContextTask(XmlElement element) : base(element)
    {
    }

    public ContextTask(string providerId, string assemblyLocation, string contextTypeName, bool runExplicitly)
      : base(providerId, assemblyLocation, contextTypeName, runExplicitly)
    {
    }

    #region Implementation of IEquatable<ContextTask>
    public bool Equals(ContextTask other)
    {
      if (other == null || !base.Equals(other))
      {
        return false;
      }

      return true;
    }
    #endregion

    public override bool Equals(object other)
    {
      return Equals(other as ContextTask);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}