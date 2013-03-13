using System.Xml;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

using Machine.Specifications.ReSharperRunner.Presentation;

namespace Machine.Specifications.ReSharperRunner
{
  [SolutionComponent]
  public class MSpecUnitTestElementSerializer : IUnitTestElementSerializer
  {
    readonly CacheManager _cacheManager;
    readonly IUnitTestElementManager _manager;
    readonly MSpecUnitTestProvider _provider;
    readonly PsiModuleManager _psiModuleManager;
    readonly ISolution _solution;

    public MSpecUnitTestElementSerializer(ISolution solution,
                                          MSpecUnitTestProvider provider,
                                          IUnitTestElementManager manager,
                                          PsiModuleManager psiModuleManager,
                                          CacheManager cacheManager)
    {
      _solution = solution;
      _provider = provider;
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
    }

    public void SerializeElement(XmlElement parent, IUnitTestElement element)
    {
      var e = element as ISerializableElement;
      if (e != null)
      {
        e.WriteToXml(parent);
        parent.SetAttribute("elementType", e.GetType().Name);
      }
    }

    public IUnitTestElement DeserializeElement(XmlElement parent, IUnitTestElement parentElement)
    {
      var typeName = parent.GetAttribute("elementType");

      if (Equals(typeName, "ContextElement"))
      {
        return ContextElement.ReadFromXml(parent,
                                          parentElement,
                                          _provider,
                                          _solution,
                                          _manager,
                                          _psiModuleManager,
                                          _cacheManager);
      }
      if (Equals(typeName, "BehaviorElement"))
      {
        return BehaviorElement.ReadFromXml(parent,
                                           parentElement,
                                           _provider,
                                           _solution,
                                           _manager,
                                           _psiModuleManager,
                                           _cacheManager);
      }
      if (Equals(typeName, "BehaviorSpecificationElement"))
      {
        return BehaviorSpecificationElement.ReadFromXml(parent,
                                                        parentElement,
                                                        _provider,
                                                        _solution,
                                                        _manager,
                                                        _psiModuleManager,
                                                        _cacheManager);
      }
      if (Equals(typeName, "ContextSpecificationElement"))
      {
        return ContextSpecificationElement.ReadFromXml(parent,
                                                       parentElement,
                                                       _provider,
                                                       _solution,
                                                       _manager,
                                                       _psiModuleManager,
                                                       _cacheManager);
      }

      return null;
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}