class Configatron
  # Helpful for making using configatron with Rails even easier!
  # 
  # To get started you can use the generator to generate
  # the necessary stub files.
  # 
  #   $ ruby script/generate configatron
  module Rails
    
    # Loads configatron files in the following order:
    # 
    # Example:
    #   <Rails.root>/config/configatron/defaults.rb
    #   <Rails.root>/config/configatron/<Rails.env>.rb
    #   # optional:
    #   <Rails.root>/config/configatron/<Rails.env>/defaults.rb
    #   <Rails.root>/config/configatron/<Rails.env>/bar.rb
    #   <Rails.root>/config/configatron/<Rails.env>/foo.rb
    def self.init(root = nil, env = nil)
      base_dir = root
      if root.nil?
        root = defined?(Rails) ? ::Rails.root : FileUtils.pwd
        base_dir = File.expand_path(File.join(root, 'config', 'configatron'))
      end
      
      if env.nil?
        env = defined?(Rails) ? ::Rails.env : 'development'
      end
      
      config_files = []

      config_files << File.join(base_dir, 'defaults.rb')
      config_files << File.join(base_dir, "#{env}.rb")

      env_dir = File.join(base_dir, env)
      config_files << File.join(env_dir, 'defaults.rb')

      Dir.glob(File.join(env_dir, '*.rb')).sort.each do |f|
        config_files << f
      end

      config_files.collect! {|config| File.expand_path(config)}.uniq!

      config_files.each do |config|
        if File.exists?(config)
          # puts "Configuration: #{config}"
          require config
        end
      end
    end
    
  end # Rails
end # Configatron