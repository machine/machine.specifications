#
# $Id$
#

require 'pathname'

$ProjectKey = :deferred_service_resolution

class Projects
  def initialize
  end

  def self.get_framework(version='2.0.50727')
    Pathname.new("C:/WINDOWS/Microsoft.NET/Framework/v#{version}")
  end

  def self.get_msbuild
    directory = get_framework('3.5')
    if !directory.directory? then
      directory = get_framework()
    end
    directory.join("MsBuild.exe")
  end

  def self.get_map
    dir = Pathname.new(Dir.pwd)
    while dir do
      if dir.join("pathing.rb").file? then
        break
      end
      if dir == dir.dirname then
        dir = nil
        break
      end
      dir = dir.dirname
    end
    if dir.nil? then
      raise "Unable to locate pathing.rb!"
    end
    require dir.join("pathing.rb")
    $Copies[$ProjectKey] || []
  end

  def self.get_home
  end
end

def msbuild(project, target=nil)
  path = Projects.get_msbuild
  sh "#{path} #{project}" unless target
  sh "#{path} #{project} /t:#{target}" if target
end

task :default do
  msbuild("Castle.Facilities.DeferredServiceResolution.sln", "Rebuild")
end

task :copy => [ :default ] do
  Projects.get_map.each do |destiny|
    cp "Build/Debug/Castle.Facilities.DeferredServiceResolution.dll", destiny
    cp "Build/Debug/Castle.Facilities.DeferredServiceResolution.pdb", destiny
  end
end

# EOF
