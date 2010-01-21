using System;
using System.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Naming.Elements;

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

    public static readonly IElementKind Fields = new MSpecElementNaming("Machine.Specifications_Field",
                                                                        "Machine.Specifications non-supporting field",
                                                                        IsField);

    public static readonly IElementKind Constants = new MSpecElementNaming("Machine.Specifications_Constant",
                                                                           "Machine.Specifications constant",
                                                                           IsConstant);

    public static readonly IElementKind Locals = new MSpecElementNaming("Machine.Specifications_Local",
                                                                        "Machine.Specifications local",
                                                                        IsLocal);

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

    static bool IsField(IDeclaredElement declaredElement)
    {
      return declaredElement.IsField() && IsInTestElementOrContext(declaredElement);
    }

    static bool IsConstant(IDeclaredElement declaredElement)
    {
        return declaredElement.IsConstant() && IsInTestElementOrContext(declaredElement);
    }

    static bool IsLocal(IDeclaredElement declaredElement)
    {
        return declaredElement.IsLocal() && IsInTestElementOrContext(declaredElement);
    }

    static bool IsBehavior(IDeclaredElement declaredElement)
    {
      return declaredElement.IsBehavior() && IsTestElement(declaredElement.GetContainingType());
    }

    static bool IsSupportingField(IDeclaredElement declaredElement)
    {
      return declaredElement.IsSupportingField() && IsInTestElementOrContext(declaredElement);
    }

    static bool IsInTestElementOrContext(IDeclaredElement declaredElement)
    {
        return IsTestElement(declaredElement.GetContainingType()) || IsContextBase(declaredElement.GetContainingType());
    }
  }
}