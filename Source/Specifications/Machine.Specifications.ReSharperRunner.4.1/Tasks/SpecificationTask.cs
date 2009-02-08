using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class SpecificationTask : RemoteTask, IEquatable<SpecificationTask>
  {
    readonly string _specificationFieldName;
    readonly string _contextTypeName;
    readonly bool _runExplicitly;
    readonly string _assemblyLocation;

    public SpecificationTask(XmlElement element) : base(element)
    {
      _contextTypeName = GetXmlAttribute(element, "ContextTypeName");
      _specificationFieldName = GetXmlAttribute(element, "SpecificationFieldName");
      _assemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
      _runExplicitly = GetXmlAttribute(element, "RunExplicitly") == "true";
    }

    public SpecificationTask(string providerId, string contextTypeName, string specificationFieldName, bool runExplicitly, string assemblyLocation)
      : base(providerId)
    {
      if (contextTypeName == null)
      {
        throw new ArgumentNullException("contextTypeName");
      }

      _contextTypeName = contextTypeName;
      _specificationFieldName = specificationFieldName;
      _runExplicitly = runExplicitly;
      _assemblyLocation = assemblyLocation;
    }

    public string AssemblyLocation
    {
      get { return _assemblyLocation; }
    }

    public string ContextTypeName
    {
      get { return _contextTypeName; }
    }

    public string SpecificationFieldName
    {
      get { return _specificationFieldName; }
    }

    public bool RunExplicitly
    {
      get { return _runExplicitly; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);
      SetXmlAttribute(element, "ContextTypeName", ContextTypeName);
      SetXmlAttribute(element, "SpecificationFieldName", SpecificationFieldName);
      SetXmlAttribute(element, "RunExplicitly", RunExplicitly.ToString());
      SetXmlAttribute(element, "AssemblyLocation", AssemblyLocation);
    }

    #region IEquatable<SpecificationTask> Members
    public bool Equals(SpecificationTask obj)
    {
      if (obj == null || !base.Equals(obj))
      {
        return false;
      }

      return (Equals(SpecificationFieldName, obj.SpecificationFieldName) &&
              Equals(ContextTypeName, obj.ContextTypeName) &&
              RunExplicitly == obj.RunExplicitly);
    }
    #endregion

    public override bool Equals(object obj)
    {
      return (this == obj || Equals(obj as SpecificationTask));
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = base.GetHashCode();
        result = (result * 397) ^ (SpecificationFieldName != null ? SpecificationFieldName.GetHashCode() : 0);
        result = (result * 397) ^ (ContextTypeName != null ? ContextTypeName.GetHashCode() : 0);
        result = (result * 397) ^ RunExplicitly.GetHashCode();
        return result;
      }
    }
  }
}