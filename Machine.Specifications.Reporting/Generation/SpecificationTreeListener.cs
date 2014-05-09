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

        public override void OnRunStart()
        {
            _nextId = 1;
        }

        public override void OnRunEnd()
        {
            _run = new Run(_assemblies);
        }

        public override void OnAssemblyStart(AssemblyInfo assembly)
        {
            _concernsToContexts = new Dictionary<string, List<Context>>();
        }

        public override void OnAssemblyEnd(AssemblyInfo assembly)
        {
            var concerns = CreateConcerns();

            _assemblies.Add(assembly.ToNode(concerns));
        }

        IEnumerable<Concern> CreateConcerns()
        {
            return _concernsToContexts.Select(x => new Concern(x.Key, x.Value));
        }

        public override void OnContextStart(ContextInfo context)
        {
            _specifications = new List<Specification>();
        }

        public override void OnContextEnd(ContextInfo context)
        {
            if (!_concernsToContexts.ContainsKey(context.Concern))
            {
                _concernsToContexts[context.Concern] = new List<Context>();
            }

            _concernsToContexts[context.Concern].Add(context.ToNode(_specifications));
        }

        public override void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            _specifications.Add(AssignId(specification.ToNode(result)));
        }

        Specification AssignId(Specification node)
        {
            node.Id = _nextId.ToString(CultureInfo.InvariantCulture);
            _nextId++;
            return node;
        }
    }
}


