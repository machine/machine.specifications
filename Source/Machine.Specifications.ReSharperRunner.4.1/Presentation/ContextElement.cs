using System.Collections.Generic;

using JetBrains.Application;
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
                          string assemblyLocation,
                          ICollection<string> tags,
                          bool isIgnored)
      : base(provider, null, project, typeName, isIgnored)
    {
      _assemblyLocation = assemblyLocation;

      if (tags != null)
      {
        AssignCategories(tags);
      }
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
      return new CLRTypeName(GetTypeClrName()).ShortName.ToFormat();
    }

    public override IDeclaredElement GetDeclaredElement()
    {
      ISolution solution = GetSolution();
      if (solution == null)
      {
        return null;
      }

      using (ReadLockCookie.Create())
      {
        DeclarationsCacheScope scope = DeclarationsCacheScope.SolutionScope(solution, false);
        IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);
        return cache.GetTypeElementByCLRName(GetTypeClrName());
      }
    }

    public override string GetKind()
    {
      return "Context";
    }

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
      {
        ContextElement other = (ContextElement)obj;
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