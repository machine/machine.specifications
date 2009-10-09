using System;
using System.Linq;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Filtering;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal abstract class FieldElement : Element
  {
    readonly string _fieldName;

    protected FieldElement(IUnitTestProvider provider,
                           UnitTestElement parent,
                           IProjectModelElement project,
                           string declaringTypeName,
                           string fieldName,
                           bool isIgnored)
      : base(provider, parent, project, declaringTypeName, isIgnored || parent.IsExplicit)
    {
      _fieldName = fieldName;
      AssignCategories(parent.GetCategories().Select(x => x.Name).ToList());
    }

    public string FieldName
    {
      get { return _fieldName; }
    }

    public override string GetTitle()
    {
      return String.Format("{0}{1}{2}",
                           GetTitlePrefix(),
                           String.IsNullOrEmpty(GetTitlePrefix()) ? String.Empty : " ",
                           FieldName.ToFormat());
    }

    public override bool Matches(string filter, PrefixMatcher matcher)
    {
      if (Parent.Matches(filter, matcher))
      {
        return true;
      }
      return matcher.IsMatch(FieldName);
    }

    public override IDeclaredElement GetDeclaredElement()
    {
      ITypeElement declaredType = GetDeclaredType();
      if (declaredType == null)
      {
        return null;
      }

      return declaredType.EnumerateMembers(FieldName, false)
        .Where(member => member as IField != null)
        .FirstOrDefault();
    }

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
      {
        FieldElement other = (FieldElement)obj;
        return Equals(Parent, other.Parent) && FieldName == other.FieldName;
      }

      return false;
    }

    public override int GetHashCode()
    {
      int result = base.GetHashCode();
      result = 29 * result + Parent.GetHashCode();
      result = 29 * result + FieldName.GetHashCode();
      return result;
    }
  }
}