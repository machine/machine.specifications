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
      :base => "0.5",
      :number => ENV['BUILD_NUMBER'],
      :sha => ENV['BUILD_VCS_NUMBER'] || 'no SHA',
    },
    :target => target,
    :sign_assembly => (ENV['SIGN_ASSEMBLY'] =~ /true/i and true or false),
    :out_dir => "Build/#{target}/",
    :nunit_framework => "net-3.5",
    :mspec_options => (["--teamcity"] if ENV.include?('TEAMCITY_PROJECT_NAME')) || []
  }

  configatron.nuget.key = Configatron::Dynamic.new do
    next File.read('NUGET_KEY') if File.readable?('NUGET_KEY')
  end
  configatron.project = Configatron::Delayed.new do
    "#{project}#{"-Testing" if ENV.include? 'VERSION'}#{'-Signed' if configatron.sign_assembly}"
  end
  configatron.nuget.package = Configatron::Delayed.new do
    "Distribution/#{configatron.project}.#{configatron.version.compatible}.nupkg"
  end
  configatron.zip.package = Configatron::Delayed.new do
    "Distribution/#{configatron.project}-#{configatron.target}.zip"
  end
  configatron.version.full  = Configatron::Delayed.new do
    ENV['VERSION'] || "#{configatron.build.base}.#{configatron.build.number || '0'}-#{configatron.build.sha[0..6]}"
  end
  configatron.version.compatible   = Configatron::Delayed.new do
    ENV['VERSION'] || "#{configatron.build.base}.#{configatron.build.number || '0'}.0"
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
    next if configatron.build.number.nil? && !ENV.include?('VERSION')
    
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
          :TrackFileAccess => false
        }
      }
      
    def build (msbuild_options, config)
      project = msbuild_options[:project]

      xml = File.read project
      config.each do |element, value|
        xml.gsub! /<#{element}>.*?<\/#{element}>/, "<#{element}>#{value}</#{element}>"
      end

      project += config.hash.to_s
      File.open(project, "w") { |file| file.puts xml }

      MSBuild.compile msbuild_options.merge({ :project => project })

      rm project
    end

    FileList.new('Source/**/*.csproj').each do |project|
      build opts.merge({ :project => project }), { :SignAssembly => configatron.sign_assembly }
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
    runner = NUnitRunner.new :tool => 'packages/NUnit.2.5.10.11092/tools/nunit-console-x86.exe', :results => "Specs", :clr_version => configatron.nunit_framework
    runner.execute tests
  end
  
  task :run do
    puts 'Running Gallio tests...'
    sh "Tools/Gallio/v3.1.397/Gallio.Echo.exe", "#{configatron.out_dir}/Tests/Gallio/Machine.Specifications.TestGallioAdapter.3.1.Tests.dll", "/plugin-directory:#{configatron.out_dir}", "/r:Local"
  end
end

namespace :package do
  def framework_files(root = '.')
    FileList.new("#{root}/Machine.Specifications.dll") \
      .include("#{root}/Machine.Specifications.pdb") \
      .include("#{root}/Machine.Specifications.dll.tdnet") \
      .include("#{root}/Machine.Specifications.TDNetRunner.*")
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
      .exclude("#{root}/NuGet")
  end

  desc "Package build artifacts as a zip file"
  task :zip => [ 'build:rebuild', 'tests:run', 'specs:run' ] do
    rm_f configatron.zip.package
    
    cp 'License.txt', configatron.out_dir
    
    sz = SevenZip.new \
      :tool => 'Tools/7-Zip/7za.exe',
      :zip_name => configatron.zip.package
    
    Dir.chdir(configatron.out_dir) do
      sz.zip :files => packaged_files
    end
  end
  
  def create_package
    opts = ["pack", "mspec.nuspec",
      "-BasePath", "#{configatron.out_dir}NuGet".gsub(/\//, '\\'),
      "-OutputDirectory", configatron.nuget.package.dirname]

    sh "Tools/NuGet/NuGet.exe", *(opts)
  end

  namespace :nuget do
    desc "Package build artifacts as a NuGet package and a symbols package"
    task :create => :zip do
      framework_files(configatron.out_dir).copy_hierarchy \
        :source_dir => configatron.out_dir,
        :target_dir => "#{configatron.out_dir}NuGet/lib/"

      source_files('Source').copy_hierarchy \
        :source_dir => 'Source',
        :target_dir => "#{configatron.out_dir}NuGet/src/"

      packaged_files(configatron.out_dir).copy_hierarchy \
        :source_dir => configatron.out_dir,
        :target_dir => "#{configatron.out_dir}NuGet/tools/"

      cp 'install.ps1', "#{configatron.out_dir}NuGet/tools/"

      create_package
      mv configatron.nuget.package, configatron.nuget.package.pathmap('%X.symbols%x')

      FileList["#{configatron.out_dir}NuGet/src/", "#{configatron.out_dir}NuGet/**/*.pdb"].each { |f| rm_rf f }
      create_package
    end
    
    desc "Publishes the NuGet package"
    task :publish do
      raise "NuGet access key is missing, cannot publish" if configatron.nuget.key.nil?

      opts = ["push",
        configatron.nuget.package,
        configatron.nuget.key,
        { :verbose => false }]

      sh "Tools/NuGet/NuGet.exe", *(opts) do |ok, status|
        ok or fail "Command failed with status (#{status.exitstatus})"
      end
    end
  end
end