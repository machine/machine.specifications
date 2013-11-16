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
    :build => {
      :base => File.open('VERSION') { |f| f.readline }.strip,
      :number => ENV['BUILD_NUMBER'],
      :sha => ENV['BUILD_VCS_NUMBER'] || 'no SHA',
      :prerelease => ENV.include?('PRERELEASE')
    },
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
  configatron.nuget.package = Configatron::Delayed.new do
    "Distribution/#{configatron.project}.#{configatron.version.package}.nupkg"
  end
  configatron.zip.package = Configatron::Delayed.new do
    "Distribution/#{configatron.project}-#{configatron.target}.zip"
  end

  configatron.version.beta = Configatron::Delayed.new do
    "-beta#{"%02d" % configatron.build.number}" if configatron.build.prerelease || nil
  end
  configatron.version.full = Configatron::Delayed.new do
    "#{configatron.build.base}#{configatron.version.beta}-#{configatron.build.sha[0..6]}"
  end
  configatron.version.package = Configatron::Delayed.new do
    "#{configatron.build.base}#{configatron.version.beta}"
  end
  configatron.version.compatible = Configatron::Delayed.new do
    "#{configatron.build.base}.0"
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
  desc "Generate embeddable version information"
  task :version do
    next if configatron.build.number.nil?

    puts "##teamcity[buildNumber '#{configatron.version.full}']"

    asmInfo = AssemblyInfoBuilder.new({
      :AssemblyFileVersion => configatron.version.compatible,
      :AssemblyVersion => configatron.version.compatible,
      :AssemblyInformationalVersion => configatron.version.full
    })

    asmInfo.write 'Source/VersionInfo.cs'
  end

  desc 'Update the configuration files for the build'
  task :config do
    FileList.new('**/*.template').each do |template|
      QuickTemplate.new(template).exec(configatron)
    end
  end
end

