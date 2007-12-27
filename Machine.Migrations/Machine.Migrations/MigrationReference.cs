using System;
using System.Collections.Generic;

namespace Machine.Migrations
{
  public class MigrationReference
  {
    private short _version;
    private string _name;
    private string _path;
    private IDatabaseMigration _databaseMigration;

    public short Version
    {
      get { return _version; }
      set { _version = value; }
    }

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public string Path
    {
      get { return _path; }
      set { _path = value; }
    }

    public IDatabaseMigration DatabaseMigration
    {
      get { return _databaseMigration; }
      set { _databaseMigration = value; }
    }

    public MigrationReference()
    {
    }

    public MigrationReference(short version, string name, string path)
    {
      _version = version;
      _name = name;
      _path = path;
    }

    public override string ToString()
    {
      return String.Format("Migration<{0}, {1}>", _version, _name);
    }
  }
}
