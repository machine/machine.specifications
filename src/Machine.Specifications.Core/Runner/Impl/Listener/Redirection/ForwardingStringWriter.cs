using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Machine.Specifications.Utility;

namespace Machine.Specifications.Runner.Impl.Listener.Redirection
{
    internal class ForwardingStringWriter : TextWriter
    {
        readonly TextWriter[] _inner;

        public ForwardingStringWriter(TextWriter[] inner)
            : base(CultureInfo.CurrentCulture)
        {
            _inner = inner;
        }

        public override Encoding Encoding
        {
            get
            {
                var first = _inner.FirstOrDefault();
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
            _inner.Each(x => x.Write(value));
        }
    }
}