version = File.read(File.expand_path("../VERSION", __FILE__)).strip

Gem::Specification.new do |spec|
  spec.platform          = Gem::Platform::RUBY
  spec.name              = 'machine.specifications'
  spec.version           = version
  spec.files             = Dir['lib/**/*']
  spec.summary           = 'Machine.Specifications Context/Specification framework'
  spec.description       = 'Machine.Specifications is a Context/Specification framework geared towards removing language noise and simplifying tests.'
  spec.authors           = ['Aaron Jensen', 'Alexander Groß']
  spec.email             = 'agross@therightstuff.de'
  spec.homepage          = 'http://github.com/machine/machine.specifications'
  spec.rubyforge_project = 'machine.specifications'
end