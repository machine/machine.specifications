using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;

namespace Machine.Specifications.Runner.Utility
{
    public class CompileContext : IDisposable
    {
        private readonly string _directory = Path.GetDirectoryName(typeof(CompileContext).Assembly.Location);

        private string _filename;

        public AssemblyPath Compile(string code, params string[] references)
        {
            _filename = Path.Combine(_directory, Guid.NewGuid() + ".dll");

            var parameters = new CompilerParameters
            {
                OutputAssembly = _filename
            };

            parameters.ReferencedAssemblies.AddRange(new []
            {
                "System.dll",
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

            return new AssemblyPath(_filename);
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
