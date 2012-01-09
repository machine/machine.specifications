require 'win32/registry'
include Win32

class MSBuild
	def self.compile(attributes)
		version = attributes.fetch(:version, 'v3.5')
		projectFile = attributes.fetch(:project).to_absolute
		userProperties = attributes.fetch(:properties, {})
		userSwitches = attributes.fetch(:switches, {})
		
		frameworkDir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', version)
		
		Registry::HKEY_LOCAL_MACHINE.open("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\#{version}") do |reg|
			frameworkDir = reg['InstallPath', Win32::Registry::REG_SZ]
		end
		
		msbuild = File.join(frameworkDir, 'msbuild.exe')
		
		properties = {
			:BuildInParallel => false,
			:BuildRunner => 'Rake',
			:Configuration => 'Debug',
			:Platform => 'AnyCPU'
		}
		properties.merge!(userProperties)
		
		properties = properties.collect { |key, value|
			"/property:#{key}=#{value}"
		}.join " "
		
		switches = {
			:maxcpucount => true,
			:target => 'Build'
		}
		switches.merge!(userSwitches)
		
		switches = switches.collect { |key, value|
			"/#{key}#{":#{value}" unless value.kind_of? TrueClass or value.kind_of? FalseClass}" if value
		}.join " "
		
		sh "#{msbuild.escape} #{projectFile.escape} #{switches} #{properties}"
	end
end