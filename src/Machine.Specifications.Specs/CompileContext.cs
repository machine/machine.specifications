using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace Machine.Specifications.Specs
{
    //public class CompileContext : IDisposable
    //{
    //    private readonly string _code;
    //    private readonly string _directory = Path.GetDirectoryName(typeof(CompileContext).Assembly.Location);

    //    private string _filename;

    //    public CompileContext(string code)
    //    {
    //        _code = code;
    //    }

    //    public AssemblyName Compile()
    //    {
    //        _filename = Path.Combine(_directory, Guid.NewGuid() + ".dll");

    //        var parameters = new CompilerParameters
    //        {
    //            OutputAssembly = _filename
    //        };

    //        parameters.ReferencedAssemblies.AddRange(new []
    //        {
    //            "Machine.Specifications.dll",
    //            "Machine.Specifications.Should.dll"
    //        });

    //        var options = new Dictionary<string, string> {{"CompilerVersion", "v4.0"}};

    //        var provider = new CSharpCodeProvider(options);
    //        var results = provider.CompileAssemblyFromSource(parameters, _code);

    //        if (results.Errors.Count > 0)
    //            throw new InvalidOperationException();

    //        return new AssemblyName(_filename);
    //    }

    //    public void Dispose()
    //    {
    //        var files = Directory.GetFiles(_directory, "*.dll")
    //            .Where(x => Guid.TryParse(Path.GetFileNameWithoutExtension(x), out _));

    //        foreach (var file in files)
    //        {
    //            SafeDelete(file);
    //        }
    //    }

    //    private void SafeDelete(string filename)
    //    {
    //        try
    //        {
    //            File.Delete(filename);
    //        }
    //        catch
    //        {
    //        }
    //    }
    //}
}
