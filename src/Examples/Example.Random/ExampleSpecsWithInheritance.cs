using Machine.Specifications;

namespace Example.Random
{
    [Subject("Context that inherits")]
    [Tags(tag.example)]
    public abstract class context_that_inherits
    {
        public static int base_establish_run_count;

        Establish context = () =>
            base_establish_run_count++;

        protected It should_be_inherited_but_not_executed = () => { };

        It should_not_be_executed = () => { };
    }

    public class context_with_inherited_specifications : context_that_inherits
    {
        public static int because_clause_run_count;
        public static int establish_run_count;

        Establish context = () =>
            establish_run_count++;

        Because of = () =>
            because_clause_run_count++;

        It spec1 = () => { };

        It spec2 = () => { };
    }

    [SetupForEachSpecification]
    public class context_with_inherited_specifications_and_setup_for_each : context_that_inherits
    {
        public static int because_clause_run_count;
        public static int establish_run_count;

        Establish context = () =>
            establish_run_count++;

        Because of = () =>
            because_clause_run_count++;

        It spec1 = () => { };

        It spec2 = () => { };
    }
}
