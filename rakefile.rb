require 'rake'
require 'rake/clean'
require 'fileutils'
require 'configatron'
Dir.glob(File.join(File.dirname(__FILE__), 'tools/Rake/*.rb')).each do |f|
  require f
end
include FileUtils

task :configure do
  project = "Machine.Specifications"
  target = ENV['target'] || 'Debug'
  
  build_config = {
    :project => project,
    :target => target,
    :out_dir => "Build/#{target}/",
    :package_name => "Distribution/#{project}-#{target}.zip",
    :nunit_framework => "net-3.5",
    :mspec_options => []
  }

  configatron.configure_from_hash build_config
  configatron.protect_all!
  puts configatron.inspect
end

Rake::Task["configure"].invoke

desc "Build and run specs"
task :default => [ "build", "tests:run", "specs:run" ]

CLEAN.clear
CLEAN.include('teamcity-info.xml')
CLEAN.include('Source/**/obj')
CLEAN.include('Build')
CLEAN.include(configatron.package_name)

desc "Build"
task :build do
  opts = {
      :version => 'v4\Full',
      :switches => { :verbosity => :minimal, :target => :Build },
      :properties => {
        :Configuration => configatron.target,
        :TrackFileAccess => false
      }
    }
    
  FileList.new('Source/**/*.csproj').each do |project|
    MSBuild.compile opts.merge({ :project => project })
  end

  def build (msbuild_options, config)
    project = msbuild_options[:project]
    
    xml = File.read project
    config.each do |element, value|
      xml.gsub! /<#{element}>.*?<\/#{element}>/, "<#{element}>#{value}</#{element}>"
    end
    
    patched_project = project + config.hash.to_s
    File.open(patched_project, "w") { |file| file.puts xml }
    
    MSBuild.compile msbuild_options.merge({ :project => patched_project })
    
    rm patched_project
  end
  
  console_runner = {
    :x86         => { :TargetFrameworkVersion => 'v3.5', :PlatformTarget => 'x86',    :AssemblyName => 'mspec-x86' },
    :AnyCPU      => { :TargetFrameworkVersion => 'v3.5', :PlatformTarget => 'AnyCPU', :AssemblyName => 'mspec' },
    :clr4_x86    => { :TargetFrameworkVersion => 'v4.0', :PlatformTarget => 'x86',    :AssemblyName => 'mspec-x86-clr4' },
    :clr4_AnyCPU => { :TargetFrameworkVersion => 'v4.0', :PlatformTarget => 'AnyCPU', :AssemblyName => 'mspec-clr4' }
  }
  
  console_runner.values.each do |config|
    project = 'Source/Machine.Specifications.ConsoleRunner/Machine.Specifications.ConsoleRunner.csproj'
    build opts.merge({ :project => project }), config
  end
end

desc "Rebuild"
task :rebuild => [ :clean, :build ]

namespace :specs do
  task :view => :run do
    system "start Specs/#{configatron.project}.Specs.html"
  end

  task :run do
    puts 'Running Specs...'
    
    specs = FileList.new("#{configatron.out_dir}/Tests/*.Specs.dll").to_a
    sh "#{configatron.out_dir}/mspec.exe", "--html", "Specs/#{configatron.project}.Specs.html", "-x", "example", *(configatron.mspec_options + specs)
    
    specs = ["#{configatron.out_dir}/Tests/Machine.Specifications.Example.Clr4.dll"]
    sh "#{configatron.out_dir}/mspec-clr4.exe", "-x", "example", *(configatron.mspec_options + specs)
    
    puts "Wrote specs to Specs/#{configatron.project}.Specs.html, run 'rake specs:view' to see them"
  end
end

namespace :tests do
  task :run do
    puts 'Running NUnit tests...'
    
    tests = FileList.new("#{configatron.out_dir}/Tests/*.Tests.dll").to_a
    runner = NUnitRunner.new :platform => 'x86', :results => "Specs", :clr_version => configatron.nunit_framework
    runner.executeTests tests
  end
  
  task :run do
    puts 'Running Gallio tests...'
    sh "Tools/Gallio/v3.1.397/Gallio.Echo.exe", "#{configatron.out_dir}/Tests/Gallio/Machine.Specifications.TestGallioAdapter.3.1.Tests.dll", "/plugin-directory:#{configatron.out_dir}", "/r:Local"
  end
end

desc "Packages the build artifacts"
task :package => [ "rebuild", "tests:run", "specs:run" ] do
  rm_f configatron.package_name
  
  cp 'License.txt', configatron.out_dir
  
  sz = SevenZip.new \
    :tool => 'Tools/7-Zip/7za.exe',
    :zip_name => configatron.package_name

  Dir.chdir(configatron.out_dir) do
    sz.zip :files => FileList.new('**/*') \
      .exclude('*.InstallLog') \
      .exclude('*.InstallState') \
      .exclude('Generation') \
      .exclude('Tests')
  end
end

desc "Packages the build artifacts as a NuGet"
task :nuget do
  FileList.new("#{configatron.package_name.dirname}/*-Release.zip").each do |f|
	version = f.split("-").values_at(1, 2).join.delete(".")

	SevenZip.unzip \
	  :tool => 'Tools/7-Zip/7za.exe',
	  :zip_name => f,
	  :destination => "Build/NuGet/#{version}".gsub(/\//, '\\')
  end

  sh "Tools/NUGet/NuGet.exe", "pack", "mspec.nuspec", "-BasePath", "Build/NuGet", "-OutputDirectory", "Distribution"
end

desc "TeamCity build"
task :teamcity => [ :teamcity_environment, :package ]

desc "Sets up the TeamCity environment"
task :teamcity_environment do
  configatron.mspec_options.push "--teamcity"
end