using Machine.Specifications;

namespace Example.Random
{
  [Subject("Context that inherits")]
  [Tags(tag.example)]
  public abstract class context_that_inherits
  {
    public static int BaseEstablishRunCount;

    Establish context = () => BaseEstablishRunCount++;

    protected It should_be_inherited_but_not_executed = () => { };
    It should_not_be_executed = () => { };
  }

  public class context_with_inherited_specifications : context_that_inherits
  {
    public static int BecauseClauseRunCount;
    public static int EstablishRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }

  [SetupForEachSpecification]
  public class context_with_inherited_specifications_and_setup_for_each : context_that_inherits
  {
    public static int BecauseClauseRunCount;
    public static int EstablishRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }
}