using System;

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

    static bool IsContext(IDeclaredElement declaredElement)
    {
      return declaredElement.IsContext();
    }

    static bool IsContextBase(IDeclaredElement declaredElement)
    {
      return declaredElement.IsContextBaseClass();
    }

    static bool IsSpecification(IDeclaredElement declaredElement)
    {
      return declaredElement.IsSpecification() && IsInSpecificationContainer(declaredElement);
    }

    static bool IsSupportingField(IDeclaredElement declaredElement)
    {
      return declaredElement.IsSupportingField() && IsInSpecificationContainer(declaredElement);
    }

    static bool IsBehavior(IDeclaredElement declaredElement)
    {
      return declaredElement.IsBehavior() && IsInSpecificationContainer(declaredElement);
    }

    static bool IsField(IDeclaredElement declaredElement)
    {
      return declaredElement.IsField() && IsInSpecificationContainer(declaredElement);
    }

    static bool IsConstant(IDeclaredElement declaredElement)
    {
      return declaredElement.IsConstant() && IsInSpecificationContainer(declaredElement);
    }

    static bool IsLocal(IDeclaredElement declaredElement)
    {
      return declaredElement.IsLocal() && IsInSpecificationContainer(declaredElement);
    }

    /// <summary>
    /// Determines if the declared element is contained in a MSpec-related container type,
    /// i.e.: Context, context base class, class with <see cref="BehaviorsAttribute" />.
    /// </summary>
    static bool IsInSpecificationContainer(IDeclaredElement declaredElement)
    {
      ITypeElement containingType = null;
      if (declaredElement is ITypeMember)
        containingType = ((ITypeMember) declaredElement).GetContainingType();
      else if (declaredElement is ITypeElement)
        containingType = (ITypeElement) declaredElement;

      return IsContext(containingType) || containingType.IsBehaviorContainer() || IsContextBase(containingType);
    }
  }
}