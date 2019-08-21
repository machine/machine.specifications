using System;
using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;

namespace Machine.Specifications.Reporting.Specs.Visitors
{
    [Subject(typeof(FailedSpecificationLinker))]
    public class when_failed_specifications_are_linked : ReportSpecs
    {
        static FailedSpecificationLinker Linker;
        static Run Report;
        static Specification Specification;
        static Specification First;
        static Specification Second;
        static Specification Last;

        Establish context = () =>
          {
              Linker = new FailedSpecificationLinker();

              First = Spec("it", "a 1 c 1 c 1 specification 1", Runner.Utility.Result.Failure(new Exception()));
              Second = Spec("it", "a 2 c 1 c 1 specification 1", Runner.Utility.Result.Failure(new Exception()));
              Last = Spec("it", "a 2 c 1 c 2 specification 1", Runner.Utility.Result.Failure(new Exception()));

              Report = Run(Assembly("assembly 1",
                                    Concern("a 1 concern 1",
                                            Context("a 1 c 1 context 1",
                                                    First,
                                                    Spec("it", "a 1 c 1 c 1 specification 2", Runner.Utility.Result.Pass())
                                              )
                                      )
                             ),
                           Assembly("assembly 2",
                                    Concern("a 2 concern 1",
                                            Context("a 2 c 1 context 1",
                                                    Spec("it", "a 2 c 1 c 1 specification 2", Runner.Utility.Result.Pass()),
                                                    Second),
                                            Context("a 2 c 1 context 2",
                                                    Last,
                                                    Spec("it", "a 2 c 1 c 2 specification 2", Runner.Utility.Result.Pass()))))
                );
          };

        Because of = () => Linker.Visit(Report);

        It should_not_assign_a__previous__link_to_the_report =
          () => Report.PreviousFailed.ShouldBeNull();

        It should_assign_a__next__link_to_the_report =
          () => Report.NextFailed.ShouldEqual(First);

        It should_assign_a__next__link_to_the_first_failed_spec =
          () => First.NextFailed.ShouldEqual(Second);

        It should_not_assign_a__previous__link_to_the_first_failed_spec =
          () => First.PreviousFailed.ShouldBeNull();

        It should_assign_a__next__link_to_the_second_failed_spec =
          () => Second.NextFailed.ShouldEqual(Last);

        It should_assign_a__previous__link_to_the_second_failed_spec =
          () => Second.PreviousFailed.ShouldEqual(First);

        It should_not_assign_a__next__link_to_the_last_failed_spec =
          () => Last.NextFailed.ShouldBeNull();

        It should_assign_a__previous__link_to_the_last_failed_spec =
          () => Last.PreviousFailed.ShouldEqual(Second);
    }
}
