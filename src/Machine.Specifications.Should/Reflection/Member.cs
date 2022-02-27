using System;

namespace Machine.Specifications.Reflection
{
    internal class Member
    {
        public Member(string name, Func<object> valueGetter)
        {
            Name = name;
            ValueGetter = valueGetter;
        }

        public string Name { get; }

        public Func<object> ValueGetter { get; }
    }
}
