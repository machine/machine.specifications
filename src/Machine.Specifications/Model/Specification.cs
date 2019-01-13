using System;
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

        public Specification(string name, Type fieldType, Delegate it, bool isIgnored, FieldInfo fieldInfo)
        {
            _leader = fieldType.ToFormat();
            _name = name;
            _it = it;
            _isIgnored = isIgnored;
            _fieldInfo = fieldInfo;
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
            _it.DynamicInvoke();
        }
    }
}