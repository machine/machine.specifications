using System;

using FluentAssertions;

namespace Machine.Specifications
{
  namespace ExampleA
  {
    namespace ExampleB
    {
      public class InExampleB_1
      {
        It is_spec_1 =()=> { };
        
      }
        
      public class InExampleB_2
      {
        It is_spec_1 =()=> { };
        
      }
        
    }

    public class InExampleA_1
    {
      It is_spec_1 =()=> { };
      
    }

    public class InExampleA_2
    {
      It is_spec_1 =()=> { };
    }
  }

  namespace ExampleC
  {
    public class InExampleC_1
    {
      It is_spec_1 =()=> { };
      It is_spec_2 =()=> { };
    }

    public class InExampleC_2
    {
      
    }
  }

  public interface IFakeContext
  {
    void Reset();
  }

  public class ContextWithSpecificationExpectingThrowThatDoesnt : IFakeContext
  {
    public static bool ItInvoked;
    static Exception exception;

    Because of =()=>
      exception = null;

    It should_throw_but_it_wont =()=> 
      exception.Should().NotBeNull();

    public void Reset()
    {
      ItInvoked = false;
    }
  }

  public class ContextWithThrowingWhenAndPassingSpecification : IFakeContext
  {
    public static bool ItInvoked;

    Because of =()=>
    {
      throw new Exception();
    };

    It should_fail =()=>
    {
      ItInvoked = true;
    };

    public void Reset()
    {
      ItInvoked = false;
    }
  }

  public class ContextWithEmptyWhen : IFakeContext
  {
    public static bool ItInvoked = false;

    Because of;

    It should_do_stuff =()=>
    {
      ItInvoked = true;
    };

    public void Reset()
    {
      ItInvoked = false;
    }
  }

  public class ContextWithTwoWhens : IFakeContext
  {
    public static bool When1Invoked = false;
    public static bool When2Invoked = false;
    public static bool ItForWhen1Invoked = false;
    public static bool ItForWhen2Invoked = false;

    Because _1 =()=>
    {
      When1Invoked = true;
    };

    It for_when_1 =()=>
    {
      ItForWhen1Invoked = true;
    };

    Because _2 =()=>
    {
      When2Invoked = true;
    };

    It for_when_2 =()=>
    {
      ItForWhen2Invoked = true;
    };

    public void Reset()
    {
      When1Invoked = false;
      When2Invoked = false;
      ItForWhen1Invoked = false;
      ItForWhen2Invoked = false;
    }
  }

  public class ContextWithEmptySpecification : IFakeContext
  {
    public static bool WhenInvoked = false;

    Because of =()=>
    {
      WhenInvoked = true;
    };

    It should_do_stuff;

    public void Reset()
    {
      WhenInvoked = false;
    }
  }

  public class ContextWithThrowingSpecification : IFakeContext
  {
    public static bool WhenInvoked = false;
    public static bool ItInvoked = false;
    public static Exception exception;
    Because it_happens = () =>
    {
      WhenInvoked = true;
      exception = Catch.Exception(() => { throw new Exception(); });
    };

    It should_throw_an_exception = () =>
    {
      ItInvoked = true;
    };

    public void Reset()
    {
      WhenInvoked = false;
      ItInvoked = false;
    }
  }

  public class ContextWithSingleSpecification : IFakeContext
  {
    public static bool BecauseInvoked = false;
    public static bool ItInvoked = false;
    public static bool ContextInvoked = false;
    public static bool CleanupInvoked = false;

    Establish context =()=>
    {
      ContextInvoked = true;
    };

    Because of = () =>
    {
      BecauseInvoked = true;
    };

    It is_a_specification = () =>
    {
      ItInvoked = true;
    };

    Cleanup after =()=>
    {
      CleanupInvoked = true;
    };

    public void Reset()
    {
      BecauseInvoked = false;
      ItInvoked = false;
      ContextInvoked = false;
      CleanupInvoked = false;
    }
  }

  public class ContextWithBadlyNamedBefore : IFakeContext
  {
    Establish foo =()=>
    {
      
    };

    public void Reset()
    {
    }
  }

  public class ContextWithBadlyNamedAfter : IFakeContext
  {
    Establish foo =()=>
    {
      
    };

    public void Reset()
    {
    }
  }
}
