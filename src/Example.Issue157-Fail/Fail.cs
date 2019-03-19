using System;

using Machine.Specifications;

namespace Example.Issue157_Fail
{
  public class when_a_spec_fails
  {
    It will_fail = () => { throw new Exception(); };
  }
}