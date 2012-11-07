using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;

namespace Machine.Specifications.Reporting.Specs.Visitors
{
  [Subject(typeof(NotImplementedSpecificationLinker))]
  public class when_unimplemented_specifications_are_linked : ReportSpecs
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

        First = Spec("it", "a 1 c 1 c 1 specification 1", Result.NotImplemented());
        Second = Spec("it", "a 2 c 1 c 1 specification 1", Result.NotImplemented());
        Last = Spec("it", "a 2 c 1 c 2 specification 1", Result.NotImplemented());

        Report = Run(Assembly("assembly 1",
                              Concern("a 1 concern 1",
                                      Context("a 1 c 1 context 1",
                                              First,
                                              Spec("it", "a 1 c 1 c 1 specification 2", Result.Pass())
                                        )
                                )
                       ),
                     Assembly("assembly 2",
                              Concern("a 2 concern 1",
                                      Context("a 2 c 1 context 1",
                                              Spec("it", "a 2 c 1 c 1 specification 2", Result.Pass()),
                                              Second),
                                      Context("a 2 c 1 context 2",
                                              Last,
                                              Spec("it", "a 2 c 1 c 2 specification 2", Result.Pass()))))
          );
      };

    Because of = () => Linker.Visit(Report);

    It should_not_assign_a__previous__link_to_the_report =
      () => Report.PreviousNotImplemented.ShouldBeNull();

    It should_assign_a__next__link_to_the_report =
      () => Report.NextNotImplemented.ShouldEqual(First);

    It should_assign_a__next__link_to_the_first_not_implemented_spec =
      () => First.NextNotImplemented.ShouldEqual(Second);

    It should_not_assign_a__previous__link_to_the_first_not_implemented_spec =
      () => First.PreviousNotImplemented.ShouldBeNull();

    It should_assign_a__next__link_to_the_second_not_implemented_spec =
      () => Second.NextNotImplemented.ShouldEqual(Last);

    It should_assign_a__previous__link_to_the_second_not_implemented_spec =
      () => Second.PreviousNotImplemented.ShouldEqual(First);

    It should_not_assign_a__next__link_to_the_last_not_implemented_spec =
      () => Last.NextNotImplemented.ShouldBeNull();

    It should_assign_a__previous__link_to_the_last_not_implemented_spec =
      () => Last.PreviousNotImplemented.ShouldEqual(Second);
  }
}