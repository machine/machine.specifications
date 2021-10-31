using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Utility;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications.Model
{
    public class Context
    {
        private readonly List<Specification> specifications;

        private readonly IEnumerable<Delegate> contextClauses;

        private readonly IEnumerable<Delegate> becauseClauses;

        private readonly IEnumerable<Delegate> cleanupClauses;

        public Context(
            Type type,
            object instance,
            IEnumerable<Delegate> contextClauses,
            IEnumerable<Delegate> becauseClauses,
            IEnumerable<Delegate> cleanupClauses,
            Subject subject,
            bool isIgnored,
            IEnumerable<Tag> tags,
            bool isSetupForEachSpec)
        {
            this.cleanupClauses = cleanupClauses;
            this.contextClauses = contextClauses;
            this.becauseClauses = becauseClauses;

            Name = type.Name.ToFormat();
            Type = type;
            Instance = instance;
            specifications = new List<Specification>();
            Subject = subject;
            IsIgnored = isIgnored;
            Tags = tags;
            IsSetupForEachSpec = isSetupForEachSpec;
        }

        public string Name { get; }

        public bool IsIgnored { get; }

        public bool IsSetupForEachSpec { get; }

        public IEnumerable<Tag> Tags { get; }

        public object Instance { get; }

        public IEnumerable<Specification> Specifications => specifications;

        public Type Type { get; }

        public Subject Subject { get; }

        // TODO: Rename to Name
        public string FullName
        {
            get
            {
                var line = string.Empty;

                if (Subject != null)
                {
                    line += Subject.FullConcern + ", ";
                }

                return line + Name;
            }
        }

        public bool HasExecutableSpecifications
        {
            get { return Specifications.Any(x => x.IsExecutable); }
        }

        public void AddSpecification(Specification specification)
        {
            specifications.Add(specification);
        }

        public void Filter(IEnumerable<Specification> toKeep)
        {
            var newList = specifications.Intersect(toKeep).ToList();

            specifications.Clear();
            specifications.AddRange(newList);
        }

        public Result EstablishContext()
        {
            var result = Result.Pass();

            try
            {
                contextClauses.InvokeAll();
                becauseClauses.InvokeAll();
            }
            catch (Exception ex)
            {
                result = Result.ContextFailure(ex);
            }

            return result;
        }

        public Result Cleanup()
        {
            var result = Result.Pass();

            try
            {
                cleanupClauses.InvokeAll();
            }
            catch (Exception err)
            {
                result = Result.ContextFailure(err);
            }

            return result;
        }
    }
}
