using System;
using System.Collections.Generic;

namespace Machine.Migrations
{
  public class VersionState
  {
    private readonly short _current;
    private readonly short _last;
    private readonly short _desired;

    public short Current
    {
      get { return _current; }
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
      get { return _desired < _current; }
    }

    public VersionState(short current, short last, short desired)
    {
      _current = current;
      _last = last;
      _desired = desired;
    }

    public bool IsApplicable(MigrationReference migrationReference)
    {
      short start = _current;
      short end = _desired;
      if (start > end)
      {
        start = _desired;
        end = _current;
      }
      return migrationReference.Version > start && migrationReference.Version <= end;
    }
  }
}
