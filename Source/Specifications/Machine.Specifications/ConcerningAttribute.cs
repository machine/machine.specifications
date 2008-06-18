using System;

namespace Machine.Specifications
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class ConcerningAttribute : Attribute
  {
    readonly Type _typeConcernedWith;
    readonly string _concern;

    public Type TypeConcernedWith
    {
      get { return _typeConcernedWith; }
    }

    public string SpecificConcern
    {
      get { return _concern; }
    }

    public ConcerningAttribute(Type typeConcernedWith)
    {
      this._typeConcernedWith = typeConcernedWith;
    }

    public ConcerningAttribute(Type typeConcernedWith, string concern)
    {
      _typeConcernedWith = typeConcernedWith;
      _concern = concern;
    }

    public ConcerningAttribute(string concern)
    {
      _concern = concern;
    }
  }
}