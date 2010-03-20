using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.GallioAdapter.TestResources
{ 
  [Subject("Scott Bellware")]
  public class at_any_given_moment
  {
    It will_fail = () =>
    {
      throw new Exception("hi scott, love you, miss you.");
    };
  }

  [Tags("example")]
  public class failing_specification_assertions
  {
    It failing_boolean_assertion = () => false.ShouldBeTrue();
    It failing_equality_assertion = () => 1.ShouldEqual(2);
    It failing_contains_assertion = () => new int[] { 1, 2, 3, 5, }.ShouldContain(4);    
  }
}
