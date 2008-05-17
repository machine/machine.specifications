#pragma warning disable 169
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

  public interface IFakeDescription
  {
    void Reset();
  }

  public class DescriptionWithThrowingWhenAndPassingSpecification : IFakeDescription
  {
    public static bool ItInvoked;

    When throws =()=>
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

  public class DescriptionWithEmptyWhen : IFakeDescription
  {
    public static bool ItInvoked = false;

    When nothing_happens;

    It should_do_stuff =()=>
    {
      ItInvoked = true;
    };

    public void Reset()
    {
      ItInvoked = false;
    }
  }

  public class DescriptionWithTwoWhens : IFakeDescription
  {
    public static bool When1Invoked = false;
    public static bool When2Invoked = false;
    public static bool ItForWhen1Invoked = false;
    public static bool ItForWhen2Invoked = false;

    When _1 =()=>
    {
      When1Invoked = true;
    };

    It for_when_1 =()=>
    {
      ItForWhen1Invoked = true;
    };

    When _2 =()=>
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

  public class DescriptionWithEmptySpecification : IFakeDescription
  {
    public static bool WhenInvoked = false;

    When not_called =()=>
    {
      WhenInvoked = true;
    };

    It should_do_stuff;

    public void Reset()
    {
      WhenInvoked = false;
    }
  }

  public class DescriptionWithThrowingSpecification : IFakeDescription
  {
    public static bool WhenInvoked = false;
    public static bool ItInvoked = false;
    When it_happens = () =>
    {
      WhenInvoked = true;
      throw new Exception();
    };

    It_should_throw an_exception = x =>
    {
      ItInvoked = true;
    };

    public void Reset()
    {
      WhenInvoked = false;
      ItInvoked = false;
    }
  }

  public class DescriptionWithSingleSpecification : IFakeDescription
  {
    public static bool WhenInvoked = false;
    public static bool ItInvoked = false;
    public static bool BeforeEachInvoked = false;
    public static bool BeforeAllInvoked = false;
    public static bool AfterEachInvoked = false;
    public static bool AfterAllInvoked = false;

    Context before_each =()=>
    {
      BeforeEachInvoked = true;
    };

    Context before_all =()=>
    {
      BeforeAllInvoked = true;
    };

    When it_happens = () =>
    {
      WhenInvoked = true;
    };

    It is_a_specification = () =>
    {
      ItInvoked = true;
    };

    Context after_each =()=>
    {
      AfterEachInvoked = true;
    };

    Context after_all =()=>
    {
      AfterAllInvoked = true;
    };

    public void Reset()
    {
      WhenInvoked = false;
      ItInvoked = false;
      BeforeEachInvoked = false;
      BeforeAllInvoked = false;
      AfterEachInvoked = false;
      AfterAllInvoked = false;
    }
  }

  public class DescriptionWithBadlyNamedBefore : IFakeDescription
  {
    Context foo =()=>
    {
      
    };

    public void Reset()
    {
    }
  }

  public class DescriptionWithBadlyNamedAfter : IFakeDescription
  {
    Context foo =()=>
    {
      
    };

    public void Reset()
    {
    }
  }
}
