using System;
using System.Reflection;
using Machine.Specifications.Runner.Impl;
using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications.Model
{
    public class Specification
    {
        private readonly Delegate it;

        public Specification(string name, Type fieldType, Delegate it, bool isIgnored, FieldInfo fieldInfo)
        {
            this.it = it;

            Leader = fieldType.ToFormat();
            Name = name;
            IsIgnored = isIgnored;
            FieldInfo = fieldInfo;
        }

        public FieldInfo FieldInfo { get; }

        public string Name { get; }

        public bool IsIgnored { get; }

        public string Leader { get; }

        public bool IsDefined => it != null;

        public bool IsExecutable => IsDefined && !IsIgnored;

        public virtual Result Verify()
        {
            if (IsIgnored)
            {
                return Result.Ignored();
            }

            if (!IsDefined)
            {
                return Result.NotImplemented();
            }

            try
            {
                InvokeSpecificationField();
            }
            catch (Exception exception)
            {
                return Result.Failure(exception);
            }

            return Result.Pass();
        }

        protected virtual void InvokeSpecificationField()
        {
            var runner = new DelegateRunner(it);
            runner.Execute();
        }
    }
}
