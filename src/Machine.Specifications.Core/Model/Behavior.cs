using System.Collections.Generic;
using Machine.Specifications.Utility.Internal;
using System;

namespace Machine.Specifications.Model
{
    public class Behavior
    {
        private readonly List<Specification> specifications = new List<Specification>();

        public Behavior(Type fieldType, object instance, Context context, bool isIgnored)
        {
            Instance = instance;
            Context = context;
            Leader = fieldType.ToFormat();
            IsIgnored = isIgnored;
        }

        public string Leader { get; }

        public bool IsIgnored { get; }

        public object Instance { get; }

        public IEnumerable<Specification> Specifications => specifications;

        public Context Context { get; }

        public void AddSpecification(Specification specification)
        {
            specifications.Add(specification);
        }
    }
}
