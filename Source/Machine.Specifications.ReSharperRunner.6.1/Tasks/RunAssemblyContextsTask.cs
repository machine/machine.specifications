using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace Machine.Specifications.ReSharperRunner.Tasks
{
  [Serializable]
  class RunAssemblyTask : RemoteTask, IEquatable<RunAssemblyTask>
  {
    readonly string _assemblyLocation;

    public RunAssemblyTask(XmlElement element) : base(element)
    {
      _assemblyLocation = GetXmlAttribute(element, "AssemblyLocation");
    }

    public RunAssemblyTask(string providerId, string assemblyLocation)
      : base(providerId)
    {
      _assemblyLocation = assemblyLocation;
    }

    public override bool IsMeaningfulTask
    {
      get { return false; }
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

    public override bool Equals(object other)
    {
      if (this == other)
      {
        return true;
      }

      return Equals(other as RunAssemblyTask);
    }

    public bool Equals(RunAssemblyTask other)
    {
      if (other == null)
      {
        return false;
      }
      return Equals(_assemblyLocation, other._assemblyLocation);
    }

    public override bool Equals(RemoteTask other)
    {
      if (this == other)
      {
        return true;
      }

      return Equals(other as RunAssemblyTask);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + 29 * _assemblyLocation.GetHashCode();
    }
  }
}