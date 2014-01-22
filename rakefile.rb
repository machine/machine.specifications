begin
  require 'bundler/setup'
  require 'albacore'
  require 'configatron'  
  require 'fileutils'

rescue LoadError
  puts 'Bundler and all the gems need to be installed prior to running this rake script. Installing...'
  system("gem install bundler --source http://rubygems.org")
  sh 'bundle install'
  system("bundle exec rake", *ARGV)
  exit 0
end

task :rebuild => [ :clean, :configure, :restore, :build ]

task :default => [ :rebuild ]

desc "Prepares necessary configuration for build"
task :configure do
  target = ENV['target'] || 'Debug'

  build_config = {
    :target => target,
    :sign_assembly => ENV.include?('SIGN_ASSEMBLY'),
    :out_dir => "Build/#{target}/",
    :nunit_framework => "net-3.5",
    :mspec_options => (["--teamcity"] if ENV.include?('TEAMCITY_PROJECT_NAME')) || []
  }
  
  configatron.solution = Configatron::Delayed.new do
    FileList.new("*.sln").to_a[0]
  end
  configatron.project = Configatron::Delayed.new do
    "#{configatron.solution.gsub(".sln", "")}#{'-Signed' if configatron.sign_assembly}"
  end
  configatron.distribution.dir = Configatron::Delayed.new do
    "Distribution/"
  end
  configatron.version.full = Configatron::Delayed.new do
    `gitflowversion`.scan(/NugetVersion":"(.*)"/)[0][0][0,20]
  end
  configatron.version.short = Configatron::Delayed.new do
    `gitflowversion`.scan(/ShortVersion":"(.*)"/)[0][0]
  end  

  configatron.configure_from_hash build_config
  #configatron.protect_all!
  puts configatron.inspect
end

desc "Prepares the working directory for a new build"
task :clean do
  filesToClean = FileList.new
  filesToClean.include('teamcity-info.xml')
  filesToClean.include('**/obj')
  filesToClean.include('Build')
  filesToClean.include('Distribution')
  filesToClean.include('Specs')
  filesToClean.include('**/*.template')
  # Clean template results.
  filesToClean.map! do |f|
  next f.ext if f.pathmap('%x') == '.template'
  f
  end
  FileUtils.rm_rf filesToClean
  
  Dir.mkdir 'Distribution'
  Dir.mkdir 'Specs'
end

task :restore do
  nopts = %W(
   nuget restore "#{configatron.solution}"
  )

  sh(*nopts)
end

desc "Run a simple clean/build"
msbuild :build do |msb|
  msb.solution = configatron.solution
  msb.targets = [:Clean, :Build]
  msb.use :net4
  msb.verbosity = :minimal
  msb.properties = {
     :Configuration => configatron.target,
     :TrackFileAccess => false,
     :SolutionDir => File.expand_path('.'),
     :SignAssembly => configatron.sign_assembly,
     :Platform => 'Any CPU'
  }
end

desc "Package build artifacts as a NuGet package and a symbols package"
task :createpackage => [ :default ] do
	FileList.new('**/*.nuspec').each do |nuspec|
		opts = %W(
			nuget pack #{nuspec} -Symbols -OutputDirectory #{configatron.distribution.dir}
		)

		sh(*opts) do |ok, status|
			ok or fail "Command failed with status (#{status.exitstatus})"
		end
	end
end