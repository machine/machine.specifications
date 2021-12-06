using System;

namespace Machine.Specifications.Reflection
{
    internal class Member
    {
        public string Name { get; set; }

        public Func<object> ValueGetter { get; set; }
    }
}
