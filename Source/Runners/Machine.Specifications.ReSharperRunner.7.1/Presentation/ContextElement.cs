using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Shims;
using Machine.Specifications.Utility.Internal;

using ICache = Machine.Specifications.ReSharperRunner.Shims.ICache;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class ContextElement : Element, ISerializableElement
  {
    readonly string _assemblyLocation;
    readonly IEnumerable<UnitTestElementCategory> _categories;
    readonly string _id;
    readonly string _subject;

    public ContextElement(MSpecUnitTestProvider provider,
                          IPsi psiModuleManager,
                          ICache cacheManager,
                          ProjectModelElementEnvoy projectEnvoy,
                          IClrTypeName typeName,
                          string assemblyLocation,
                          string subject,
                          IEnumerable<string> tags,
                          bool isIgnored)
      : base(provider, psiModuleManager, cacheManager, null, projectEnvoy, typeName, isIgnored)
    {
      _id = CreateId(subject, TypeName.FullName, tags);
      _assemblyLocation = assemblyLocation;
      _subject = subject;

      if (tags != null)
      {
        _categories = UnitTestElementCategory.Create(new JetHashSet<string>(tags));
      }
    }

    public override string ShortName
    {
      get { return Kind + GetPresentation(); }
    }

    public string AssemblyLocation
    {
      get { return _assemblyLocation; }
    }

    public override string Kind
    {
      get { return "Context"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return _categories; }
    }

    public override string Id
    {
      get { return _id; }
    }

    public void WriteToXml(XmlElement parent)
    {
      parent.SetAttribute("projectId", GetProject().GetPersistentID());
      parent.SetAttribute("typeName", TypeName.FullName);
      parent.SetAttribute("assemblyLocation", AssemblyLocation);
      parent.SetAttribute("isIgnored", Explicit.ToString());
      parent.SetAttribute("subject", _subject);
    }

    public override string GetPresentation()
    {
      return GetSubject() + GetTypeClrName().ShortName.ToFormat();
    }

    string GetSubject()
    {
      if (String.IsNullOrEmpty(_subject))
      {
        return null;
      }

      return _subject + ", ";
    }

    public override IDeclaredElement GetDeclaredElement()
    {
      return GetDeclaredType();
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent,
                                               ISolution solution,
                                               ContextFactory factory)
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(solution, projectId) as IProject;
      if (project == null)
      {
        return null;
      }

      var typeName = parent.GetAttribute("typeName");
      var assemblyLocation = parent.GetAttribute("assemblyLocation");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));
      var subject = parent.GetAttribute("subject");

      return factory.GetOrCreateContext(assemblyLocation,
                                        project,
                                        new ClrTypeName(typeName),
                                        subject,
                                        EmptyArray<string>.Instance,
                                        isIgnored);
    }

    public static string CreateId(string subject, string typeName, IEnumerable<string> tags)
    {
      string tagsAsString = null;
      if (tags != null)
      {
        tagsAsString = tags.AggregateString("", "|", (builder, tag) => builder.Append(tag));
      }
      var result = new[] {subject, typeName, tagsAsString};
      return result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
    }
  }
}