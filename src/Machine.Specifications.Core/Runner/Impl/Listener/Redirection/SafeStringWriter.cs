using System.IO;
using System.Text;

namespace Machine.Specifications.Runner.Impl.Listener.Redirection
{
    internal class SafeStringWriter : StringWriter
    {
        private readonly StringBuilder buffer = new StringBuilder();

        private static readonly object Lock = new object();

        public override void Write(char value)
        {
            lock (Lock)
            {
                buffer.Append(value);
            }
        }

        public override string ToString()
        {
            lock (Lock)
            {
                return buffer.ToString();
            }
        }
    }
}
