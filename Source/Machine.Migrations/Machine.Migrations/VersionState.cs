using System;
using System.Collections.Generic;

namespace Machine.Migrations
{
  public class VersionState
  {
    private readonly IList<short> _applied;
    private readonly short _last;
    private readonly short _desired;

    public IList<short> Applied
    {
      get { return _applied; }
    }

    public short Last
    {
      get { return _last; }
    }

    public short Desired
    {
      get { return _desired; }
    }

    public bool IsReverting
    {
      get
      {
        foreach (short applied in _applied)
        {
          if (_desired < applied)
          {
            return true;
          }
        }
        return false;
      }
    }

    public VersionState(short last, short desired, IList<short> applied)
    {
      _applied = applied;
      _last = last;
      _desired = desired;
    }

    public bool IsApplicable(MigrationReference migrationReference)
    {
      bool isApplied = _applied.Contains(migrationReference.Version);
      if (isApplied)
      {
        if (migrationReference.Version > _desired)
        {
          return true;
        }
        return false;
      }
      else
      {
        if (migrationReference.Version <= _desired)
        {
          return true;
        }
        return false;
      }
    }
  }
}
