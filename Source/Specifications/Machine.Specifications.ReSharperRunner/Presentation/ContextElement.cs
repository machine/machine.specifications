using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.CamelTyping;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestExplorer;

using Machine.Specifications.Utility;

namespace Machine.Specifications.ReSharperRunner.Presentation
{
  internal class ContextElement : Element
  {
    readonly string _assemblyLocation;

    public ContextElement(IUnitTestProvider provider,
                          IProjectModelElement project,
                          string typeName,
                          string assemblyLocation)
      : base(provider, null, project, typeName)
    {
      _assemblyLocation = assemblyLocation;
    }

    public string AssemblyLocation
    {
      get { return _assemblyLocation; }
    }

    public override bool Matches(string filter, PrefixMatcher matcher)
    {
      return matcher.IsMatch(GetTypeClrName());
    }

    public override string GetTitle()
    {
      return new CLRTypeName(GetTypeClrName()).ShortName.ReplaceUnderscores();
    }

    public override IDeclaredElement GetDeclaredElement()
    {
      ISolution solution = GetSolution();
      if (solution == null)
      {
        return null;
      }

      PsiManager manager = PsiManager.GetInstance(solution);
      DeclarationsCacheScope scope = DeclarationsCacheScope.ProjectScope(GetProject(), false);
      IDeclarationsCache cache = manager.GetDeclarationsCache(scope, true);
      return cache.GetTypeElementByCLRName(GetTypeClrName());
    }

    public override string GetKind()
    {
      return "Context";
    }
  }
}