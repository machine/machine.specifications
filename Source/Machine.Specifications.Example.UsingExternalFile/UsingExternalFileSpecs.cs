using System.IO;

namespace Machine.Specifications.Example.UsingExternalFile
{
  [Subject("External resources usage")]
  public class when_using_file_copied_to_assembly_output_directory
  {
    It should_be_able_to_locate_it_by_relative_path = () => 
      File.Exists("TestData.txt").ShouldBeTrue();
  }
}