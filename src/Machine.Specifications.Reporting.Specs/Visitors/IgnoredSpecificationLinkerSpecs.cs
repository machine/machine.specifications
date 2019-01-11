using FluentAssertions;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;

namespace Machine.Specifications.Reporting.Specs.Visitors
{
    [Subject(typeof(IgnoredSpecificationLinker))]
    public class when_ignored_specifications_are_linked : ReportSpecs
    {
        static IgnoredSpecificationLinker Linker;
        static Run Report;
        static Specification Specification;
        static Specification First;
        static Specification Second;
        static Specification Last;

        Establish context = () =>
          {
              Linker = new IgnoredSpecificationLinker();

              First = Spec("it", "a 1 c 1 c 1 specification 1", Runner.Utility.Result.Ignored());
              Second = Spec("it", "a 2 c 1 c 1 specification 1", Runner.Utility.Result.Ignored());
              Last = Spec("it", "a 2 c 1 c 2 specification 1", Runner.Utility.Result.Ignored());

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
          () => Report.PreviousIgnored.Should().BeNull();

        It should_assign_a__next__link_to_the_report =
          () => Report.NextIgnored.Should().Be(First);

        It should_assign_a__next__link_to_the_first_ignored_spec =
          () => First.NextIgnored.Should().Be(Second);

        It should_not_assign_a__previous__link_to_the_first_ignored_spec =
          () => First.PreviousIgnored.Should().BeNull();

        It should_assign_a__next__link_to_the_second_ignored_spec =
          () => Second.NextIgnored.Should().Be(Last);

        It should_assign_a__previous__link_to_the_second_ignored_spec =
          () => Second.PreviousIgnored.Should().Be(First);

        It should_not_assign_a__next__link_to_the_last_ignored_spec =
          () => Last.NextIgnored.Should().BeNull();

        It should_assign_a__previous__link_to_the_last_ignored_spec =
          () => Last.PreviousIgnored.Should().Be(Second);
    }
}