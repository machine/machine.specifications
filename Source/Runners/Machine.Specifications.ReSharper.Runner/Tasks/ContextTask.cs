using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  class ContextTask : Task, IEquatable<ContextTask>
  {
    public ContextTask(XmlElement element) : base(element)
    {
      ContextTypeName = GetXmlAttribute(element, "ContextTypeName");
    }

    public ContextTask(string providerId, string assemblyLocation, string contextTypeName)
      : base(providerId, assemblyLocation)
    {
      ContextTypeName = contextTypeName;
    }

    public string ContextTypeName { get; private set; }

    public override bool IsMeaningfulTask
    {
      get { return true; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "ContextTypeName", ContextTypeName);
    }

    public bool Equals(ContextTask other)
    {
      if (other == null)
      {
        return false;
      }

      return base.Equals(other) &&
             Equals(ContextTypeName, other.ContextTypeName);
    }

    public override bool Equals(object other)
    {
      return Equals(other as ContextTask);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var result = base.GetHashCode();
        result = (result * 397) ^ ContextTypeName.GetHashCode();
        return result;
      }
    }
  }
}