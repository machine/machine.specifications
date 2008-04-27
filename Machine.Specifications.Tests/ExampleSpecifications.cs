using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public class SpecificationWithThrowingRequirement
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
  }

  public class SpecificationWithSingleRequirement
  {
    public static bool WhenInvoked = false;
    public static bool ItInvoked = false;
    When it_happens = () =>
    {
      WhenInvoked = true;
    };

    It is_a_requirement = () =>
    {
      ItInvoked = true;
    };
  }
}
