﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using JetBrains.ReSharper.UnitTestFramework.Elements;
#endif
using JetBrains.Util;

using Machine.Specifications.Factories;
using Machine.Specifications.Utility.Internal;

using ContextFactory = Machine.Specifications.ReSharperRunner.Factories.ContextFactory;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class ContextElement : Element, ISerializableElement
  {
    readonly string _assemblyLocation;
    readonly string _subject;
    readonly IEnumerable<UnitTestElementCategory> _categories;

    public ContextElement(MSpecUnitTestProvider provider,
                          PsiModuleManager psiModuleManager,
                          CacheManager cacheManager, 
                          ProjectModelElementEnvoy projectEnvoy,
                          string typeName,
                          string assemblyLocation,
                          string subject,
                          IEnumerable<string> tags,
                          bool isIgnored)
      : base(provider, psiModuleManager, cacheManager, null, projectEnvoy, typeName, isIgnored)
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

    public override string Id
    {
      get { return CreateId(_subject, TypeName); }
    }

    public void WriteToXml(XmlElement parent)
    {
      parent.SetAttribute("projectId", GetProject().GetPersistentID());
      parent.SetAttribute("typeName", TypeName);
      parent.SetAttribute("assemblyLocation", AssemblyLocation);
      parent.SetAttribute("isIgnored", Explicit.ToString());
      parent.SetAttribute("subject", _subject);
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, MSpecUnitTestProvider provider, ISolution solution
#if RESHARPER_61
      , IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager
#endif
      )
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

      return ContextFactory.GetOrCreateContextElement(provider,
#if RESHARPER_61
                                                      manager, psiModuleManager, cacheManager,
#endif
                                                      project,
                                                      ProjectModelElementEnvoy.Create(project),
                                                      typeName,
                                                      assemblyLocation,
                                                      subject,
                                                      EmptyArray<string>.Instance,
                                                      isIgnored);
    }

    public static string CreateId(string subject, string typeName)
    {
      var id = String.Format("{0}.{1}", subject, typeName);
      System.Diagnostics.Debug.WriteLine("CE  " + id);
      return id;
    }
  }
}