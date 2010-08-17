using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  internal abstract partial class Task : RemoteTask, IEquatable<Task>
  {
    readonly string _assemblyLocation;
    readonly string _contextTypeName;
    readonly bool _runExplicitly;

    protected Task(XmlElement element) : base(element)
    {
      _assemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
      _contextTypeName = GetXmlAttribute(element, "ContextTypeName");
      _runExplicitly = GetXmlAttribute(element, "RunExplicitly") == true.ToString();
    }

    protected Task(string providerId, string assemblyLocation, string contextTypeName, bool runExplicitly)
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

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "AssemblyLocation", AssemblyLocation);
      SetXmlAttribute(element, "ContextTypeName", ContextTypeName);
      SetXmlAttribute(element, "RunExplicitly", RunExplicitly.ToString());
    }

    public override bool Equals(object other)
    {
      return Equals(other as Task);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = base.GetHashCode();
        result = (result * 397) ^ AssemblyLocation.GetHashCode();
        result = (result * 397) ^ ContextTypeName.GetHashCode();
        result = (result * 397) ^ RunExplicitly.GetHashCode();
        return result;
      }
    }

    public bool Equals(Task other)
    {
      if (other == null || !BaseEquals(other))
      {
        return false;
      }

      return (Equals(AssemblyLocation, other.AssemblyLocation) &&
              Equals(ContextTypeName, other.ContextTypeName) &&
              RunExplicitly == other.RunExplicitly);
    }
  }
}