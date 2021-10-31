using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl.Listener.Redirection
{
    internal class ForwardingStringWriter : TextWriter
    {
        private readonly TextWriter[] inner;

        public ForwardingStringWriter(TextWriter[] inner)
            : base(CultureInfo.CurrentCulture)
        {
            this.inner = inner;
        }

        public override Encoding Encoding
        {
            get
            {
                var first = inner.FirstOrDefault();

                if (first == null)
                {
#if NETSTANDARD
                    return Encoding.UTF8;
#else

                    return Encoding.Default;
#endif
                }

                return first.Encoding;
            }
        }

        public override void Write(char value)
        {
            inner.Each(x => x.Write(value));
        }
    }
}
