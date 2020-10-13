using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp;

#if NETCOREAPP
using Microsoft.CSharp.RuntimeBinder;
#endif

namespace Machine.Specifications.Runner.Utility
{
    public class CompileContext : IDisposable
    {
        private readonly string _directory = Path.GetDirectoryName(typeof(CompileContext).Assembly.Location);

        private string _filename;

#if NETCOREAPP
        public AssemblyPath Compile(string code, params string[] references)
        {
            _filename = Path.Combine(_directory, Guid.NewGuid() + ".dll");

            var codeString = SourceText.From(code);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);

            var neededAssemblies = new[]
            {
                "System.Console",
                "System.Runtime",
                "System.Runtime.Extensions",
                "System.Private.CoreLib",
                "netstandard",
                "Machine.Specifications",
                "Machine.Specifications.Should"
            };

            var metadataReferences = trustedAssembliesPaths
                .Where(p => neededAssemblies.Contains(Path.GetFileNameWithoutExtension(p)))
                .Select(p => MetadataReference.CreateFromFile(p))
                .Concat(references.Select(x => MetadataReference.CreateFromFile(x)));

            var result = CSharpCompilation.Create(Path.GetFileName(_filename),
                    new[] {parsedSyntaxTree},
                    metadataReferences,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .Emit(_filename);

            if (!result.Success)
                throw new InvalidOperationException();

            return new AssemblyPath(_filename);
        }
#else

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
#endif
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
