using System;
using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class ContextElement : Element
  {
    readonly string _assemblyLocation;
    readonly string _subject;
    readonly IEnumerable<UnitTestElementCategory> _categories;

    public ContextElement(MSpecUnitTestProvider provider,
                          ProjectModelElementEnvoy projectEnvoy,
                          string typeName,
                          string assemblyLocation,
                          string subject,
                          IEnumerable<string> tags,
                          bool isIgnored)
      : base(provider, null, projectEnvoy, typeName, isIgnored)
    {
      _assemblyLocation = assemblyLocation;
      _subject = subject;

      if (tags != null)
      {
        _categories = UnitTestElementCategory.Create(tags);
      }
    }

    public override string ShortName
    {
      get { return Kind; }
    }

    public string AssemblyLocation
    {
      get { return _assemblyLocation; }
    }

    public override string GetPresentation()
    {
      return GetSubject() + new ClrTypeName(GetTypeClrName()).ShortName.ToFormat();
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

    public override string Kind
    {
      get { return "Context"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get { return _categories; }
    }

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
      {
        var other = obj as ContextElement;
        if (other != null)
            return Equals(AssemblyLocation, other.AssemblyLocation);
      }

      return false;
    }

    public override int GetHashCode()
    {
      int result = base.GetHashCode();
      result = 29 * result + AssemblyLocation.GetHashCode();
      return result;
    }
  }
}