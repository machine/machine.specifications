require 'fileutils'

class NUnitRunner
	include FileTest

	def initialize(paths)
		@resultsDir = paths.fetch(:results, 'results')
		@platform = paths.fetch(:platform, 'x86')
		@clrVersion = paths.fetch(:clr_version, 'net-3.5')
		
		# Check if we are running in TeamCity and somebody supplies us with the addin path.
    rm_rf 'Tools/NUnit/addins'
    if ENV["teamcity.dotnet.nunitaddin"]
			# We are not using the TeamCity NUnit launcher. We use NUnit with the TeamCity NUnit Addin which needs to be 
      # copied to our NUnit addins folder
			# http://blogs.jetbrains.com/teamcity/2008/07/28/unfolding-teamcity-addin-for-nunit-secrets/
			teamCityAddinPath = ENV["teamcity.dotnet.nunitaddin"]
      mkdir_p 'Tools/NUnit/addins'
      cp Dir.glob("#{teamCityAddinPath.gsub("\\", "/")}-2.5.4.*"), 'Tools/NUnit/addins'
		end
	
		@nunitExe = File.join('tools', 'nunit', "nunit-console#{(@platform.nil? ? '' : "-#{@platform}")}.exe") 
	end
	
	def execute(assemblies)
		Dir.mkdir @resultsDir unless exists?(@resultsDir)
		
		assemblies.each do |assem|
			sh "#{@nunitExe} #{assem.escape} /nothread /xml=\"#{@resultsDir}/#{assem.name}.xml\" /framework=#{@clrVersion}"
		end
	end
end