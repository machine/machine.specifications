# Build tasks specific to .NET 3.5 builds.

namespace :tests do
  task :run do
    puts 'Running Gallio tests...'
    sh "Tools/Gallio/v3.1.397/Gallio.Echo.exe", "#{configatron.outDir}/Tests/Gallio/Machine.Specifications.TestGallioAdapter.3.1.Tests.dll", "/plugin-directory:#{configatron.outDir}", "/r:Local"
  end
end