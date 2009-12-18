using System;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;

namespace Machine.Specifications.Reporting.Specs.Visitors
{
  [Subject(typeof(NotImplementedSpecificationLinker))]
  public class when_unimplemented_specifications_are_linked_by_id : ReportSpecs
  {
    static NotImplementedSpecificationLinker Linker;
    static Run Report;
    static Specification Specification;
    static Specification First;
    static Specification Second;
    static Specification Last;

    Establish context = () =>
      {
        Linker = new NotImplementedSpecificationLinker();

        First = Spec("a 1 c 1 c 1 specification 1", Result.NotImplemented());
        Second = Spec("a 2 c 1 c 1 specification 1", Result.NotImplemented());
        Last = Spec("a 2 c 1 c 2 specification 1", Result.NotImplemented());

        Report = Run(Assembly("assembly 1",
                              Concern("a 1 concern 1",
                                      Context("a 1 c 1 context 1",
                                              First,
                                              Spec("a 1 c 1 c 1 specification 2", Result.Pass())
                                        )
                                )
                       ),
                     Assembly("assembly 2",
                              Concern("a 2 concern 1",
                                      Context("a 2 c 1 context 1",
                                              Spec("a 2 c 1 c 1 specification 2", Result.Pass()),
                                              Second),
                                      Context("a 2 c 1 context 2",
                                              Last,
                                              Spec("a 2 c 1 c 2 specification 2", Result.Pass()))))
          );

        new SpecificationIdGenerator().Visit(Report);
      };

    Because of = () => Linker.Visit(Report);

    It should_assign_a__next__link_to_the_first_unimplemented_spec =
      () => First.Metadata[NotImplementedSpecificationLinker.Next].ShouldBeOfType<Guid>();

    It should_not_assign_a__previous__link_to_the_first_unimplemented_spec =
      () => First.Metadata[NotImplementedSpecificationLinker.Previous].ShouldBeNull();

    It should_assign_a__next__link_to_the_second_unimplemented_spec =
      () => Second.Metadata[NotImplementedSpecificationLinker.Next].ShouldBeOfType<Guid>();

    It should_assign_a__previous__link_to_the_second_unimplemented_spec =
      () => Second.Metadata[NotImplementedSpecificationLinker.Previous].ShouldBeOfType<Guid>();

    It should_not_assign_a__next__link_to_the_last_unimplemented_spec =
      () => Last.Metadata[NotImplementedSpecificationLinker.Next].ShouldBeNull();

    It should_assign_a__previous__link_to_the_last_unimplemented_spec =
      () => Last.Metadata[NotImplementedSpecificationLinker.Previous].ShouldBeOfType<Guid>();
  }
}