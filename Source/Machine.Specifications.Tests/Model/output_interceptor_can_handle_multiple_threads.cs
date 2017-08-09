using System;
using System.Threading.Tasks;
using Machine.Specifications.Runner.Impl.Listener.Redirection;
using NUnit.Framework;

namespace Machine.Specifications.Tests.Model
{
    [TestFixture]
    public class output_interceptor_can_handle_multiple_threads
    {
        /* Error manifesting during test runs as:
         * 
         * ERROR System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
         *  Parameter name: chunkLength
         *  at System.Text.StringBuilder.ToString()
         */

        [Test]
        public void for_example()
        {
            var outputInterceptor = new OutputInterceptor();
            outputInterceptor.CaptureStandardOut();
            var result = string.Empty;

            Parallel.For(0, 100, i =>
            {
                Console.WriteLine(i.ToString());
                result = outputInterceptor.CapturedOutput; //"do something" with captured output to invoke ToString method
            });

            Assert.NotNull(result);
        }
    }
}
