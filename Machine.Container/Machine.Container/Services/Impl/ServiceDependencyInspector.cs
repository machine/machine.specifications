using System;
using System.Collections.Generic;
using System.Reflection;

using Machine.Container.Model;

namespace Machine.Container.Services.Impl
{
  public class ServiceDependencyInspector : IServiceDependencyInspector
  {
    #region IServiceDependencyInspector
    public ConstructorCandidate SelectConstructor(Type type)
    {
      if (type.IsAbstract)
      {
        throw new InvalidOperationException("Can't resolve dependencies on Abstract type: " + type);
      }
      List<ConstructorCandidate> candidates = DetermineCandidates(type);
      ConstructorCandidate selected = null;
      foreach (ConstructorCandidate candidate in candidates)
      {
        if (selected == null)
        {
          selected = candidate;
        }
        else
        {
          if (selected.Dependencies.Count == candidate.Dependencies.Count)
          {
            throw new InvalidOperationException();
          }
          if (selected.Dependencies.Count > candidate.Dependencies.Count)
          {
            selected = candidate;
          }
        }
      }
      if (selected == null)
      {
        throw new InvalidOperationException("No constructor candidate found: " + type);
      }
      return selected;
    }
    #endregion

    #region Protected Methods
    protected virtual List<ConstructorCandidate> DetermineCandidates(Type type)
    {
      List<ConstructorCandidate> candidates = new List<ConstructorCandidate>();
      foreach (ConstructorInfo ctor in type.GetConstructors())
      {
        ConstructorCandidate candidate = new ConstructorCandidate(ctor);
        foreach (ParameterInfo parameter in ctor.GetParameters())
        {
          candidate.Dependencies.Add(new ServiceDependency(parameter.ParameterType, DependencyType.Constructor));
        }
        candidates.Add(candidate);
      }
      return candidates;
    }
    #endregion
  }
}