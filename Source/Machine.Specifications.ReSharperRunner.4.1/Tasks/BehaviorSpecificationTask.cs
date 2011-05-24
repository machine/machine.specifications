using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class BehaviorSpecificationTask : Task, IEquatable<BehaviorSpecificationTask>
  {
    readonly string _behaviorTypeName;
    readonly string _specificationFieldName;
    readonly string _specificationFieldNameOnContext;

    public BehaviorSpecificationTask(XmlElement element) : base(element)
    {
      _behaviorTypeName = GetXmlAttribute(element, "BehaviorTypeName");
      _specificationFieldName = GetXmlAttribute(element, "SpecificationFieldName");
      _specificationFieldNameOnContext = GetXmlAttribute(element, "SpecificationFieldNameOnContext");
    }

    public BehaviorSpecificationTask(string providerId,
                                     string assemblyLocation,
                                     string contextTypeName,
                                     string specificationFieldNameOnContext,
                                     string behaviorSpecificationFieldName,
                                     string behaviorTypeName,
                                     bool runExplicitly)
      : base(providerId, assemblyLocation, contextTypeName, runExplicitly)
    {
      _behaviorTypeName = behaviorTypeName;
      _specificationFieldName = behaviorSpecificationFieldName;
      _specificationFieldNameOnContext = specificationFieldNameOnContext;
    }

    public string BehaviorTypeName
    {
      get { return _behaviorTypeName; }
    }

    public string SpecificationFieldName
    {
      get { return _specificationFieldName; }
    }

    public string SpecificationFieldNameOnContext
    {
      get { return _specificationFieldNameOnContext; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "BehaviorTypeName", BehaviorTypeName);
      SetXmlAttribute(element, "SpecificationFieldName", SpecificationFieldName);
      SetXmlAttribute(element, "SpecificationFieldNameOnContext", SpecificationFieldNameOnContext);
    }

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
        result = (result * 397) ^ SpecificationFieldName.GetHashCode();
        result = (result * 397) ^ SpecificationFieldNameOnContext.GetHashCode();
        return result;
      }
    }

#if RESHARPER_6

    public override bool IsMeaningfulTask
    {
      get { return true; }
    }

#endif

    public bool Equals(BehaviorSpecificationTask other)
    {
      if (other == null || !base.Equals(other))
      {
        return false;
      }

      return Equals(BehaviorTypeName, other.BehaviorTypeName) &&
             Equals(SpecificationFieldName, other.SpecificationFieldName) &&
             Equals(SpecificationFieldNameOnContext, other.SpecificationFieldNameOnContext);
    }
  }
}