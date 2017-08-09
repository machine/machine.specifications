using System;
using System.Threading.Tasks;
using Machine.Specifications.Runner.Impl.Listener.Redirection;
using NUnit.Framework;

namespace Machine.Specifications.Tests.Model
{
    [TestFixture]
    public class ResultTests
    {
        /* Error manifesting during test runs as:
         * 
         * ERROR System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
         *  Parameter name: chunkLength
         *  at System.Text.StringBuilder.ToString()
         */

        [Test]
        public void output_interceptor_can_handle_multiple_threads()
        {
            var thing = new OutputInterceptor();

            Parallel.For(0, 100, i =>
            {
                thing.CaptureStandardOut();
                Console.WriteLine(thing.CapturedOutput);
            });
        }
    }
}
