using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal class ContextTask : RemoteTask, IEquatable<ContextTask>
  {
    readonly string _assemblyLocation;
    readonly string _contextTypeName;
    readonly bool _runExplicitly;

    public ContextTask(XmlElement element) : base(element)
    {
      _contextTypeName = GetXmlAttribute(element, "ContextTypeName");
      _assemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
      _runExplicitly = GetXmlAttribute(element, "RunExplicitly") == "true";
    }

    public ContextTask(string providerId, string assemblyLocation, string contextTypeName, bool runExplicitly)
      : base(providerId)
    {
      if (contextTypeName == null)
      {
        throw new ArgumentNullException("contextTypeName");
      }

      _assemblyLocation = assemblyLocation;
      _contextTypeName = contextTypeName;
      _runExplicitly = runExplicitly;
    }

    public string AssemblyLocation
    {
      get { return _assemblyLocation; }
    }

    public string ContextTypeName
    {
      get { return _contextTypeName; }
    }

    public bool RunExplicitly
    {
      get { return _runExplicitly; }
    }

    #region IEquatable<ContextTask> Members
    public bool Equals(ContextTask obj)
    {
      if (obj == null || !base.Equals(obj))
      {
        return false;
      }

      return (Equals(AssemblyLocation, obj.AssemblyLocation) &&
              Equals(ContextTypeName, obj.ContextTypeName) &&
              RunExplicitly == obj.RunExplicitly);
    }
    #endregion

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);
      SetXmlAttribute(element, "ContextTypeName", ContextTypeName);
      SetXmlAttribute(element, "AssemblyLocation", AssemblyLocation);
      SetXmlAttribute(element, "RunExplicitly", RunExplicitly.ToString());
    }

    public override bool Equals(object obj)
    {
      return (this == obj || Equals(obj as ContextTask));
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = base.GetHashCode();
        result = (result * 397) ^ (AssemblyLocation != null ? AssemblyLocation.GetHashCode() : 0);
        result = (result * 397) ^ (ContextTypeName != null ? ContextTypeName.GetHashCode() : 0);
        result = (result * 397) ^ RunExplicitly.GetHashCode();
        return result;
      }
    }
  }
}