using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;

namespace Machine.Specifications.ConsoleRunner.Specs
{
    public class CompileContext : IDisposable
    {
        private readonly string _directory = Path.GetDirectoryName(typeof(CompileContext).Assembly.Location);
        
        public string Compile(string code, params string[] references)
        {
            var filename = Path.Combine(_directory, Guid.NewGuid() + ".dll");

            var parameters = new CompilerParameters
            {
                OutputAssembly = filename
            };

            parameters.ReferencedAssemblies.AddRange(new []
            {
                "Machine.Specifications.dll",
                "Machine.Specifications.Should.dll"
            });

            if (references.Any())
                parameters.ReferencedAssemblies.AddRange(references);

            var options = new Dictionary<string, string> {{"CompilerVersion", "v4.0"}};

            var provider = new CSharpCodeProvider(options);
            var results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.Count > 0)
                throw new InvalidOperationException();

            return filename;
        }

        public void Dispose()
        {
            var files = Directory.GetFiles(_directory, "*.dll")
                .Where(x => Guid.TryParse(Path.GetFileNameWithoutExtension(x), out _));

            foreach (var file in files)
            {
                SafeDelete(file);
            }
        }

        private void SafeDelete(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch
            {
            }
        }
    }
}
