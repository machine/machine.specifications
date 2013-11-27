using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Application;
using JetBrains.Application.Extensions;
using JetBrains.Metadata.Utils;
using JetBrains.ReSharper.Psi.Impl.Reflection2.ExternalAnnotations;
using JetBrains.Util;

namespace Machine.Specifications.ReSharper.Provider.Annotations
{
    // This is a bit of a hack. The current version of Machine.Specifications.dll looks for attributes
    // to identify the delegate types, and previous versions didn't include that. We use external
    // annotations to retroactively add the attributes. This is a bit cheeky, as they will be visible
    // to the project (e.g. in QuickDoc windows), but it makes it easy to support older versions
    [ShellComponent]
    public class AnnotationsLoader : IExternalAnnotationsFileProvider
    {
        private readonly OneToSetMap<string, FileSystemPath> _annotations;

        public AnnotationsLoader(ExtensionManager extensionManager)
        {
            if (extensionManager.IsInstalled("Machine.Specifications"))
                return;

            var location = FileSystemPath.CreateByCanonicalPath(Assembly.GetExecutingAssembly().Location)
                .Directory.Combine("ExternalAnnotations");
            _annotations = new OneToSetMap<string, FileSystemPath>(StringComparer.OrdinalIgnoreCase);

            // Cache the annotation filenames to save scanning the directory multiple times.
            // Safe to cache as the user shouldn't be changing files in the install dir. If
            // they want to add extra annotations, use an extension.
            // The rules are simple: either the file is named after the assembly, or the folder
            // is (or both, but that doesn't matter)
            foreach (var file in location.GetChildFiles("*.xml"))
                _annotations.Add(file.NameWithoutExtension, file);
            foreach (var directory in location.GetChildDirectories())
            {
                foreach (var file in directory.GetChildFiles("*.xml", PathSearchFlags.RecurseIntoSubdirectories))
                {
                    _annotations.Add(file.NameWithoutExtension, file);
                    _annotations.Add(file.Directory.Name, file);
                }
            }
        }

        public IEnumerable<FileSystemPath> GetAnnotationsFiles(AssemblyNameInfo assemblyName, FileSystemPath assemblyLocation)
        {
            return _annotations[assemblyName.Name];
        }
    }
}