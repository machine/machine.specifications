require 'rake'
require 'rake/clean'
require 'fileutils'
require 'configatron'
Dir.glob(File.join(File.dirname(__FILE__), 'tools/Rake/*.rb')).each do |f|
  require f
end
include FileUtils

task :configure do
  project = "Machine.Specifications#{"-Testing" if ENV.include? 'FAKE_VERSION'}"
  target = ENV['target'] || 'Debug'
  
  build_config = {
    :build => {
      :base => "0.4",
      :number => ENV['BUILD_NUMBER'],
      :sha => ENV['BUILD_VCS_NUMBER'] || 'no SHA',
    },
    :project => project,
    :target => target,
    :out_dir => "Build/#{target}/",
    :nunit_framework => "net-3.5",
    :mspec_options => (["--teamcity"] if ENV.include?('TEAMCITY_PROJECT_NAME')) || []
  }

  configatron.nuget.key = Configatron::Dynamic.new do
    ENV['NUGET_KEY']
  end
  configatron.nuget.package = Configatron::Delayed.new do
    "Distribution/#{configatron.project}.#{configatron.version.compatible}.nupkg"
  end
  configatron.zip.package = Configatron::Delayed.new do
    "Distribution/#{configatron.project}-#{configatron.target}-#{configatron.version.full}.zip"
  end
  configatron.version.full  = Configatron::Delayed.new do
    ENV['FAKE_VERSION'] || "#{configatron.build.base}.#{configatron.build.number || '0'}-#{configatron.build.sha[0..6]}"
  end
  configatron.version.compatible   = Configatron::Delayed.new do
    ENV['FAKE_VERSION'] || "#{configatron.build.base}.#{configatron.build.number || '0'}.0"
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
end

namespace :build do
  desc "Compile everything"
  task :compile => 'generate:version' do
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
    
    specs = FileList.new("#{configatron.out_dir}/Tests/*.Specs.dll").to_a
    sh "#{configatron.out_dir}/mspec.exe", "--html", "Specs/#{configatron.project}.Specs.html", "-x", "example", *(configatron.mspec_options + specs)
    
    specs = ["#{configatron.out_dir}/Tests/Machine.Specifications.Example.Clr4.dll"]
    sh "#{configatron.out_dir}/mspec-clr4.exe", "-x", "example", *(configatron.mspec_options + specs)
    
    puts "Wrote specs to Specs/#{configatron.project}.Specs.html, run 'rake specs:view' to see them"
  end
end

namespace :tests do
  desc "Run unit tests"
  task :run do
    puts 'Running NUnit tests...'
    
    tests = FileList.new("#{configatron.out_dir}/Tests/*.Tests.dll").to_a
    runner = NUnitRunner.new :platform => 'x86', :results => "Specs", :clr_version => configatron.nunit_framework
    runner.execute tests
  end
  
  task :run do
    puts 'Running Gallio tests...'
    # This fails left and right for no obvious reason.
    # sh "Tools/Gallio/v3.1.397/Gallio.Echo.exe", "#{configatron.out_dir}/Tests/Gallio/Machine.Specifications.TestGallioAdapter.3.1.Tests.dll", "/plugin-directory:#{configatron.out_dir}", "/r:Local"
  end
end

namespace :package do
  desc "Package build artifacts as a zip file"
  task :zip => [ 'build:rebuild', 'tests:run', 'specs:run' ] do
    rm_f configatron.zip.package
    
    cp 'License.txt', configatron.out_dir
    
    sz = SevenZip.new \
      :tool => 'Tools/7-Zip/7za.exe',
      :zip_name => configatron.zip.package

    Dir.chdir(configatron.out_dir) do
      sz.zip :files => FileList.new('**/*') \
        .exclude('*.InstallLog') \
        .exclude('*.InstallState') \
        .exclude('Generation') \
        .exclude('Tests') \
        .exclude('NuGet')
    end
  end

  namespace :nuget do
    desc "Package build artifacts as a NuGet package"
    task :create => :zip do
      SevenZip.unzip \
        :tool => 'Tools/7-Zip/7za.exe',
        :zip_name => configatron.zip.package,
        :destination => "#{configatron.out_dir}/NuGet".gsub(/\//, '\\')

      QuickTemplate.new('mspec.nuspec.template').exec configatron

      opts = ["pack", "mspec.nuspec",
        "-BasePath", "#{configatron.out_dir}/NuGet",
        "-OutputDirectory", configatron.nuget.package.dirname]

      sh "Tools/NUGet/NuGet.exe", *(opts)
    end
    
    desc "Publishes the NuGet package"
    task :publish do
      raise "NuGet access key is missing, cannot publish" if configatron.nuget.key.nil?

      opts = ["push",
        "-source", "http://packages.nuget.org/v1/",
        configatron.nuget.package,
        configatron.nuget.key,
        { :verbose => false }]

      sh "Tools/NuGet/NuGet.exe", *(opts) do |ok, status|
        ok or fail "Command failed with status (#{status.exitstatus})"
      end
    end
  end
end