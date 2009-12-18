using System;
using System.Linq;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Specs.Visitors
{
  [Subject(typeof(SpecificationIdGenerator))]
  public class when_specification_ids_are_generated : ReportSpecs
  {
    static SpecificationIdGenerator Generator;
    static Run Report;

    Establish context = () =>
      {
        Generator = new SpecificationIdGenerator();

        Report = Run(Assembly("assembly 2",
                              Concern("a 2 concern 2",
                                      Context("a 2 c 2 context 2",
                                              Spec("a 2 c 2 c 2 specification 2", Result.Pass()),
                                              Spec("a 2 c 2 c 2 specification 1", Result.Failure(new Exception()))
                                        ),
                                      Context("a 2 c 2 context 1",
                                              Spec("a 2 c 2 c 1 specification 2", Result.Pass()),
                                              Spec("a 2 c 2 c 1 specification 1", Result.Failure(new Exception()))
                                        )
                                )),
                     Assembly("assembly 1",
                              Concern("a 1 concern 2",
                                      Context("a 1 c 2 context 2",
                                              Spec("a 1 c 2 c 2 specification 2", Result.Pass()),
                                              Spec("a 1 c 2 c 2 specification 1", Result.Failure(new Exception()))
                                        )
                                )
                       )
          );
      };

    Because of = () => Generator.Visit(Report);

    It should_assign_unique_ids_to_all_specifications =
      () => Report.Assemblies
              .SelectMany(x => x.Concerns)
              .SelectMany(x => x.Contexts)
              .SelectMany(x => x.Specifications)
              .Each(x => x.Metadata[SpecificationIdGenerator.Id].ShouldBeOfType<Guid>());
  }
}