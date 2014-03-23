using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  class BehaviorSpecificationTask : Task, IEquatable<BehaviorSpecificationTask>
  {
    public BehaviorSpecificationTask(XmlElement element) : base(element)
    {
      ContextTypeName = GetXmlAttribute(element, "ContextTypeName");
      BehaviorTypeName = GetXmlAttribute(element, "BehaviorTypeName");
      SpecificationFieldName = GetXmlAttribute(element, "SpecificationFieldName");
      SpecificationFieldNameOnContext = GetXmlAttribute(element, "SpecificationFieldNameOnContext");
    }

    public BehaviorSpecificationTask(string providerId,
                                     string assemblyLocation,
                                     string contextTypeName,
                                     string specificationFieldNameOnContext,
                                     string behaviorSpecificationFieldName,
                                     string behaviorTypeName)
      : base(providerId, assemblyLocation)
    {
      ContextTypeName = contextTypeName;
      BehaviorTypeName = behaviorTypeName;
      SpecificationFieldName = behaviorSpecificationFieldName;
      SpecificationFieldNameOnContext = specificationFieldNameOnContext;
    }

    public string ContextTypeName { get; set; }

    public string BehaviorTypeName { get; private set; }

    public string SpecificationFieldName { get; private set; }

    string SpecificationFieldNameOnContext { get; set; }

    public override bool IsMeaningfulTask
    {
      get { return true; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "ContextTypeName", ContextTypeName);
      SetXmlAttribute(element, "SpecificationFieldName", SpecificationFieldName);
      SetXmlAttribute(element, "BehaviorTypeName", BehaviorTypeName);
      SetXmlAttribute(element, "SpecificationFieldName", SpecificationFieldName);
      SetXmlAttribute(element, "SpecificationFieldNameOnContext", SpecificationFieldNameOnContext);
    }

    public bool Equals(BehaviorSpecificationTask other)
    {
      if (other == null)
      {
        return false;
      }

      return base.Equals(other) &&
             Equals(ContextTypeName, other.ContextTypeName) &&
             Equals(BehaviorTypeName, other.BehaviorTypeName) &&
             Equals(SpecificationFieldName, other.SpecificationFieldName) &&
             Equals(SpecificationFieldNameOnContext, other.SpecificationFieldNameOnContext);
    }

    public override bool Equals(object other)
    {
      return Equals(other as BehaviorSpecificationTask);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var result = base.GetHashCode();
        result = (result * 397) ^ ContextTypeName.GetHashCode();
        result = (result * 397) ^ BehaviorTypeName.GetHashCode();
        result = (result * 397) ^ SpecificationFieldName.GetHashCode();
        result = (result * 397) ^ SpecificationFieldNameOnContext.GetHashCode();
        return result;
      }
    }
  }
}