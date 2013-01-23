using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class BehaviorTask : Task, IEquatable<BehaviorTask>
  {
    readonly string _behaviorFieldName;
    readonly string _behaviorFieldType;

    public BehaviorTask(XmlElement element) : base(element)
    {
      _behaviorFieldType = GetXmlAttribute(element, "BehaviorFieldType");
      _behaviorFieldName = GetXmlAttribute(element, "BehaviorFieldName");
    }

    public BehaviorTask(string providerId,
                        string assemblyLocation,
                        string contextTypeName,
                        string behaviorFieldType,
                        string behaviorFieldName,
                        bool runExplicitly)
      : base(providerId, assemblyLocation, contextTypeName, runExplicitly)
    {
      _behaviorFieldType = behaviorFieldType;
      _behaviorFieldName = behaviorFieldName;
    }

    public string BehaviorFieldType
    {
      get { return _behaviorFieldType; }
    }

    public string BehaviorFieldName
    {
      get { return _behaviorFieldName; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "BehaviorFieldType", BehaviorFieldType);
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
        result = (result * 397) ^ BehaviorFieldType.GetHashCode();
        result = (result * 397) ^ BehaviorFieldName.GetHashCode();
        return result;
      }
    }

    public override bool IsMeaningfulTask
    {
      get { return true; }
    }

    public bool Equals(BehaviorTask other)
    {
      if (other == null || !base.Equals(other))
      {
        return false;
      }

      return Equals(BehaviorFieldType, other.BehaviorFieldType) &&
             Equals(BehaviorFieldName, other.BehaviorFieldName);
    }
  }
}