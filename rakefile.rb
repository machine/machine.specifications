begin
  require 'bundler/setup'
  require 'albacore'
  require 'configatron'  
  require 'fileutils'
  require './quicktemplate.rb'
  require './rakefile.include.rb'

rescue LoadError
  puts 'Bundler and all the gems need to be installed prior to running this rake script. Installing...'
  system("gem install bundler --source http://rubygems.org")
  sh 'bundle install'
  system("bundle exec rake", *ARGV)
  exit 0
end

task :rebuild => [ :clean, :configure, :restore, :build ]

task :default => [ :rebuild ]

desc "Package build artifacts as a NuGet package and a symbols package"
task(:createpackage).clear
task :createpackage => [ :default ] do
	nuspecs = FileList.new('**/*.nuspec')
	nuspecs.exclude(/packages/)
	nuspecs.each do |nuspec|
		opts = %W(
			nuget pack #{nuspec} -Symbols -Version #{configatron.version.full} -OutputDirectory #{configatron.distribution.dir}
		)

		sh(*opts) do |ok, status|
			ok or fail "Command failed with status (#{status.exitstatus})"
		end
	end
end