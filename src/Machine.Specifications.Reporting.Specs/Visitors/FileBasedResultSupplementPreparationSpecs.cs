using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Reporting.Visitors;
using Machine.Specifications.Utility;
using Rhino.Mocks;

namespace Machine.Specifications.Reporting.Specs.Visitors
{
    [Subject(typeof(FileBasedResultSupplementPreparation))]
    public class when_no_supplements_need_to_be_prepared_for_the_html_report : ReportSpecs
    {
        static FileBasedResultSupplementPreparation Preparation;
        static Run Report;
        static Func<string> ResourcePathCreator;

        Establish context = () =>
        {
            ResourcePathCreator = MockRepository.GenerateStub<Func<string>>();

            Preparation = new FileBasedResultSupplementPreparation(MockRepository.GenerateStub<IFileSystem>());
            Preparation.Initialize(new VisitorContext { ResourcePathCreator = ResourcePathCreator });

            Report = Run(Assembly("assembly 1",
                                  Concern("a 1 concern 1",
                                          Context("a 1 c 1 context 1",
                                                  Spec("it", "a 1 c 1 c 1 specification 2", Runner.Utility.Result.Pass())
                                            )
                                    )
                           ));
        };

        Because of = () => Preparation.Visit(Report);

        It should_not_create_the_folder_for_supplements =
          () => ResourcePathCreator.AssertWasNotCalled(x => x());
    }

    [Subject(typeof(FileBasedResultSupplementPreparation))]
    public class when_file_based_result_supplements_are_prepared_for_the_html_report : ReportSpecs
    {
        static FileBasedResultSupplementPreparation Preparation;
        static Specification Images;
        static Specification HtmlFiles;
        static Run Report;
        static Specification Texts;

        Establish context = () =>
          {
              Preparation = new FileBasedResultSupplementPreparation(MockRepository.GenerateStub<IFileSystem>());
              Preparation.Initialize(new VisitorContext { ResourcePathCreator = () => @"C:\report\resources" });

              Images = Spec("it", "a 1 c 1 c 1 specification 1",
                            Runner.Utility.Result.Supplement(Runner.Utility.Result.Pass(),
                                              "Images",
                                              new Dictionary<string, string>
                                        {
                                          { "img-image", @"C:\some\image.png" },
                                          { "img-another-image", @"C:\some\other\image.png" }
                                        }));

              HtmlFiles = Spec("it", "a 2 c 1 c 1 specification 1",
                               Runner.Utility.Result.Supplement(Runner.Utility.Result.Pass(),
                                                 "HTML",
                                                 new Dictionary<string, string>
                                           {
                                             { "html-file", @"C:\some\file.html" },
                                             { "html-another-file", @"C:\some\other\file.html" }
                                           }));

              Texts = Spec("it", "a 2 c 1 c 2 specification 1",
                           Runner.Utility.Result.Supplement(Runner.Utility.Result.Pass(),
                                             "Text",
                                             new Dictionary<string, string>
                                       {
                                         { "text-text", "text" },
                                         { "text-another-text", "text" }
                                       }));

              Report = Run(Assembly("assembly 1",
                                    Concern("a 1 concern 1",
                                            Context("a 1 c 1 context 1",
                                                    Images,
                                                    Spec("it", "a 1 c 1 c 1 specification 2", Runner.Utility.Result.Pass())
                                              )
                                      )
                             ),
                           Assembly("assembly 2",
                                    Concern("a 2 concern 1",
                                            Context("a 2 c 1 context 1",
                                                    Spec("it", "a 2 c 1 c 1 specification 2", Runner.Utility.Result.Pass()),
                                                    HtmlFiles),
                                            Context("a 2 c 1 context 2",
                                                    Texts,
                                                    Spec("it", "a 2 c 1 c 2 specification 2", Runner.Utility.Result.Pass())))));
          };

        Because of = () => Preparation.Visit(Report);

