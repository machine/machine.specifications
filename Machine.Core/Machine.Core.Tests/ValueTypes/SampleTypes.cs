using System;
using System.Collections.Generic;
using System.Text;

namespace Machine.Core.ValueTypes
{
  public class Message1
  {
  }
  public class Message2 : ClassTypeAsValueType
  {
    private string _name;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public Message2(string name)
    {
      _name = name;
    }
  }
  public class Message3
  {
    private string _name;
    private short _size;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public short Size
    {
      get { return _size; }
      set { _size = value; }
    }

    public Message3(string name, short size)
    {
      _name = name;
      _size = size;
    }
  }
  public class Message4
  {
    private string _name;
    private short _size;
    private Message2 _message2;

    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public short Size
    {
      get { return _size; }
      set { _size = value; }
    }

    public Message4(string name, short size, Message2 message2)
    {
      _name = name;
      _message2 = message2;
      _size = size;
    }
  }
  public enum YesNoMaybe
  {
    Yes = 0, No, Maybe
  }
  public class TypeWithABunchOfTypes
  {
    private readonly string _aString;
    private readonly long _aLong;
    private readonly short _aShort;
    private readonly bool _aBool;
    private readonly int _aInt;
    private readonly YesNoMaybe _aEnum;
    private readonly DateTime _aDateTime;

    public TypeWithABunchOfTypes(bool aBool, int aInt, long aLong, short aShort, string aString, YesNoMaybe aEnum, DateTime aDateTime)
    {
      _aBool = aBool;
      _aDateTime = aDateTime;
      _aInt = aInt;
      _aLong = aLong;
      _aShort = aShort;
      _aString = aString;
      _aEnum = aEnum;
    }
  }
  public class TypeWithOnlyEnum
  {
    private readonly YesNoMaybe _maybe;

    public TypeWithOnlyEnum(YesNoMaybe maybe)
    {
      _maybe = maybe;
    }
  }
}
