using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Runner.Utility;

namespace Machine.Specifications.Reporting.Generation
{
    public class SpecificationTreeListener : SpecificationRunListenerBase
    {
        Run _run;
        int _nextId;
        readonly List<Assembly> _assemblies = new List<Assembly>();
        Dictionary<string, List<Context>> _concernsToContexts = new Dictionary<string, List<Context>>();
        List<Specification> _specifications = new List<Specification>();

        public Run Run
        {
            get { return _run; }
        }

        protected override void OnRunStart()
        {
            _nextId = 1;
        }

        protected override void OnRunEnd()
        {
            _run = new Run(_assemblies);
        }

        protected override void OnAssemblyStart(AssemblyInfo assembly)
        {
            _concernsToContexts = new Dictionary<string, List<Context>>();
        }

        protected override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            var concerns = CreateConcerns();

            _assemblies.Add(assembly.ToNode(concerns));
        }

        IEnumerable<Concern> CreateConcerns()
        {
            return _concernsToContexts.Select(x => new Concern(x.Key, x.Value));
        }

        protected override void OnContextStart(ContextInfo context)
        {
            _specifications = new List<Specification>();
        }

        protected override void OnContextEnd(ContextInfo context)
        {
            if (!_concernsToContexts.ContainsKey(context.Concern))
            {
                _concernsToContexts[context.Concern] = new List<Context>();
            }

            _concernsToContexts[context.Concern].Add(context.ToNode(_specifications));
        }

        protected override void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        protected override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _specifications.Add(AssignId(specification.ToNode(result)));
        }

        protected override void OnFatalError(ExceptionResult exception)
        {
        }

        Specification AssignId(Specification node)
        {
            node.Id = _nextId.ToString(CultureInfo.InvariantCulture);
            _nextId++;
            return node;
        }
    }
}


