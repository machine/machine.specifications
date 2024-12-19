using System;
using System.Collections.Generic;
using Machine.Specifications.Runner.VisualStudio.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public interface ISpecificationExecutor
    {
        void RunAssemblySpecifications(string assemblyPath, IEnumerable<VisualStudioTestIdentifier> specifications, Uri adapterUri, IFrameworkHandle frameworkHandle);
    }
}
