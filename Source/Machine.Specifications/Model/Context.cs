using System;
using System.Collections.Generic;
using System.Linq;

using Machine.Specifications.Utility;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications.Model
{
    public class Context
    {
        readonly List<Specification> _specifications;
        readonly object _instance;

        readonly Subject _subject;
        readonly IEnumerable<Delegate> _contextClauses;
        readonly IEnumerable<Delegate> _becauseClauses;
        readonly IEnumerable<Delegate> _cleanupClauses;

        readonly IEnumerable<Tag> _tags;

        public string Name { get; private set; }

        public bool IsIgnored { get; private set; }

        public bool IsSetupForEachSpec { get; set; }

        public IEnumerable<Tag> Tags
        {
            get { return _tags; }
        }

        public object Instance
        {
            get { return _instance; }
        }

        public IEnumerable<Specification> Specifications
        {
            get { return _specifications; }
        }

        public Type Type { get; private set; }

        public Subject Subject
        {
            get { return _subject; }
        }

        public Context(Type type,
                       object instance,
                       IEnumerable<Delegate> contextClauses,
                       IEnumerable<Delegate> becauseClauses,
                       IEnumerable<Delegate> cleanupClauses,
                       Subject subject,
                       bool isIgnored,
                       IEnumerable<Tag> tags,
                       bool isSetupForEachSpec)
        {
            Name = type.Name.ToFormat();
            Type = type;
            _instance = instance;
            _cleanupClauses = cleanupClauses;
            _contextClauses = contextClauses;
            _becauseClauses = becauseClauses;
            _specifications = new List<Specification>();
            _subject = subject;
            IsIgnored = isIgnored;
            _tags = tags;
            IsSetupForEachSpec = isSetupForEachSpec;
        }

        public void AddSpecification(Specification specification)
        {
            _specifications.Add(specification);
        }

        public Result EstablishContext()
        {
            var result = Result.Pass();

            try
            {
                _contextClauses.InvokeAll();
                _becauseClauses.InvokeAll();
            }
            catch (Exception err)
            {
                result = Result.ContextFailure(err);
            }

            return result;
        }

        public Result Cleanup()
        {
            var result = Result.Pass();

            try
            {
                _cleanupClauses.InvokeAll();
            }
            catch (Exception err)
            {
                result = Result.ContextFailure(err);
            }

            return result;
        }

        // TODO: Rename to Name
        public string FullName
        {
            get
            {
                var line = "";

                if (Subject != null)
                {
                    line += Subject.FullConcern + ", ";
                }

                return line + Name;
            }
        }

        public bool HasExecutableSpecifications
        {
            get { return Specifications.Where(x => x.IsExecutable).Any(); }
        }
    }

    public class SpecificationVerificationIteration
    {
        public Specification Next { get; private set; }
        public Specification Current { get; private set; }
        public Result Result { get; private set; }

        public SpecificationVerificationIteration(Specification current, Result result, Specification next)
        {
            Next = next;
            Current = current;
            Result = result;
        }
    }
}
