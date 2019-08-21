using System;
using System.Linq;
using System.Text;

using Machine.Specifications.Reporting.Model;

using Spark;

namespace Machine.Specifications.Reporting.Generation.Spark
{
    public abstract class SparkView : AbstractSparkView
    {
        public Run Model
        {
            get;
            set;
        }

        public string H(object value)
        {
            return HtmlEncode(Convert.ToString(value));
        }

        public static string Pluralize(string caption, int count)
        {
            if (count > 1 || count == 0)
            {
                caption += "s";
            }

            return caption;
        }

        private string HtmlEncode(string value)
        {
            if (string.IsNullOrEmpty(value) || !ShouldEncode(value))
                return value;

            var encoded = new StringBuilder();

            foreach (var c in value)
            {
                if (c == '&')
                    encoded.Append("&amp;");
                else if (c == '"')
                    encoded.Append("&quot;");
                else if (c == '<')
                    encoded.Append("&lt;");
                else if (c == '>')
                    encoded.Append("&gt;");
                else if (c == '\'')
                    encoded.Append("&#39;");
                else if (c > 159 && c < 256)
                    encoded.Append($"&#{Convert.ToInt32(c).ToString()};");
                else
                    encoded.Append(c);
            }

            return encoded.ToString();
        }

        private bool ShouldEncode(string value)
        {
            bool IsHtmlChar(char c)
            {
                var highChar = c > 159 && c < 256;

                return c == '&' || c == '"' || c == '<' || c == '>' || c == '\'' || highChar;
            }

            return value.Any(IsHtmlChar);
        }
    }
}
