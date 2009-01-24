namespace Machine.Specifications.Specs
{
  public abstract class context_that_inherits
  {
    protected It should_be_inherited = () => { };
    It should_not_be_inherited = () => { };
  }

  [Tags(tag.example)]
  public class context_with_multiple_inherited_specifications : context_that_inherits
  {
    public static int BecauseClauseRunCount;
    public static int EstablishRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }

  [SetupForEachSpecification]
  [Tags(tag.example)]
  public class context_with_multiple_inherited_specifications_and_setup_for_each : context_that_inherits
  {
    public static int BecauseClauseRunCount;
    public static int EstablishRunCount;

    Establish context = () => EstablishRunCount++;

    Because of = () => BecauseClauseRunCount++;

    It spec1 = () => { };
    It spec2 = () => { };
  }
}