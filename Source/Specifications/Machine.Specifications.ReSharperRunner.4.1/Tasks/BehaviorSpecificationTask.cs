using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class BehaviorSpecificationTask : ContextSpecificationTask, IEquatable<BehaviorSpecificationTask>
  {
    readonly string _behaviorTypeName;

    public BehaviorSpecificationTask(XmlElement element) : base(element)
    {
      _behaviorTypeName = GetXmlAttribute(element, "BehaviorTypeName");
    }

    public BehaviorSpecificationTask(string providerId,
                                     string assemblyLocation,
                                     string contextTypeName,
                                     string behaviorSpecificationFieldName,
                                     string behaviorTypeName,
                                     bool runExplicitly)
      : base(providerId, assemblyLocation, contextTypeName, behaviorSpecificationFieldName, runExplicitly)
    {
      _behaviorTypeName = behaviorTypeName;
    }

    public string BehaviorTypeName
    {
      get { return _behaviorTypeName; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "BehaviorTypeName", BehaviorTypeName);
    }

    #region IEquatable<BehaviorSpecificationTask> Members
    public bool Equals(BehaviorSpecificationTask other)
    {
      if (other == null || !base.Equals(other))
      {
        return false;
      }

      return BehaviorTypeName == other.BehaviorTypeName;
    }
    #endregion

    public override bool Equals(object other)
    {
      return Equals(other as BehaviorSpecificationTask);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = base.GetHashCode();
        result = (result * 397) ^ BehaviorTypeName.GetHashCode();
        return result;
      }
    }
  }
}