require 'rake'
require 'fileutils'
require 'configatron'
Dir.glob(File.join(File.dirname(__FILE__), 'tools/Rake/*.rb')).each do |f|
  require f
end
include FileUtils

project = "Machine.Specifications"
mspec_options = []

task :configure do
  version = ENV.include?('version') ? ENV['version'].to_sym : :net_35
  target = ENV.include?('target') ? ENV['target'] : 'Debug'
  
  build_config = {
    :net_35 => {
      :friendly_name => 'net-3.5',
      :version => 'v3.5',
      :project => project,
      :solution => project,
      :target => target,
      :compile_target => target,
      :out_dir => "Build/#{target}/",
      :package_name => "Distribution/#{project}-net-3.5-#{target}.zip",
      :nunit_framework => "net-3.5"
    },
    :net_40 => {
      :friendly_name => 'net-4.0',
      :version => 'v4\Full',
      :project => project,
      :solution => "#{project}-2010",
      :target => target,
      :compile_target => "#{target} .NET 4.0".escape,
      :out_dir => "Build/#{target}/",
      :package_name => "Distribution/#{project}-net-4.0-#{target}.zip",
      :nunit_framework => "net-3.5",
      :additional_specs => ["Machine.Specifications.Example.Clr4.dll"],
    }
  }

  configatron.configure_from_hash build_config[version]
  configatron.protect_all!
  puts configatron.inspect
end

Rake::Task["configure"].invoke

desc "Build and run specs"
task :default => [ "build", "tests:run", "specs:run" ]

desc "Clean"
task :clean do
  MSBuild.compile \
    :project => "Source/#{configatron.solution}.sln",
    :version => configatron.version,
    :properties => {
      :Configuration => configatron.compile_target
    },
    :switches => {
      :target => 'Clean'
    }

  rm_f configatron.package_name
  rm_rf "Build"
end

desc "Build"
task :build do
  MSBuild.compile \
    :project => "Source/#{configatron.solution}.sln",
    :version => configatron.version,
    :properties => {
      :Configuration => configatron.compile_target
    }
end

desc "Rebuild"
task :rebuild => [ :clean, :build ]

namespace :specs do
  task :view => :run do
    system "start Specs/#{configatron.project}.Specs.html"
  end

  task :run do
    puts 'Running Specs...'
    specs = ["Machine.Specifications.Specs.dll", "Machine.Specifications.Reporting.Specs.dll", "Machine.Specifications.ConsoleRunner.Specs.dll"]
    specs = specs | configatron.additional_specs if configatron.exists?(:additional_specs)
    specs.map! {|spec| "#{configatron.out_dir}/Tests/#{spec}"}
    sh "#{configatron.out_dir}/mspec.exe", "--html", "Specs/#{configatron.project}.Specs.html", "-x", "example", *(mspec_options + specs)
    puts "Wrote specs to Specs/#{configatron.project}.Specs.html, run 'rake specs:view' to see them"
  end
end

namespace :tests do
  task :run do
    puts 'Running NUnit tests...'
    tests = ["Machine.Specifications.Tests.dll"].map {|test| "#{configatron.out_dir}/Tests/#{test}"}
    runner = NUnitRunner.new :platform => 'x86', :results => "Specs", :clr_version => configatron.nunit_framework
    runner.executeTests tests
  end
end

desc "Open solution in VS"
task :sln do
  Thread.new do
    system "devenv Source/#{configatron.solution}.sln"
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

desc "TeamCity build"
task :teamcity => [ :teamcity_environment, :package ]

desc "Sets up the TeamCity environment"
task :teamcity_environment do
  mspec_options.push "--teamcity"
end

require "rakefile.#{configatron.friendly_name}.rb" if File.exists? "rakefile.#{configatron.friendly_name}.rb"