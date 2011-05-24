using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class BehaviorTask : Task, IEquatable<BehaviorTask>
  {
    readonly string _behaviorFieldName;

    public BehaviorTask(XmlElement element) : base(element)
    {
      _behaviorFieldName = GetXmlAttribute(element, "BehaviorFieldName");
    }

    public BehaviorTask(string providerId,
                        string assemblyLocation,
                        string contextTypeName,
                        string behaviorFieldName,
                        bool runExplicitly)
      : base(providerId, assemblyLocation, contextTypeName, runExplicitly)
    {
      _behaviorFieldName = behaviorFieldName;
    }

    public string BehaviorFieldName
    {
      get { return _behaviorFieldName; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "BehaviorFieldName", BehaviorFieldName);
    }

    public override bool Equals(object other)
    {
      return Equals(other as BehaviorTask);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = base.GetHashCode();
        result = (result * 397) ^ BehaviorFieldName.GetHashCode();
        return result;
      }
    }

#if RESHARPER_6

    public override bool IsMeaningfulTask
    {
      get { return true; }
    }

#endif

    public bool Equals(BehaviorTask other)
    {
      if (other == null || !base.Equals(other))
      {
        return false;
      }

      return Equals(BehaviorFieldName, other.BehaviorFieldName);
    }
  }
}