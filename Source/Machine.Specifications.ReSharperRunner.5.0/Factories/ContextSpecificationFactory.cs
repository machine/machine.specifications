using System.Collections.Generic;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  internal class ContextSpecificationFactory
  {
    readonly ProjectModelElementEnvoy _projectEnvoy;
    readonly MSpecUnitTestProvider _provider;
    readonly ContextCache _cache;
    readonly IProject _project;

    public ContextSpecificationFactory(MSpecUnitTestProvider provider, IProject project, ProjectModelElementEnvoy projectEnvoy, ContextCache cache)
    {
      _provider = provider;
      _cache = cache;
      _project = project;
      _projectEnvoy = projectEnvoy;
    }

    public ContextSpecificationElement CreateContextSpecification(IDeclaredElement field)
    {
#if RESHARPER_6
      IClass clazz = ((ITypeMember)field).GetContainingType() as IClass;
#else
      IClass clazz = field.GetContainingType() as IClass;
#endif
      if (clazz == null)
      {
        return null;
      }

      ContextElement context;
      _cache.Classes.TryGetValue(clazz, out context);
      if (context == null)
      {
        return null;
      }

      return GetOrCreateContextSpecification(_provider,
                                             _project,
                                             context,
                                             _projectEnvoy,
#if RESHARPER_6
                                             clazz.GetClrName().FullName,
#else
                                             clazz.CLRName,
#endif
                                             field.ShortName,
                                             clazz.GetTags(),
                                             field.IsIgnored());
    }

    public ContextSpecificationElement CreateContextSpecification(ContextElement context, IMetadataField specification)
    {
      return GetOrCreateContextSpecification(_provider,
                                             _project,
                                             context,
                                             _projectEnvoy,
                                             specification.DeclaringType.FullyQualifiedName,
                                             specification.Name,
                                             specification.DeclaringType.GetTags(),
                                             specification.IsIgnored());
    }

    public static ContextSpecificationElement GetOrCreateContextSpecification(MSpecUnitTestProvider provider,
                                                                              IProject project,
                                                                              ContextElement context,
                                                                              ProjectModelElementEnvoy projectEnvoy,
                                                                              string declaringTypeName,
                                                                              string fieldName,
                                                                              ICollection<string> tags,
                                                                              bool isIgnored)
    {
#if RESHARPER_6
      var id = ContextSpecificationElement.CreateId(context, fieldName);
      var contextSpecification = provider.UnitTestManager.GetElementById(project, id) as ContextSpecificationElement;
      if (contextSpecification != null)
      {
        contextSpecification.Parent = context;
        contextSpecification.State = UnitTestElementState.Valid;
        return contextSpecification;
      }
#endif

      return new ContextSpecificationElement(provider,
                                             context,
                                             projectEnvoy,
                                             declaringTypeName,
                                             fieldName,
                                             tags,
                                             isIgnored);
    }
  }
}
