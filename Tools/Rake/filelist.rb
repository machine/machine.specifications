class FileList
	def copy_hierarchy(attributes)
		self.each do |source|
			target = source.pathmap("%{^#{attributes[:source_dir]},#{attributes[:target_dir]}}p") 
			
			if File.directory? source
				FileUtils.cp_r \
					source,
					target.dirname,
					:verbose => false,
					:preserve => true
				next
			end
			
			FileUtils.mkdir_p target.dirname
			FileUtils.cp \
				source,
				target,
				:verbose => false,
				:preserve => true
		end
	end
end