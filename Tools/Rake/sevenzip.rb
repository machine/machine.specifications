require 'tempfile'

class SevenZip
	attr_accessor :tool, :args, :zipName
	
	def initialize(params = {})
		@tool = params.fetch(:tool).to_absolute
		@args = params.fetch(:args, 'a')
		@zipName = params.fetch(:zip_name).to_absolute
	end
	
	def zip(params = {})
		files = params.fetch(:files)
		
		SevenZip.zip_files @tool, @args, @zipName, files
	end
	
	def self.zip(params = {})
		tool = params.fetch(:tool)
		args = params.fetch(:args, 'a')
		zipName = params.fetch(:zip_name).to_absolute
		files = params.fetch(:files)
		
		zip_files tool, args, zipName, files
	end
	
	def self.unzip(params= {})
		tool = params.fetch(:tool)
		args = params.fetch(:args, 'x -y')
		zipName = params.fetch(:zip_name).to_absolute
		destination = params.fetch(:destination, '.')
		
		sevenZip = tool.to_absolute
		
		sh "#{sevenZip.escape} #{args} #{zipName.escape} -o#{destination.escape}"
	end
	
	def self.zip_files(tool, args, zipName, files)
		return if files.empty?
		
		FileUtils.mkdir_p zipName.dirname
		files.map! do |f|
			f.escape
		end
		
		Tempfile.open('random') do |f|
			f.write files.join("\r\n")
			f.close
			
			sep = File::ALT_SEPARATOR || File::SEPARATOR
			filesToZip = f.path
			filesToZip = filesToZip.gsub("/", sep) if File::ALT_SEPARATOR
			
			sevenZip = tool.to_absolute
			sh "#{sevenZip.escape} #{args} #{zipName.escape} @#{filesToZip.escape}"
		end
	end
end