using System.Dynamic;
using Machine.Specifications;

namespace Example.Clr4
{
    public class When_specs_target_the_common_language_runtime_in_version_4
    {
        static ExpandoObject expando_object;

        Because of = () =>
            expando_object = new ExpandoObject();

        It should_be_able_to_use_components_that_are_available_in_the_target_framework = () =>
            expando_object.ShouldNotBeNull();
    }
}
