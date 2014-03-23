using System;
using System.Xml;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  class ContextSpecificationTask : Task, IEquatable<ContextSpecificationTask>
  {
    public ContextSpecificationTask(XmlElement element) : base(element)
    {
      ContextTypeName = GetXmlAttribute(element, "ContextTypeName");
      SpecificationFieldName = GetXmlAttribute(element, "SpecificationFieldName");
    }

    public ContextSpecificationTask(string providerId,
                                    string assemblyLocation,
                                    string contextTypeName,
                                    string specificationFieldName)
      : base(providerId, assemblyLocation)
    {
      ContextTypeName = contextTypeName;
      SpecificationFieldName = specificationFieldName;
    }

    public string ContextTypeName { get; private set; }

    public string SpecificationFieldName { get; private set; }

    public override bool IsMeaningfulTask
    {
      get { return true; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "ContextTypeName", ContextTypeName);
      SetXmlAttribute(element, "SpecificationFieldName", SpecificationFieldName);
    }

    public bool Equals(ContextSpecificationTask other)
    {
      if (other == null)
      {
        return false;
      }

      return base.Equals(other) &&
             Equals(ContextTypeName, other.ContextTypeName) &&
             Equals(SpecificationFieldName, other.SpecificationFieldName);
    }

    public override bool Equals(object other)
    {
      return Equals(other as ContextSpecificationTask);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var result = base.GetHashCode();
        result = (result * 397) ^ ContextTypeName.GetHashCode();
        result = (result * 397) ^ SpecificationFieldName.GetHashCode();
        return result;
      }
    }
  }
}