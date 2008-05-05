using System;

namespace Machine.MsMvc
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ViewEngineAttribute : Attribute
  {
    private readonly Type _type;

    public Type ViewEngineType
    {
      get { return _type; }
    }

    public ViewEngineAttribute(Type type)
    {
      _type = type;
    }
  }
}