using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  abstract class Task : RemoteTask, IEquatable<Task>
  {
    readonly string _assemblyLocation;

    protected Task(XmlElement element) : base(element)
    {
      _assemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
    }

    protected Task(string providerId, string assemblyLocation)
      : base(providerId)
    {
      _assemblyLocation = assemblyLocation;
    }

    public string AssemblyLocation
    {
      get { return _assemblyLocation; }
    }

    public override void SaveXml(XmlElement element)
    {
      base.SaveXml(element);

      SetXmlAttribute(element, "AssemblyLocation", AssemblyLocation);
    }

    public bool Equals(Task other)
    {
      if (other == null)
      {
        return false;
      }

      return Equals(RunnerID, other.RunnerID) &&
             Equals(AssemblyLocation, other.AssemblyLocation);
    }

    public override bool Equals(RemoteTask other)
    {
      return Equals(other as Task);
    }

    public override bool Equals(object other)
    {
      return Equals(other as Task);
    }

    public override int GetHashCode()
    {
      unchecked
      {
#if !RESHARPER_8
        var result = base.GetHashCode();
#else
        var result = Id.GetHashCode();
#endif
        result = (result * 397) ^ AssemblyLocation.GetHashCode();
        return result;
      }
    }
  }
}