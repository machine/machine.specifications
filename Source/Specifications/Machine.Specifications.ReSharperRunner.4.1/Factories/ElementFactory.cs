using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.Util.DataStructures;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  // TODO: Split into separate factories.
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

      ContextElement context = new ContextElement(_provider,
                                                  _project,
                                                  type.CLRName,
                                                  _assemblyPath,
                                                  type.GetTags(),
                                                  type.IsIgnored());
      _classes.Add(type, context);
      return context;
    }

    public ContextElement CreateContextElement(IMetadataTypeInfo type)
    {
      return new ContextElement(_provider,
                                _project,
                                type.FullyQualifiedName,
                                _assemblyPath,
                                type.GetTags(),
                                type.IsIgnored());
    }

    public SpecificationElement CreateSpecificationElement(IDeclaredElement field)
    {
      IClass clazz = field.GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context = _classes[clazz];
      if (context == null)
      {
        return null;
      }

      return new SpecificationElement(_provider,
                                      context,
                                      _project,
                                      clazz.CLRName,
                                      field.ShortName,
                                      field.IsIgnored());
    }

    public SpecificationElement CreateSpecificationElement(ContextElement context, IMetadataField specification)
    {
      return new SpecificationElement(_provider,
                                      context,
                                      _project,
                                      specification.DeclaringType.FullyQualifiedName,
                                      specification.Name,
                                      specification.IsIgnored());
    }

    public BehaviorElement CreateBehaviorElement(IDeclaredElement field)
    {
      IClass clazz = field.GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      ContextElement context = _classes[clazz];
      if (context == null)
      {
        return null;
      }

      return new BehaviorElement(_provider,
                                 context,
                                 _project,
                                 clazz.CLRName,
                                 field.ShortName,
                                 field.IsIgnored());
    }

    public BehaviorElement CreateBehaviorElement(ContextElement context, IMetadataField behavior)
    {
      return new BehaviorElement(_provider,
                                 context,
                                 _project,
                                 behavior.DeclaringType.FullyQualifiedName,
                                 behavior.Name,
                                 behavior.IsIgnored());
    }
  }
}