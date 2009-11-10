using System;

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming.Elements;
using JetBrains.ReSharper.UnitTestExplorer;

#if RESHARPER_5
using JetBrains.ReSharper.UnitTestFramework;
#endif

namespace Machine.Specifications.ReSharperRunner
{
  [NamedElementsBag(null)]
  public class MSpecElementNaming : ElementKindOfElementType
  {
    public static readonly IElementKind Behavior = new MSpecElementNaming("Machine.Specifications_Behavior",
                                                                          "Machine.Specifications behavior (field of type Behaves_like<>)",
                                                                          IsBehavior);

    public static readonly IElementKind Context = new MSpecElementNaming("Machine.Specifications_Context",
                                                                         "Machine.Specifications context (class containing specifications)",
                                                                         IsContext);

    public static readonly IElementKind ContextBase = new MSpecElementNaming("Machine.Specifications_ContextBase",
                                                                             "Machine.Specifications context base class",
                                                                             IsContextBase);

    public static readonly IElementKind Specification = new MSpecElementNaming("Machine.Specifications_Specification",
                                                                               "Machine.Specifications specification (field of type It)",
                                                                               IsSpecification);

    public static readonly IElementKind SupportingField =
      new MSpecElementNaming("Machine.Specifications_SupportingField",
                             "Machine.Specifications supporting field (Establish, Because, Cleanup)",
                             IsSupportingField);

    protected MSpecElementNaming(string name, string presentableName, Func<IDeclaredElement, bool> isApplicable)
      : base(name, presentableName, isApplicable)
    {
    }

#if RESHARPER_5
    public override PsiLanguageType Language
    {
      get { return PsiLanguageType.ANY; }
    }
#endif

    static bool IsTestElement(IDeclaredElement declaredElement)
    {
      return declaredElement.IsContext() || declaredElement.IsSpecification();
    }

    static bool IsContext(IDeclaredElement declaredElement)
    {
      return declaredElement.IsContext() && IsTestElement(declaredElement);
    }

    static bool IsContextBase(IDeclaredElement declaredElement)
    {
      return declaredElement.IsContextBaseClass();
    }

    static bool IsSpecification(IDeclaredElement declaredElement)
    {
      return declaredElement.IsSpecification() && IsTestElement(declaredElement);
    }

    static bool IsBehavior(IDeclaredElement declaredElement)
    {
      return declaredElement.IsBehavior() && IsTestElement(declaredElement.GetContainingType());
    }

    static bool IsSupportingField(IDeclaredElement declaredElement)
    {
      return declaredElement.IsSupportingField() &&
             (IsTestElement(declaredElement.GetContainingType()) || IsContextBase(declaredElement.GetContainingType()));
    }
  }
}