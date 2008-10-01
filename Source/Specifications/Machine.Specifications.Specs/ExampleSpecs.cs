using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Specs
{
  [SetupForEachSpecification, Tags(tag.example)]
  public class context_with_multiple_specifications_and_setup_for_each
  {
    public static int EstablishRunCount;
    public static int BecauseClauseRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }

  [Tags(tag.example)]
  public class context_with_multiple_specifications
  {
    public static int EstablishRunCount;
    public static int BecauseClauseRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }

  [Tags(tag.example, tag.example)]
  [Tags(tag.example)]
  public class context_with_duplicate_tags 
  {
    It bla_bla = ()=> { };
  }

  [Tags(tag.example, tag.some_other_tag, tag.one_more_tag)]
  public class context_with_tags 
  {
    It bla_bla = ()=> { };
  }

  [Ignore]
  public class context_with_ignore : context_with_no_specs
  {
    public static bool IgnoredSpecRan;

    It should_be_ignored = () =>
      IgnoredSpecRan = true;
  }

  public class context_with_ignore_on_one_spec : context_with_no_specs
  {
    public static bool IgnoredSpecRan;

    [Ignore]
    It should_be_ignored = () =>
      IgnoredSpecRan = true;
  }


  [Tags(tag.example)]
  public class context_with_no_specs 
  {
    public static bool ContextEstablished;
    public static bool OneTimeContextEstablished;
    public static bool CleanupOccurred;
    public static bool CleanupOnceOccurred;

    Establish context = () =>
    {
      ContextEstablished = true;
    };

    Establish context_once = () =>
    {
      ContextEstablished = true;
    };

    Cleanup after_each = () =>
    {
      CleanupOccurred = true;
    };

    Cleanup after_all = () =>
    {
      CleanupOnceOccurred = true;
    };
  }

  [Subject(typeof(int), "Some description")]
  [Tags(tag.example)]
  public class context_with_subject
  {
    public void Reset()
    {
    }
  }

  [Tags(tag.example)]
  public class context_with_misnamed_establish_clause
  {
    Establish foo = () => { };
  }
}