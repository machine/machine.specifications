require 'fileutils'

class NUnitRunner
	include FileTest

	def initialize(args)
		tool = args.fetch(:tool)
		@resultsDir = args.fetch(:results, 'results')
		@clrVersion = args.fetch(:clr_version, 'net-3.5')
		
		# Check if we are running in TeamCity and somebody supplies us with the addin path.
    addin_dir = tool.dirname + '/addins'
    rm_rf addin_dir
    
    if ENV["teamcity.dotnet.nunitaddin"]
			# We are not using the TeamCity NUnit launcher. We use NUnit with the TeamCity NUnit Addin which needs to be 
      # copied to our NUnit addins folder
			# http://blogs.jetbrains.com/teamcity/2008/07/28/unfolding-teamcity-addin-for-nunit-secrets/
			teamCityAddinPath = ENV["teamcity.dotnet.nunitaddin"]
      
      mkdir_p addin_dir
      
      tool_version = tool.match(%r|NUnit\.(\d+\.\d+\.\d+)\.\d+/|).captures.first
      
      cp Dir.glob("#{teamCityAddinPath.gsub("\\", "/")}-#{tool_version}.*"), addin_dir
		end
	
		@nunitExe = tool
	end
	
	def execute(assemblies)
		mkdir_p @resultsDir unless exists?(@resultsDir)
		
		assemblies.each do |assem|
			sh @nunitExe, assem, '/nothread', "/xml=""#{@resultsDir}/#{assem.name}.xml""" #, "/framework=#{@clrVersion}"
		end
	end
end