using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;
using JetBrains.Util;

using Machine.Specifications.ReSharperRunner.Factories;
using Machine.Specifications.ReSharperRunner.Shims;

using ICache = Machine.Specifications.ReSharperRunner.Shims.ICache;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  public class BehaviorElement : FieldElement
  {
    readonly string _id;

    public BehaviorElement(MSpecUnitTestProvider provider,
                           IPsi psiModuleManager,
                           ICache cacheManager,
                           // ReSharper disable SuggestBaseTypeForParameter
                           ContextElement context,
                           // ReSharper restore SuggestBaseTypeForParameter
                           ProjectModelElementEnvoy projectEnvoy,
                           IClrTypeName declaringTypeName,
                           string fieldName,
                           bool isIgnored,
                           string fieldType)
      : base(provider,
             psiModuleManager,
             cacheManager,
             context,
             projectEnvoy,
             declaringTypeName,
             fieldName,
             isIgnored || context.Explicit)
    {
      FieldType = fieldType;
      _id = CreateId(context, fieldType, fieldName);
    }

    public ContextElement Context
    {
      get { return (ContextElement) Parent; }
    }

    public string FieldType { get; private set; }

    public override string Kind
    {
      get { return "Behavior"; }
    }

    public override IEnumerable<UnitTestElementCategory> Categories
    {
      get
      {
        var parent = Parent ?? Context;
        if (parent == null)
        {
          return UnitTestElementCategory.Uncategorized;
        }

        return parent.Categories;
      }
    }

    public override string Id
    {
      get { return _id; }
    }

    public override string GetTitlePrefix()
    {
      return "behaves like";
    }

    public override void WriteToXml(XmlElement parent)
    {
      base.WriteToXml(parent);
      parent.SetAttribute("fieldType", FieldType);
    }

    public static IUnitTestElement ReadFromXml(XmlElement parent,
                                               IUnitTestElement parentElement,
                                               ISolution solution,
                                               BehaviorFactory factory)
    {
      var projectId = parent.GetAttribute("projectId");
      var project = ProjectUtil.FindProjectElementByPersistentID(solution, projectId) as IProject;
      if (project == null)
      {
        return null;
      }

      var context = parentElement as ContextElement;
      if (context == null)
      {
        return null;
      }

      var typeName = parent.GetAttribute("typeName");
      var methodName = parent.GetAttribute("methodName");
      var isIgnored = bool.Parse(parent.GetAttribute("isIgnored"));
      var fieldType = parent.GetAttribute("fieldType");

      return factory.GetOrCreateBehavior(context,
                                         new ClrTypeName(typeName),
                                         methodName,
                                         isIgnored,
                                         fieldType);
    }

    public static string CreateId(ContextElement contextElement, string fieldType, string fieldName)
    {
      var result = new[] {contextElement.Id, fieldType, fieldName};
      return result.Where(s => !string.IsNullOrEmpty(s)).AggregateString(".");
    }
  }
}
