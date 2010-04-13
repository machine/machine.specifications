require 'fileutils'

class NUnitRunner
	include FileTest

	def initialize(paths)
		@resultsDir = paths.fetch(:results, 'results')
		@compilePlatform = paths.fetch(:platform, 'x86')
		@clrVersion = paths.fetch(:clr_version, 'net-3.5')
		
		if ENV["teamcity.dotnet.nunitlauncher"] # check if we are running in TeamCity
			# We are not using the TeamCity nunit launcher. We use NUnit with the TeamCity NUnit Addin which needs to be copied to our NUnit addins folder
			# http://blogs.jetbrains.com/teamcity/2008/07/28/unfolding-teamcity-addin-for-nunit-secrets/
			# The teamcity.dotnet.nunitaddin environment variable is not available until TeamCity 4.0, so we hardcode it for now
			@teamCityAddinPath = ENV["teamcity.dotnet.nunitaddin"] ? ENV["teamcity.dotnet.nunitaddin"] : 'c:/TeamCity/buildAgent/plugins/dotnetPlugin/bin/JetBrains.TeamCity.NUnitAddin-NUnit'
			cp @teamCityAddinPath + '-2.4.7.dll', 'tools/nunit/addins'
		end
	
		@nunitExe = File.join('tools', 'nunit', "nunit-console#{(@compilePlatform.nil? ? '' : "-#{@compilePlatform}")}.exe") 
	end
	
	def executeTests(assemblies)
		Dir.mkdir @resultsDir unless exists?(@resultsDir)
		
		assemblies.each do |assem|
			sh "#{@nunitExe} #{assem.escape} /nothread /xml=\"#{@resultsDir}/#{assem.name}.xml\" /framework=#{@clrVersion}"
		end
	end
end