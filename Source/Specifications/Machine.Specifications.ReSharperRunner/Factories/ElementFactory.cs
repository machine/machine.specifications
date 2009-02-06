using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Util.DataStructures;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ElementFactory
  {
    readonly string _assemblyPath;
    readonly Dictionary2<ITypeElement, ContextElement> _classes = new Dictionary2<ITypeElement, ContextElement>();
    readonly IProjectModelElement _project;
    readonly IUnitTestProvider _provider;

    public ElementFactory(IUnitTestProvider provider, IProjectModelElement project, string assemblyPath)
    {
      _provider = provider;
      _project = project;
      _assemblyPath = assemblyPath;
    }

    public ContextElement CreateContextElement(ITypeElement type)
    {
      if (_classes.ContainsKey(type))
      {
        return _classes[type];
      }

      ContextElement context = new ContextElement(_provider, _project, type.CLRName, _assemblyPath);
      _classes.Add(type, context);
      return context;
    }

    public ContextElement CreateContextElement(IMetadataTypeInfo type)
    {
      return new ContextElement(_provider, _project, type.FullyQualifiedName, _assemblyPath);
    }

    public SpecificationElement CreateSpecificationElement(IDeclaredElement field)
    {
      IClass clazz = field.GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement contextElement = _classes[clazz];
      if (contextElement == null)
      {
        return null;
      }

      return new SpecificationElement(_provider, contextElement, _project, clazz.CLRName, field.ShortName);
    }

    public SpecificationElement CreateSpecificationElement(ContextElement context, IMetadataField specification)
    {
      return new SpecificationElement(_provider,
                                      context,
                                      _project,
                                      specification.DeclaringType.FullyQualifiedName,
                                      specification.Name);
    }

    public BehaviorElement CreateBehaviorElement(IDeclaredElement field)
    {
      IClass clazz = field.GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement contextElement = _classes[clazz];
      if (contextElement == null)
      {
        return null;
      }

      return new BehaviorElement(_provider, contextElement, _project, clazz.CLRName, field.ShortName);
    }

    public BehaviorElement CreateBehaviorElement(ContextElement context, IMetadataField behavior)
    {
      return new BehaviorElement(_provider,
                                 context,
                                 _project,
                                 behavior.DeclaringType.FullyQualifiedName,
                                 behavior.Name);
    }
  }
}