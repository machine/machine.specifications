using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using Machine.Testing.AutoMocking;

using NUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Impl;
using Rhino.Mocks.Interfaces;

namespace Machine.Testing
{
  public abstract class TestsFor<TType> where TType : class
  {
    AutoMockingContainer _container;
    TType _target;
    MockRepository _mocks;
    Dictionary<Type, object> _overrides;
    Dictionary<Type, IEventRaiser> _raisers;

    [ThreadStatic]
    static Dictionary<int, EventFireExpectation> _eventsToVerify =
      new Dictionary<int, EventFireExpectation>();

    [ThreadStatic]
    static int _currentEventId = 0;

    public enum With
    {
      Stub,
      StrictMock
    }

    public AutoMockingContainer Container
    {
      get { return _container; }
    }

    public TType Target
    {
      get
      {
        CreateTargetIfNeedBe();
        return _target;
      }
    }

    public MockRepository Mocks
    {
      get { return _mocks; }
    }

    public IDisposable Record
    {
      get { return Mocks.Record(); }
    }

    public IDisposable Playback
    {
      get
      {
        CreateTargetIfNeedBe();
        return new PlaybackAndVerifyEvents(this, Mocks.Playback());
      }
    }

    public IDisposable PlaybackOnly
    {
      get
      {
        using (Record)
        {
        }
        return Playback;
      }
    }

    class EventFireExpectation
    {
      bool _wasFired;
      string _delegateName;

      public bool WasFired
      {
        get { return _wasFired; }
        set { _wasFired = value; }
      }

      public string DelegateName
      {
        get { return _delegateName; }
      }

      public EventFireExpectation(string delegateName)
      {
        _wasFired = false;
        _delegateName = delegateName;
      }
    }

    class PlaybackAndVerifyEvents : IDisposable
    {
      readonly TestsFor<TType> _tests;
      readonly IDisposable _playback;

      public PlaybackAndVerifyEvents(TestsFor<TType> tests, IDisposable playback)
      {
        _tests = tests;
        _playback = playback;
      }

      public void Dispose()
      {
        _playback.Dispose();
        _tests.VerifyEvents();
      }
    }

    public void VerifyEvents()
    {
      foreach (var pair in _eventsToVerify)
      {
        Assert.IsTrue(pair.Value.WasFired, String.Format("{0} was not fired.", pair.Value.DelegateName));
      }
    }

    [SetUp]
    public void BaseSetup()
    {
      _overrides = new Dictionary<Type, object>();
      _raisers = new Dictionary<Type, IEventRaiser>();
      _mocks = new MockRepository();
      _container = new AutoMockingContainer(_mocks);
      _eventsToVerify.Clear();
      _currentEventId = 0;

      SetupContainer();
      _container.Initialize();
      _container.PrepareForServices();
      _container.Start();

      _target = null;
      BeforeEachTest();
    }

    void CreateTargetIfNeedBe()
    {
      if (_target != null) return;
      _target = _container.Resolve.New<TType>(new List<object>(_overrides.Values).ToArray());
    }

    public virtual void SetupContainer()
    {
    }

    public virtual void BeforeEachTest()
    {
    }

    public void Override<T>(With what)
    {
      switch (what)
      {
        case With.Stub:
          Override(Mocks.Stub<T>());
          break;
        case With.StrictMock:
          Override(Mocks.StrictMock<T>());
          break;
        default:
          throw new Exception("Unknown With");
      }
    }

    public void Override<T>(T service)
    {
      _overrides.Add(typeof(T), service);
    }

    public T The<T>()
    {
      if (_overrides.ContainsKey(typeof(T)))
      {
        return (T)_overrides[typeof(T)];
      }
      return _container.Get<T>();
    }

    public void FireEventOn<T>(params object[] args)
    {
      if (!_raisers.ContainsKey(typeof(T)))
      {
        throw new Exception(
          String.Format("You must first call PrimeEventFiringOn<{0}>(x => x.Event += null) in the Record phase",
          typeof(T).Name));
      }
      List<object> list = new List<object>(args);
      list.Insert(0, The<T>());
      _raisers[typeof(T)].Raise(list.ToArray());
    }

    public void PrimeEventFiringOn<T>(Action<T> eventPlusEqualNull)
    {
      if (_raisers.ContainsKey(typeof(T)))
      {
        throw new Exception(
          String.Format(
          "You can only prime one event at a time for {0}. If you must prime multiple, try using LastCall.GetEventRaiser the normal way.",
          typeof(T).Name));
      }

      eventPlusEqualNull(The<T>());
      _raisers[typeof(T)] = LastCall.GetEventRaiser();
    }

    static void RegisterEventFiring(int eventId)
    {
      _eventsToVerify[eventId].WasFired = true;
    }

    public T NewEventFireExpectation<T>() where T : class
    {
      _eventsToVerify[_currentEventId] = new EventFireExpectation(typeof(T).Name);

      MethodInfo registerMethod = typeof(TestsFor<TType>).GetMethod("RegisterEventFiring",
        BindingFlags.Static | BindingFlags.NonPublic);
      MethodInfo invokeMethod = typeof(T).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
      ParameterInfo[] parameters = invokeMethod.GetParameters();
      Type[] parameterTypes = new Type[parameters.Length];
      for (int i = 0; i < parameters.Length; ++i)
      {
        parameterTypes[i] = parameters[i].ParameterType;
      }

      if (invokeMethod == null)
      {
        throw new Exception(String.Format("Cannot find Invoke method on {0}, is it a Delegate?", typeof(T).Name));
      }

      DynamicMethod method = new DynamicMethod("__FireEvent" + _currentEventId, invokeMethod.ReturnType, parameterTypes,
        this.GetType(), true);

      ILGenerator ilGenerator = method.GetILGenerator();
      ilGenerator.Emit(OpCodes.Ldc_I4, _currentEventId++);
      ilGenerator.Emit(OpCodes.Call, registerMethod);
      ilGenerator.Emit(OpCodes.Ret);

      return method.CreateDelegate(typeof(T)) as T;
    }
  }
}