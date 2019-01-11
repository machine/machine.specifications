using System.Collections.Generic;
using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Generation
{
    public static class SpecificationTreeMapping
    {
        public static Assembly ToNode(this AssemblyInfo assemblyInfo, IEnumerable<Concern> concerns)
        {
            return new Assembly(assemblyInfo, concerns);
        }

        public static Context ToNode(this ContextInfo contextInfo, IEnumerable<Specification> specifications)
        {
            return new Context(contextInfo, specifications);
        }

        public static Specification ToNode(this SpecificationInfo specification, Result result)
        {
            return new Specification(specification, result);
        }
    }
}