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

task :rebuild => [ :clean, :configure, :restore, :build, :templates ]

task :default => [ :rebuild, :tests ]