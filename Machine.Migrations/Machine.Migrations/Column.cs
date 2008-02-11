using System;
using System.Collections.Generic;

namespace Machine.Migrations
{
  public class Column
  {
    private string _name;
    private Type _type;
    private short _size;
    private bool _isPrimaryKey;
    private bool _allowNull;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public Type Type
    {
      get { return _type; }
      set { _type = value; }
    }

    public short Size
    {
      get { return _size; }
      set { _size = value; }
    }

    public bool IsPrimaryKey
    {
      get { return _isPrimaryKey; }
      set { _isPrimaryKey = value; }
    }

    public bool AllowNull
    {
      get { return _allowNull; }
      set { _allowNull = value; }
    }

    public Column()
    {
    }

    public Column(string name, Type type)
     : this(name, type, false)
    {
    }

    public Column(string name, Type type, bool allowNull)
     : this(name, type, 0)
    {
      if (type == typeof(Int16))
      {
        _size = 2;
      }
      else if (type == typeof(Int32))
      {
        _size = 4;
      }
      else if (type == typeof(Int64))
      {
        _size = 8;
      }
    }

    public Column(string name, Type type, short size)
     : this(name, type, size, false)
    {
    }

    public Column(string name, Type type, short size, bool isPrimaryKey)
     : this(name, type, size, isPrimaryKey, false)
    {
    }

    public Column(string name, Type type, short size, bool isPrimaryKey, bool allowNull)
    {
      _name = name;
      _type = type;
      _size = size;
      _isPrimaryKey = isPrimaryKey;
      _allowNull = allowNull;
    }
  }
}
