using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using ObjectFactories.Services;
using ObjectFactories.Services.Impl;

namespace ObjectFactories.MsBuild
{
  public class FactoryWeaverTask : Task
  {
    #region Member Data
    private string _primaryAssembly;
    private string[] _assemblies;
    #endregion

    #region Properties
    [Required]
    public string PrimaryAssembly
    {
      get { return _primaryAssembly; }
      set { _primaryAssembly = value; }
    }

    [Required]
    public string[] Assemblies
    {
      get { return _assemblies; }
      set { _assemblies = value; }
    }
    #endregion

    #region Private Methods
    public override bool Execute()
    {
      ILog log = new MsBuildLogger(this.Log);
      IAssemblies assemblies = new Assemblies();
      IFactoryFinder factoryFinder = new FactoryFinder(log);
      IConstructorCallFinder constructorCallFinder = new ConstructorCallFinder(log);
      IFactoryCallWeaver factoryCallWeaver = new FactoryCallWeaver(log);
      IFactoryMapSerializer factoryMapSerializer = new FactoryMapSerializer();
      IObjectFactoriesProcessor processor = new ObjectFactoriesProcessor(log, assemblies, factoryFinder, constructorCallFinder, factoryCallWeaver, factoryMapSerializer);
      foreach (string path in this.Assemblies)
      {
        this.Log.LogMessage("Processing {0}", path);
        processor.ProcessAssembly(path);
      }
      return true;
    }
    #endregion
  }
}