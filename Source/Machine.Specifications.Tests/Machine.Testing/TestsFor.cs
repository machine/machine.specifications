using NUnit.Framework;

namespace Machine.Testing
{
  public abstract class TestsFor<TType> where TType : class, new()
  {
    TType _target;

    public TType Target
    {
      get
      {
        CreateTargetIfNeedBe();
        return _target;
      }
    }

    [SetUp]
    public void BaseSetup()
    {
      _target = null;
      BeforeEachTest();
    }

    void CreateTargetIfNeedBe()
    {
      if (_target != null) return;
      _target = new TType();
    }

    public virtual void BeforeEachTest()
    {
    }
  }
}