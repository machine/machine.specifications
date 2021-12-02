using System;
using System.Net;
using System.Text;

namespace Machine.Specifications.Reporting.Integration.AppVeyor
{
    public class AppVeyorBuildWorkerApiClient : IAppVeyorBuildWorkerApiClient
    {
        private const string ApiResource = "/api/tests";

        private readonly string apiUrl;

        public AppVeyorBuildWorkerApiClient(string apiUrl)
        {
            this.apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
        }

        public void AddTest(
            string testName,
            string testFramework,
            string fileName,
            string outcome,
            long? durationMilliseconds,
            string errorMessage,
            string errorStackTrace,
            string stdOut,
            string stdErr)
        {
            try
            {
                var json = Json(
                    testName,
                    testFramework,
                    fileName,
                    outcome,
                    durationMilliseconds,
                    errorMessage,
                    errorStackTrace,
                    TrimStdOut(stdOut),
                    TrimStdOut(stdErr));

                using (var wc = GetClient())
                {
                    wc.UploadData(ApiResource, "POST", json);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error communicating AppVeyor Build Worker API: " + ex.Message);
            }
        }

        public void UpdateTest(
            string testName,
            string testFramework,
            string fileName,
            string outcome,
            long? durationMilliseconds,
            string errorMessage,
            string errorStackTrace,
            string stdOut,
            string stdErr)
        {
            try
            {
                var json = Json(
                    testName,
                    testFramework,
                    fileName,
                    outcome,
                    durationMilliseconds,
                    errorMessage,
                    errorStackTrace,
                    TrimStdOut(stdOut),
                    TrimStdOut(stdErr));

                using (var wc = GetClient())
                {
                    wc.UploadData(ApiResource, "PUT", json);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error communicating AppVeyor Build Worker API: " + ex.Message);
            }
        }

        private static string TrimStdOut(string str)
        {
            const int maxLength = 4096;

            if (str == null)
            {
                return null;
            }

            return str.Length > maxLength
                ? str.Substring(0, maxLength)
                : str;
        }

        private static byte[] Json(
            string testName,
            string testFramework,
            string fileName,
            string outcome,
            long? durationMilliseconds,
            string errorMessage,
            string errorStackTrace,
            string stdOut,
            string stdErr)
        {
            var value = new StringBuilder()
                .Append("{")
                .Append($"{GetJsonValue("TestName")}:{GetJsonValue(testName)},")
                .Append($"{GetJsonValue("TestFramework")}:{GetJsonValue(testFramework)},")
                .Append($"{GetJsonValue("FileName")}:{GetJsonValue(fileName)},")
                .Append($"{GetJsonValue("Outcome")}:{GetJsonValue(outcome)},")
                .Append($"{GetJsonValue("DurationMilliseconds")}:{GetJsonValue(durationMilliseconds)},")
                .Append($"{GetJsonValue("ErrorMessage")}:{GetJsonValue(errorMessage)},")
                .Append($"{GetJsonValue("ErrorStackTrace")}:{GetJsonValue(errorStackTrace)},")
                .Append($"{GetJsonValue("StdOut")}:{GetJsonValue(stdOut)},")
                .Append($"{GetJsonValue("StdErr")}:{GetJsonValue(stdErr)}")
                .Append("}")
                .ToString();

            return Encoding.UTF8.GetBytes(value);
        }

        private static string GetJsonValue(long? value)
        {
            return value == null
                ? "null"
                : value.ToString();
        }

        private static string GetJsonValue(string value)
        {
            if (value == null)
            {
                return "null";
            }

            var jsonValue = value
                .Replace("\\", "\\\\")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");

            return $"\"{jsonValue}\"";
        }

        private WebClient GetClient()
        {
            var client = new WebClient
            {
                BaseAddress = apiUrl
            };

            client.Headers["Accept"] = "application/json";
            client.Headers["Content-type"] = "application/json";

            return client;
        }
    }
}