        It should_copy_all_image_files_to_the_report_resource_directory =
          () => Images.Supplements.Values.Each(v => v.Values.Each(f => f.ShouldStartWith(@"C:\report\resources\")));

        It should_copy_all_HTML_files_to_the_report_resource_directory =
          () => HtmlFiles.Supplements.Values.Each(v => v.Values.Each(f => f.ShouldStartWith(@"C:\report\resources\")));

        It should_not_alter_text_supplements =
          () => Texts.Supplements.Values.Each(v => v.Values.Each(f => f.ShouldEqual("text")));
    }

    [Subject(typeof(FileBasedResultSupplementPreparation))]
    public class when_copying_file_based_result_supplements_fails : ReportSpecs
    {
        static FileBasedResultSupplementPreparation Preparation;
        static Specification Failing;
        static Run Report;
        static IDictionary<string, string> FirstSupplement;

        Establish context = () =>
          {
              var fileSystem = MockRepository.GenerateStub<IFileSystem>();
              fileSystem
                .Stub(x => x.Move(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(PrepareException());

              Preparation = new FileBasedResultSupplementPreparation(fileSystem);
              Preparation.Initialize(new VisitorContext { ResourcePathCreator = () => @"C:\report\resources" });

              Failing = Spec("it", "a 1 c 1 c 1 specification 1",
                             Runner.Utility.Result.Supplement(Runner.Utility.Result.Pass(),
                                               "Failing Images and Text",
                                               new Dictionary<string, string>
                                         {
                                           { "img-image", @"C:\some\image.png" },
                                           { "img-another-image", @"C:\some\other\image.png" },
                                           { "text-some-text", "text" }
                                         }));

              Report = Run(Assembly("assembly 1",
                                    Concern("a 1 concern 1",
                                            Context("a 1 c 1 context 1",
                                                    Failing))));
          };

        Because of = () =>
          {
              Preparation.Visit(Report);

              FirstSupplement = Report.Assemblies.First()
                .Concerns.First()
                .Contexts.First()
                .Specifications.First()
                .Supplements.First()
                .Value;
          };

        It should_replace_file_results_with_text_results =
          () => FirstSupplement.Keys.Each(x => x.ShouldStartWith("text-"));

        It should_replace_file_results_with_text_results_where_the_original_key_is_prefixed_with__text__ =
          () => FirstSupplement.Keys.Where(x => x.StartsWith("text-img-")).Count().ShouldEqual(2);

        It should_replace_file_results_with_text_results_containing_the_error_message =
          () => FirstSupplement
                  .Where(x => x.Key.StartsWith("text-img-"))
                  .Each(x => x.Value.ShouldStartWith(@"Failed to copy supplement C:\some\"));
    }

    [Subject(typeof(FileBasedResultSupplementPreparation))]
    public class when_copying_file_based_result_supplements_fails_and_the_error_message_generates_a_conflict : ReportSpecs
    {
        static FileBasedResultSupplementPreparation Preparation;
        static Specification Failing;
        static Run Report;
        static IDictionary<string, string> FirstSupplement;

        Establish context = () =>
          {
              var fileSystem = MockRepository.GenerateStub<IFileSystem>();
              fileSystem
                .Stub(x => x.Move(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(PrepareException());

              Preparation = new FileBasedResultSupplementPreparation(fileSystem);
              Preparation.Initialize(new VisitorContext { ResourcePathCreator = () => @"C:\report\resources" });

              Failing = Spec("it", "a 1 c 1 c 1 specification 1",
                             Runner.Utility.Result.Supplement(Runner.Utility.Result.Pass(),
                                               "Failing Images and Text",
                                               new Dictionary<string, string>
                                         {
                                           { "img-image", @"C:\some\image.png" },
                                           { "text-img-image-error", "will conflict with error for img-image" },
                                           { "text-img-image-error-error", "will conflict with error for img-image" }
                                         }));

              Report = Run(Assembly("assembly 1",
                                    Concern("a 1 concern 1",
                                            Context("a 1 c 1 context 1",
                                                    Failing))));
          };

        Because of = () =>
          {
              Preparation.Visit(Report);

              FirstSupplement = Report.Assemblies.First()
                .Concerns.First()
                .Contexts.First()
                .Specifications.First()
                .Supplements.First()
                .Value;
          };

        It should_succeed =
          () => true.ShouldBeTrue();

        It should_create_a_unique_error_key =
          () => FirstSupplement.Keys.ShouldContain("text-img-image-error-error-error");
    }
}
