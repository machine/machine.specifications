using System.Linq;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl.Reflection2;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.Presentation;
using Machine.Specifications.ReSharperRunner.Shims;

using ICache = Machine.Specifications.ReSharperRunner.Shims.ICache;

namespace Machine.Specifications.ReSharperRunner.Factories
{
  [SolutionComponent]
  public class BehaviorFactory
  {
    readonly ElementCache _cache;
    readonly IUnitTestElementManager _manager;
    readonly ICache _cacheManager;
    readonly MSpecUnitTestProvider _provider;
    readonly IPsi _psiModuleManager;
    readonly ReflectionTypeNameCache _reflectionTypeNameCache = new ReflectionTypeNameCache();

     public BehaviorFactory(MSpecUnitTestProvider provider,
                          IUnitTestElementManager manager,
                          IPsi psiModuleManager,
                          ICache cacheManager,
                          ElementCache cache)
    {
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
      _provider = provider;
      _cache = cache;
      _manager = manager;
    }

    public BehaviorElement CreateBehavior(IDeclaredElement field)
    {
      var clazz = ((ITypeMember) field).GetContainingType() as IClass;
      if (clazz == null)
      {
        return null;
      }

      var context = _cache.TryGetContext(clazz);
      if (context == null)
      {
        return null;
      }

      var fieldType = new NormalizedTypeName(field as ITypeOwner);

      var behavior = GetOrCreateBehavior(context,
                                         clazz.GetClrName(),
                                         field.ShortName,
                                         field.IsIgnored(),
                                         fieldType);

      foreach (var child in behavior.Children)
      {
        child.State = UnitTestElementState.Pending;
      }

      _cache.AddBehavior(field, behavior);
      return behavior;
    }

    public BehaviorElement CreateBehavior(ContextElement context, IMetadataField behavior)
    {
      var typeContainingBehaviorSpecifications = behavior.GetFirstGenericArgument();

      var metadataTypeName = behavior.FirstGenericArgumentClass().FullyQualifiedName();
      var fieldType = new NormalizedTypeName(new ClrTypeName(metadataTypeName));

      var behaviorElement = GetOrCreateBehavior(context,
                                                _reflectionTypeNameCache.GetClrName(behavior.DeclaringType),
                                                behavior.Name,
                                                behavior.IsIgnored() || typeContainingBehaviorSpecifications.IsIgnored(),
                                                fieldType);

      return behaviorElement;
    }

    public BehaviorElement GetOrCreateBehavior(ContextElement context,
                                               IClrTypeName declaringTypeName,
                                               string fieldName,
                                               bool isIgnored,
                                               string fieldType)
    {
      var id = BehaviorElement.CreateId(context, fieldType, fieldName);
      var behavior = _manager.GetElementById(context.GetProject(), id) as BehaviorElement;
      if (behavior != null)
      {
        behavior.Parent = context;
        behavior.State = UnitTestElementState.Valid;
        return behavior;
      }

      return new BehaviorElement(_provider,
                                 _psiModuleManager,
                                 _cacheManager,
                                 context,
                                 new ProjectModelElementEnvoy(context.GetProject()),
                                 declaringTypeName,
                                 fieldName,
                                 isIgnored,
                                 fieldType);
    }

    public void UpdateChildState(IDeclaredElement field)
    {
      var behavior = _cache.TryGetBehavior(field);
      if (behavior == null)
      {
        return;
      }

      foreach (var element in behavior
        .Children.Where(x => x.State == UnitTestElementState.Pending)
        .Flatten(x => x.Children))
      {
        element.State = UnitTestElementState.Invalid;
      }
    }
  }
}