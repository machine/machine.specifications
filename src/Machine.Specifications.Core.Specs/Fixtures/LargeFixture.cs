using System.Text;

namespace Machine.Specifications.Specs.Fixtures
{
    public class LargeFixture
    {
        public static string CreateCode(int specCount)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Machine.Specifications;

namespace Example.Large
{
    public class when_there_are_many_contexts
    {
        public static bool Created = false;

        public when_there_are_many_contexts()
        {
            Created = true;
        }

        It spec = () => {};
    }

    public static class OtherTests
    {
        public static bool Created = false;
    }

");

            for (var i = 1; i <= specCount; i++)
            {
                sb.AppendLine($@"
    public class when_there_are_many_contexts_{i}
    {{
        public when_there_are_many_contexts_{i}()
        {{
            OtherTests.Created = true;
        }}

        It spec = () => {{}};
    }}");
            }

            sb.AppendLine(@"
}");

            return sb.ToString();
        }
    }
}
