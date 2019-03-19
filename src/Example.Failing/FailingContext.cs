using System;

using Machine.Specifications;

namespace Example.Failing
{
  [Subject("Scott Bellware")]
  public class at_any_given_moment
  {
    It will_fail = () => { throw new Exception("hi scott, love you, miss you."); };
  }

  [Tags("example")]
  public class context_with_multiple_establish_clauses
  {
    Establish foo = () => { };
    Establish bar = () => { };

    It should = () => { };
  }

  [SetupDelegate]
  public delegate void Given();

  [Tags("example")]
  public class context_with_multiple_given_clauses
  {
    Given foo = () => { };
    Given bar = () => { };

    It should = () => { };
  }
}