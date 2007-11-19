using System;

using ObjectFactories.Services;

namespace ObjectFactories
{
  public class MyObject { }
  public class MyObjectFactory : IObjectFactory<MyObject>
  {
    #region IObjectFactory<MyObject> Members
    public MyObject Create()
    {
      return new MyObject();
    }
    #endregion
  }

  public class SampleProgram
  {
    public static void Main(string[] args)
    {
      MyObject obj1 = new MyObject();
      Console.WriteLine(obj1);
    }
  }
}

