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
    :target => target,
    :sign_assembly => ENV.include?('SIGN_ASSEMBLY'),
    :out_dir => "Build/#{target}/",
    :nunit_framework => "net-3.5",
    :mspec_options => (["--teamcity"] if ENV.include?('TEAMCITY_PROJECT_NAME')) || []
  }

  configatron.nuget.key = Configatron::Dynamic.new do
    next File.read('NUGET_KEY') if File.readable?('NUGET_KEY')
  end
  configatron.project = Configatron::Delayed.new do
    "#{project}#{'-Signed' if configatron.sign_assembly}"
  end
  configatron.distribution.dir = Configatron::Delayed.new do
    "Distribution/"
  end
  configatron.version.full = Configatron::Delayed.new do
    open("|Tools/GitFlowVersion/GitFlowVersion.exe").read().scan(/NugetVersion":"(.*)"/)[0][0][0,20]
  end
  configatron.version.short = Configatron::Delayed.new do
    open("|Tools/GitFlowVersion/GitFlowVersion.exe").read().scan(/ShortVersion":"(.*)"/)[0][0]
  end

  configatron.configure_from_hash build_config
  configatron.protect_all!
  puts configatron.inspect
end

Rake::Task['configure'].invoke

desc "Build and run specs"
task :default => ['build:compile', 'tests:run', 'specs:run']

CLEAN.clear
CLEAN.include('teamcity-info.xml')
CLEAN.include('Source/**/obj')
CLEAN.include('Build')
CLEAN.include('Distribution')
CLEAN.include('Specs')
CLEAN.include('**/*.template')
# Clean template results.
CLEAN.map! do |f|
  next f.ext if f.pathmap('%x') == '.template'
  f
end

namespace :generate do
  
  puts "##teamcity[buildNumber '#{configatron.version.short}']"

  desc 'Update the configuration files for the build'
  task :config do
    FileList.new('**/*.template').each do |template|
      QuickTemplate.new(template).exec(configatron)
    end
  end
end

namespace :build do
  desc "Compile everything"
  task :compile => ['generate:config'] do
    opts = {
        :version => 'v4\Full',
        :switches => { :verbosity => :minimal, :target => :Build },
		:project => './Machine.Specifications.sln',
        :properties => {
          :Configuration => configatron.target,
          :TrackFileAccess => false,
          :SolutionDir => File.expand_path('.'),
		  :SignAssembly => configatron.sign_assembly,
		  :Platform => 'Any CPU'
        }
      }

    begin
      nopts = %W(
        Tools/Nuget/NuGet.exe restore ./Machine.Specifications.sln
      )

      sh(*nopts)
	  
      MSBuild.compile opts

    end
  end

  desc "Rebuild everything"
  task :rebuild => [ :clean, :compile ]
end

namespace :specs do
  task :view => :run do
    system "start Specs/#{configatron.project}.Specs.html"
  end

  desc "Run specifications"
  task :run do
    puts 'Running Specs...'

    specs = FileList.new("#{configatron.out_dir}/Tests/*.Specs.dll").exclude(/Clr4/)
    sh "#{configatron.out_dir}/mspec.exe", "--html", "Specs/#{configatron.project}.Specs.html", *(configatron.mspec_options + specs)

    specs = FileList.new("#{configatron.out_dir}/Tests/*.Clr4.Specs.dll")
    sh "#{configatron.out_dir}/mspec-clr4.exe", *(configatron.mspec_options + specs)

    puts "Wrote specs to Specs/#{configatron.project}.Specs.html, run 'rake specs:view' to see them"
  end
end

namespace :tests do
  desc "Run unit tests"
  task :run do
    puts 'Running NUnit tests...'

    tests = FileList.new("#{configatron.out_dir}/Tests/*.Tests.dll").to_a
    runner = NUnitRunner.new :tool => 'Source/packages/NUnit.Runners/tools/nunit-console-x86.exe', :results => "Specs", :clr_version => configatron.nunit_framework
    runner.execute tests
  end
end

namespace :package do
    namespace :nuget do
    desc "Package build artifacts as a NuGet package and a symbols package"
    task :create => [ 'build:rebuild', 'tests:run', 'specs:run' ] do
		opts = %W(
		  Tools/Ripple/Ripple.exe create-packages --version #{configatron.version.full} --symbols --verbose --destination #{configatron.distribution.dir}
		  )

		sh(*opts)
    end

    desc "Publishes the NuGet package"
    task :publish => [ 'build:rebuild', 'tests:run', 'specs:run' ] do
      raise "NuGet access key is missing, cannot publish" if configatron.nuget.key.nil?

      opts = %W(
        Tools/Ripple/Ripple.exe publish #{configatron.version.full} #{configatron.nuget.key} --symbols --artifacts #{configatron.distribution.dir} --verbose         
      )

      sh(*opts) do |ok, status|
        ok or fail "Command failed with status (#{status.exitstatus})"
    end
	end
	end
end
