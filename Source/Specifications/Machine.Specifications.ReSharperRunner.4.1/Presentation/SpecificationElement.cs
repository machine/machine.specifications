using System.Linq;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.CamelTyping;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class SpecificationElement : Element
  {
    readonly ContextElement _context;
    readonly string _fieldName;

    public SpecificationElement(IUnitTestProvider provider,
                                ContextElement context,
                                IProjectModelElement project,
                                string declaringTypeName,
                                string fieldName)
      : base(provider, context, project, declaringTypeName)
    {
      _context = context;
      _fieldName = fieldName;
    }

    public ContextElement Context
    {
      get { return _context; }
    }

    public string FieldName
    {
      get { return _fieldName; }
    }

    public override string GetTitle()
    {
      return _fieldName.ReplaceUnderscores();
    }

    public override bool Matches(string filter, PrefixMatcher matcher)
    {
      if (_context.Matches(filter, matcher))
      {
        return true;
      }
      return matcher.IsMatch(_fieldName);
    }

    public override IDeclaredElement GetDeclaredElement()
    {
      ITypeElement declaredType = GetDeclaredType();
      if (declaredType == null)
      {
        return null;
      }

      return MiscUtil.EnumerateMembers(declaredType, _fieldName, false)
        .Where(member =>
          {
            var field = member as IField;
            if (field != null && !field.IsStatic)
            {
              return true;
            }

            return false;
          })
        .FirstOrDefault();
    }

    public override string GetKind()
    {
      return "Specification";
    }

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
      {
        SpecificationElement specificationElement = (SpecificationElement) obj;
        return Equals(_context, specificationElement._context) && _fieldName == specificationElement._fieldName;
      }

      return false;
    }

    public override int GetHashCode()
    {
      int result = base.GetHashCode();
      result = 29 * result + _context.GetHashCode();
      result = 29 * result + _fieldName.GetHashCode();
      return result;
    }
  }
}