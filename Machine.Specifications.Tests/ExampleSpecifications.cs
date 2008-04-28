using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public interface IFakeSpecification
  {
    void Reset();
  }

  public class SpecificationWithThrowingRequirement : IFakeSpecification
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

  public class SpecificationWithSingleRequirement : IFakeSpecification
  {
    public static bool WhenInvoked = false;
    public static bool ItInvoked = false;
    public static bool BeforeEachInvoked = false;
    public static bool BeforeAllInvoked = false;
    public static bool AfterEachInvoked = false;
    public static bool AfterAllInvoked = false;

    Before each =()=>
    {
      BeforeEachInvoked = true;
    };

    Before all =()=>
    {
      BeforeAllInvoked = true;
    };

    When it_happens = () =>
    {
      WhenInvoked = true;
    };

    It is_a_requirement = () =>
    {
      ItInvoked = true;
    };

    After _each =()=>
    {
      AfterEachInvoked = true;
    };

    After _all =()=>
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

  public class SpecificationWithBadlyNamedBefore : IFakeSpecification
  {
    Before foo =()=>
    {
      
    };

    public void Reset()
    {
    }
  }

  public class SpecificationWithBadlyNamedAfter : IFakeSpecification
  {
    After foo =()=>
    {
      
    };

    public void Reset()
    {
    }
  }
}
