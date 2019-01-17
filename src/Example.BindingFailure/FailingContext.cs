using Example.BindingFailure.Ref;

using Machine.Specifications;

namespace Example.BindingFailure
{
  [Subject("Assembly binding failure")]
  public class if_a_referenced_assembly_cannot_be_bound
  {
    Referenced _referenced;

    It will_fail = ()=> { };
  }
}