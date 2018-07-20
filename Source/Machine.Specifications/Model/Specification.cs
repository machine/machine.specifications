using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Machine.Specifications.Utility.Internal;

namespace Machine.Specifications.Model
{
    public class Specification
    {
        readonly string _name;
        readonly Delegate _it;
        readonly bool _isIgnored;
        readonly FieldInfo _fieldInfo;
        readonly string _leader;
        readonly Prerequesite[]  _prerequisites;

        public FieldInfo FieldInfo
        {
            get { return _fieldInfo; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsIgnored
        {
            get { return _isIgnored; }
        }

        public string Leader
        {
            get { return _leader; }
        }


        // TODO: Use a dedicated class for prerequisites, instead of Tuples.
        public Specification(string name, Type fieldType, Delegate it, bool isIgnored, FieldInfo fieldInfo, Prerequesite[] prerequisites) 
        {
            this._leader = fieldType.ToFormat();
            this._name = name;
            this._it = it;
            this._isIgnored = isIgnored;
            this._fieldInfo = fieldInfo;
            this._prerequisites = prerequisites;
        }

        public Specification(string name, Type fieldType, Delegate it, bool isIgnored, FieldInfo fieldInfo) : this(name, fieldType, it, isIgnored, fieldInfo, new Prerequesite[0])
        {
        }

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

        public bool IsDefined
        {
            get { return _it != null; }
        }

        public bool IsExecutable
        {
            get { return IsDefined && !IsIgnored; }
        }

        protected virtual void InvokeSpecificationField()
        {
            foreach (var prerequisite in _prerequisites)
            {
                try
                {
                    prerequisite.Condition.DynamicInvoke(this);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The requirements expressed by the following specification were not met: \r\nRequires {prerequisite.SpecificationField.Name.ToFormat()}\r\n\r\nSpecification: {prerequisite.SpecificationField.DeclaringType}.{prerequisite.SpecificationField.Name}");
                    throw new PrerequisiteNotMetException(
                        e.InnerException?.Message,
                        // unwrap the exception.
                        e.InnerException);
                }
            }
            _it.DynamicInvoke();
        }
    }
}