namespace :build do
  desc "Compile everything"
  task :compile => ['generate:version', 'generate:config'] do
    opts = {
        :version => 'v4\Full',
        :switches => { :verbosity => :minimal, :target => :Build },
        :properties => {
          :Configuration => configatron.target,
          :TrackFileAccess => false,
          :SolutionDir => File.expand_path('.')
        }
      }

    def patch(project_file, config)
      csproj = File.read project_file

      patched = csproj.dup
      config.each do |element, value|
        patched.gsub!(/<#{element}>.*?<\/#{element}>/, "<#{element}>#{value}</#{element}>")
      end

      File.open(project_file, "w") { |file| file.write patched }

      {
        :file => project_file,
        :original_contents => csproj
      }
    end

    begin
      csprojs = FileList.new('Source/**/*.csproj').map do |project|
        patch project, { :SignAssembly => configatron.sign_assembly }
      end

      csprojs.each do |project|
        MSBuild.compile opts.merge({ :project => project[:file] })
      end

      runner_configs = {
        :x86         => { :TargetFrameworkVersion => 'v3.5', :PlatformTarget => 'x86',    :AssemblyName => 'mspec-x86' },
        :AnyCPU      => { :TargetFrameworkVersion => 'v3.5', :PlatformTarget => 'AnyCPU', :AssemblyName => 'mspec' },
        :clr4_x86    => { :TargetFrameworkVersion => 'v4.0', :PlatformTarget => 'x86',    :AssemblyName => 'mspec-x86-clr4' },
        :clr4_AnyCPU => { :TargetFrameworkVersion => 'v4.0', :PlatformTarget => 'AnyCPU', :AssemblyName => 'mspec-clr4' }
      }
      console_runner = 'Source/Runners/Machine.Specifications.ConsoleRunner/Machine.Specifications.ConsoleRunner.csproj'

      runner_configs.values.each do |config|
        project = patch console_runner, config

        # We need to remove obj, otherwise MSBuild will delete the build output from previous runner_config builds.
        rm_rf File.join(File.dirname(console_runner), 'obj')

        MSBuild.compile opts.merge({ :project => project[:file] })
      end
    ensure
      csprojs.each do |project|
        File.open(project[:file], "w") { |file| file.write project[:original_contents] }
      end
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
    runner = NUnitRunner.new :tool => 'packages/NUnit.Runners.2.6.3/tools/nunit-console-x86.exe', :results => "Specs", :clr_version => configatron.nunit_framework
    runner.execute tests
  end

  task :run do
    puts 'Running Gallio tests...'
    sh "Tools/Gallio/bin/Gallio.Echo.exe", "#{configatron.out_dir}/Tests/Gallio/Machine.Specifications.GallioAdapter.Tests.dll", "/plugin-directory:#{configatron.out_dir}", "/r:Local"
  end
end

namespace :package do
  def net_20_framework_files(root = '.')
    FileList.new("#{root}/Machine.Specifications.dll") \
      .include("#{root}/Machine.Specifications.pdb") \
      .include("#{root}/Machine.Specifications.Should.dll") \
      .include("#{root}/Machine.Specifications.Should.pdb") \
      .include("#{root}/Machine.Specifications.dll.tdnet") \
      .include("#{root}/Machine.Specifications.TDNetRunner.*")
  end

  def net_40_framework_files(root = '.')
    net_20_framework_files(root) \
      .include("#{root}/Machine.Specifications.Clr4.dll") \
      .include("#{root}/Machine.Specifications.Clr4.pdb")
  end

  def source_files(root = '.')
    FileList.new("#{root}/**/*.cs") \
      .exclude('**/*Example*') \
      .exclude('**/*.Specs/') \
      .exclude('**/*.Test*/')
  end

  def packaged_files(root = '.')
    FileList.new("#{root}/**/*") \
      .exclude("#{root}/**/*.InstallLog") \
      .exclude("#{root}/**/*.InstallState") \
      .exclude("#{root}/Generation") \
      .exclude("#{root}/Tests") \
      .exclude("#{root}/NuGet") \
      .exclude("#{root}/Package")
  end

  def copy_distributed_files_to(destination)
    mkdir_p destination

    %w(License.txt History.txt).each { |f| cp f, destination }

    net_20_framework_files(configatron.out_dir).copy_hierarchy \
      :source_dir => configatron.out_dir,
      :target_dir => "#{destination}/lib/net20"

    net_40_framework_files(configatron.out_dir).copy_hierarchy \
      :source_dir => configatron.out_dir,
      :target_dir => "#{destination}/lib/net40"

    packaged_files(configatron.out_dir).copy_hierarchy \
      :source_dir => configatron.out_dir,
      :target_dir => "#{destination}/tools"
  end

  desc "Package build artifacts as a zip file"
  task :zip => [ 'build:rebuild', 'tests:run', 'specs:run' ] do
    rm_f configatron.zip.package

    copy_distributed_files_to "#{configatron.out_dir}/Package"

    sz = SevenZip.new \
      :tool => 'Tools/7-Zip/7za.exe',
      :zip_name => configatron.zip.package

    Dir.chdir("#{configatron.out_dir}/Package") do
      sz.zip :files => FileList.new("**/*")
    end
  end

  def create_package_from(dir)
    dir = dir.gsub(%r|/|, '\\').gsub(%r|\\\\|, '\\')

    opts = %W(
      .nuget/NuGet.exe pack mspec.nuspec
      -BasePath #{dir}
      -OutputDirectory #{configatron.nuget.package.dirname}
      )

    sh(*opts)
  end

  namespace :nuget do
    desc "Package build artifacts as a NuGet package and a symbols package"
    task :create => :zip do

      copy_distributed_files_to "#{configatron.out_dir}/NuGet"
      source_files('Source').copy_hierarchy \
        :source_dir => 'Source',
        :target_dir => "#{configatron.out_dir}/NuGet/src/"

      cp 'install.ps1', "#{configatron.out_dir}/NuGet/tools"

      create_package_from "#{configatron.out_dir}/NuGet"
      mv configatron.nuget.package, configatron.nuget.package.pathmap('%X.symbols%x')

      FileList["#{configatron.out_dir}/NuGet/src/", "#{configatron.out_dir}/NuGet/**/*.pdb"].each { |f| rm_rf f }
      create_package_from "#{configatron.out_dir}/NuGet"
    end

    desc "Publishes the NuGet package"
    task :publish do
      raise "NuGet access key is missing, cannot publish" if configatron.nuget.key.nil?

      opts = %W(
        .nuget/NuGet.exe push
        #{configatron.nuget.package}
        #{configatron.nuget.key}
      ) << { :verbose => false }

      sh(*opts) do |ok, status|
        ok or fail "Command failed with status (#{status.exitstatus})"
      end
    end
  end
end
